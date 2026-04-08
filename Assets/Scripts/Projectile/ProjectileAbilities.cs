using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

// handles shooting, recalling, and parrying
public class ProjectileAbilities : MonoBehaviour
{
    [HideInInspector] public UnityEvent<KnifeThrowInfo> knifeThrown = new();
    /// <summary>
    /// Bool represents whether parry was successful
    /// </summary>
    [HideInInspector] public UnityEvent<bool> attemptedParry = new();
    [HideInInspector] public UnityEvent dashPerformed = new();
    [HideInInspector] public UnityEvent recallStarted = new();




    [Header("References")]
    [SerializeField] private Projectile featherKnife;
    [SerializeField] private Transform projectileFirePoint;
    [SerializeField] private PlayerMovementStateMachine playerMovement;
    [SerializeField] private Animator armAnimator;
    [SerializeField] InputManager inputManager;
    [SerializeField] private Camera cam => Camera.main;
    public ApplyShake camShaker;

    [Header("Settings")]
    [SerializeField] private float parryTiming = 0.2f;
    public float recallCooldown = 8f;
    public float currentRecallCooldown;
    [SerializeField] private float parryForce = 500f;
    [SerializeField] private float hitStopTime = 0.1f;
    [SerializeField] private float minParryWindow = 80.0f;
    [SerializeField] private float upwardsBiasForParry = 0.65f;
    [SerializeField] private HUDManager hudManager;
    public ApplyShake.CamShakeProfile parryCamShakeProfile;

    float parryWindow = 0.0f;
    
    [Header("Impact Frame Post Processing Effects")]
    [SerializeField] private Volume postProcessVolume;
    [SerializeField] private VolumeProfile hitstopProfile;
    [SerializeField] private float hitstopEffectFadeIn = 0.05f;
    [SerializeField] private float hitstopEffectFadeOut = 0.1f;
    
    private VolumeProfile originalProfile;
    private Coroutine hitstopEffectCoroutine;
    
    [Header("Input")]
    public InputActionReference fireAction;
    public InputActionReference recallAction;
    public InputActionReference blinkAction;
    
    
    [Header("BlinkProperties")]
    public float blinkCooldownTime;
    public float blinkEffectTime = 0.33f;
    public float currentBlinkTimer;
    public bool canBlink => currentBlinkTimer <= 0.0f && featherKnife.currentState != Projectile.ProjectileState.Idle && featherKnife.currentState != Projectile.ProjectileState.Recalling;

    public PlayerVFXManager playerVFXManager;

    [Header("FMOD events")]
    public string FMODParryEvent = "", FMODShootEvent = "", FMODBlinkEvent = "", FMODParryFailEvent = "";
    public FMOD.Studio.EventInstance parrySFX, parryFailSFX, shootSFX, blinkSFX;

    [Header("Homing")]
    /// <summary>
    /// Maximum distance to consider trying to home towards the target. If exceeded, just treated as a straight shot
    /// </summary>
    [SerializeField] float maxHomingRange = 50.0f;
    [SerializeField] float durationForKnifeToHitTargetDuringHome = 0.4f;
    bool parryActive = false;
    public bool ParryActive { get; private set; } = false;
    bool blinkedThisFrame; // just used for the animator

    List<EnemyType> enemiesToNotHomeTowards = new()
    {
        EnemyType.Banshee
    };

    bool homingPreviously;

    HomeData homeData;
    struct HomeData
    {
        public Vector3 knifeStart;
        public Vector3 enemyPos;
        public float elapsedTime;
    }

    void Start()
    {
        if (postProcessVolume != null)
        {
            originalProfile = postProcessVolume.profile;
        }

        featherKnife.stateChanged.AddListener(OnKnifeStateChanged);
    }

    void Awake()
    {
        if(FMODParryEvent != "")    { parrySFX = FMODUnity.RuntimeManager.CreateInstance(FMODParryEvent); }
        if(FMODShootEvent != "")    { shootSFX = FMODUnity.RuntimeManager.CreateInstance(FMODShootEvent); }
        if(FMODParryFailEvent != ""){ parryFailSFX = FMODUnity.RuntimeManager.CreateInstance(FMODParryFailEvent); }
        if(FMODBlinkEvent != "")    { blinkSFX = FMODUnity.RuntimeManager.CreateInstance(FMODBlinkEvent); }
    }

    //void OnEnable()
    //{
    //    fireAction.action.started += OnFirePressed;
    //    recallAction.action.performed += OnRecallPressed;
    //    blinkAction.action.performed += OnBlinkPressed;
    //}

    //void OnDisable()
    //{
    //    fireAction.action.started -= OnFirePressed;
    //    recallAction.action.performed -= OnRecallPressed;
    //    // blinkAction.action.performed -= OnBlinkPressed;
    //}


    void Update()
    {
        blinkedThisFrame = false;
        if(FMODParryEvent != "") { FMODUnity.RuntimeManager.AttachInstanceToGameObject(parrySFX, transform); }
        if(FMODShootEvent != "") { FMODUnity.RuntimeManager.AttachInstanceToGameObject(shootSFX, transform); }
        if(FMODParryFailEvent != "") { FMODUnity.RuntimeManager.AttachInstanceToGameObject(parryFailSFX, transform); }
        if(FMODBlinkEvent != "") { FMODUnity.RuntimeManager.AttachInstanceToGameObject(blinkSFX, transform); }

        if(armAnimator != null){armAnimator.SetBool("hasDagger", featherKnife.currentState == Projectile.ProjectileState.Idle);}

        if (currentRecallCooldown > 0)
        {
            currentRecallCooldown -= Time.deltaTime;
            
            if (hudManager != null)
            {
                hudManager.UpdateRecallCooldown(currentRecallCooldown, recallCooldown);
            }
        }

        if (currentBlinkTimer > 0) { currentBlinkTimer -= Time.deltaTime; }
        if (HUDManager.Instance != null)
        {
            if (HUDManager.Instance.blinkElement != null)
            {
                HUDManager.Instance.blinkElement.SetReady(canBlink);
                HUDManager.Instance.blinkElement.SetTimer(  currentBlinkTimer);
            }
        }
       
        if (RecallAvailable() && inputManager.IsRecallBuffered())
        {
            PerformRecall();
        }
        if (KnifeFireable() && inputManager.IsFireBuffered())
        {
            PerformFire();
        }
        if (canBlink && inputManager.IsBlinkBuffered())
        {
            StartCoroutine(BlinkCoroutine());
        }
        
    }
    private void FixedUpdate()
    {
        var manager = GameManager.Instance;
        if (manager != null && featherKnife.currentState == Projectile.ProjectileState.Flying && !parryActive)
        {
            var newTarget = manager.entityManager.GetClosestEnemyToPosition(featherKnife.rb.position, enemiesToNotHomeTowards);
            if (newTarget == null) return;
            var targetDistance = Vector3.Distance(newTarget.transform.position, featherKnife.rb.position);
            if (targetDistance <= maxHomingRange)
            {
                if (!homingPreviously)
                {
                    StartHoming(featherKnife.rb.position, newTarget.collider.bounds.center);
                }
                HomeTowardsPosition();
                homingPreviously = true;
            }
        }
    }

    void OnKnifeStateChanged(Projectile.ProjectileState state)
    {
        homingPreviously = false;
    }
    void HomeTowardsPosition()
    {
        homeData.elapsedTime += Time.deltaTime;
        featherKnife.rb.position = Vector3.Slerp(homeData.knifeStart, homeData.enemyPos, homeData.elapsedTime / durationForKnifeToHitTargetDuringHome);
        if (Vector3.Distance(featherKnife.rb.position, homeData.enemyPos) <= 0.1f)
        {
            featherKnife.rb.isKinematic = false;
        }
    }

    void StartHoming(Vector3 knifePos, Vector3 enemyPos)
    {
        homeData.knifeStart = knifePos;
        homeData.enemyPos = enemyPos;
        homeData.elapsedTime = 0;

        featherKnife.rb.isKinematic = true; //manual control;
    }
    public void SetParryWindow(float totalDistance)
    {
        parryWindow = totalDistance * parryTiming;
        if (parryWindow < minParryWindow)
        {
            parryWindow = minParryWindow;
        }
    }

    bool RecallAvailable()
    {
        return featherKnife.currentState != Projectile.ProjectileState.Idle && currentRecallCooldown <= 0.0f;
    }
    public bool KnifeFireable()
    {
        return featherKnife.currentState != Projectile.ProjectileState.Flying && featherKnife.currentState != Projectile.ProjectileState.Embedded;
    }
    public bool IsParryable()
    {
        if (featherKnife.currentState != Projectile.ProjectileState.Recalling) return false;
        float currentDistance = Vector3.Distance(featherKnife.transform.position, transform.position);

        return currentDistance <= parryWindow;
    }
    void PerformFire()
    {
        bool parryable = IsParryable();
        if (featherKnife.currentState == Projectile.ProjectileState.Recalling)
        {
            attemptedParry.Invoke(parryable);
            if (!parryable) parryFailSFX.start();
        }
        TryShoot(parryable);
    }

    void PerformRecall()
    {
        ParryActive = false;
        if (featherKnife.currentState == Projectile.ProjectileState.Flying) { featherKnife.SetEmbeddedPos(); }

        recallStarted.Invoke();
        featherKnife.SetState(Projectile.ProjectileState.Recalling);
        currentRecallCooldown = recallCooldown;


        if (armAnimator != null) { armAnimator.SetTrigger("Recall"); }

        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateRecallCooldown(currentRecallCooldown, recallCooldown);
            HUDManager.Instance.TriggerRecallPrompt();
            HUDManager.Instance.recallElement.Activate();
        }
        if (playerMovement.parryTutorialEvent != null)
        {
            playerMovement.parryTutorialEvent.TryActivate();
        }

        currentRecallCooldown = recallCooldown;
        inputManager.OnRecallPerformed();
    }
    private void Blink()
    {
        blinkedThisFrame = true;

        currentBlinkTimer = blinkCooldownTime;
        playerMovement.Blink();
        featherKnife.Pickup();
        ParryActive = false;
        HUDManager.Instance.blinkElement.Activate();
        HUDManager.Instance.TriggerBlinkPrompt();
        KnifeRetrievalInfo info = new ()
        {
            pickupType = KnifeRetrievalType.Blink
        };
    }

    public void OnPickup()
    {
        if(armAnimator != null && !blinkedThisFrame){armAnimator.SetTrigger("Catch");}
    }
    
    
    public void ResetRecallCooldown()
    {
        currentRecallCooldown = 0f;
    
        if (hudManager != null)
        {
            hudManager.UpdateRecallCooldown(currentRecallCooldown, recallCooldown);
        }
    
        Debug.Log("spooky triad");
    }

    private void TryShoot(bool parry = false)
    {
        if (featherKnife.currentState != Projectile.ProjectileState.Idle && !parry)
            return;
        
        if (cam == null) 
            return;

        inputManager.OnFirePerformed();
        HUDManager.Instance.TriggerShootPrompt();
        
        Shoot(parry);
        if (parry){ StartCoroutine(ParryCoroutine()); }
    }

    private void Shoot(bool parry)
    {
        Vector3 spawnPos;

        ParryActive = parry;
        if (parry)
        {
            // For parry, spawn the projectile further away from camera
            spawnPos = cam.transform.position + cam.transform.forward * 2f; // Increased distance
        }
        else
        {
            // Normal shooting spawn position
            spawnPos = cam.transform.position + cam.transform.forward * 0.5f;
        }
        // Ray from crosshair
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, 200f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(200f); // far point
        }

        Vector3 direction = (targetPoint - spawnPos).normalized;

        // parry multiplier
        if (parry)
            direction *= 2f;

        featherKnife.transform.position = spawnPos;
        featherKnife.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(-90, 180, 0);

        featherKnife.CastProjectile(direction);
    
        if (parry)
        {
            StartCoroutine(ParryCoroutine());
            if(armAnimator != null){armAnimator.SetTrigger("Parry");}
            parrySFX.start();
            HUDManager.Instance.recallElement.Parry();
            featherKnife.OnParry();
        }
        else
        {
            if(armAnimator != null){armAnimator.SetTrigger("Shoot");}
            shootSFX.start();
        }

        KnifeThrowInfo throwInfo = new ()
        {
            parried = parry,
            direction = direction,
        };
        knifeThrown.Invoke(throwInfo);

    }

    private IEnumerator ParryCoroutine()
    {

        // Apply hitstop post processing immediately
        if (postProcessVolume != null && hitstopProfile != null)
        {
            if (hitstopEffectCoroutine != null)
                StopCoroutine(hitstopEffectCoroutine);
            hitstopEffectCoroutine = StartCoroutine(HitstopPostProcessEffect());
        }
        
        // Immediate hitstop
        if(hitStopTime > 0)
        {
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(hitStopTime);
            Time.timeScale = 1f;
        }

       
        var parryImpulse = (transform.up - transform.forward).normalized * parryForce;
        parryImpulse = Vector3.Lerp(parryImpulse, new Vector3(0, parryForce, 0), upwardsBiasForParry);
        //Debug.Log("Applying parry impulse of " + parryImpulse);
        Vector3 newVelocity = parryImpulse;
        if (newVelocity.y < parryForce) newVelocity.y = parryForce;
        playerMovement.rigidbody.linearVelocity = newVelocity;
        if (playerMovement.rigidbody.linearVelocity.y < 0)
        camShaker.StartShake(parryCamShakeProfile);
    }

    private IEnumerator HitstopPostProcessEffect()
    {
        // Smooth transition to hitstop profile
        float timer = 0f;
        while (timer < hitstopEffectFadeIn)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / hitstopEffectFadeIn;
            postProcessVolume.weight = Mathf.SmoothStep(0f, 1f, t);
            postProcessVolume.profile = hitstopProfile;
            yield return null;
        }

        postProcessVolume.weight = 1f;

        // Wait for the actual hitstop duration
        yield return new WaitForSecondsRealtime(hitStopTime);

        // Smooth transition back to original profile
        timer = 0f;
        while (timer < hitstopEffectFadeOut)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / hitstopEffectFadeOut;
            postProcessVolume.weight = Mathf.SmoothStep(1f, 0f, t);
            yield return null;
        }

        postProcessVolume.weight = 0f;
        postProcessVolume.profile = originalProfile;
        hitstopEffectCoroutine = null;
    }

    private IEnumerator BlinkCoroutine()
    {
        float timer = 0f;
        bool hasBlinked = false;
        float blinkTimeMarker = 0.3f;
        blinkSFX.start();
        
        if(armAnimator != null){
            armAnimator.SetTrigger("Blink");
            armAnimator.SetBool("OnWall", false);
        }

        
        
        while (timer < blinkEffectTime)
        {
            float t = timer / blinkEffectTime;
            playerVFXManager.blinkVolume.weight = playerVFXManager.blinkPPCurve.Evaluate(t);
            
            if(t > blinkTimeMarker && !hasBlinked)
            {
                // The actual blink event
                Blink();
                hasBlinked = true;
            } 

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        playerVFXManager.blinkVolume.weight = 0f;
    }

}