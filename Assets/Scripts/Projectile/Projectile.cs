using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder.MeshOperations;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{

    public UnityEvent<KnifeCollisionInfo> enemyStruck = new();
    public UnityEvent<KnifeCollisionInfo> terrainStruck = new();
    public UnityEvent<KnifeRetrievalInfo> knifeRetrieved = new();
    [HideInInspector] public UnityEvent<ProjectileState> stateChanged = new();

    public enum ProjectileState {Idle, Flying, Embedded, Recalling}//, PickedUp}
    public ProjectileState currentState { get; private set; } = ProjectileState.Idle;
    public Vector3 targetLocalScale;

    [Header("Managers")]
    [SerializeField] KnifeParticleManager knifeParticleManager;
    [Header("Flying Settings")]
    public float speed = 30f;
    public float lifetime = 5f;
    
    [Header("Embedded Settings")]
    public float pickUpRadius = 2f;
    public float embedDepth = 0.5f;
    public GameObject hitEffect;
    
    [Header("Idle Settings")]
    public Transform initProjectilePosition, heldParent;
    
    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform;
    public ProjectileAbilities projectileAbilities;
    
    [Header("Camera Layer Data")]

    public Camera armRendererCam;
    public LayerMask armIdleCull, armFlyingCull;
    public Camera mainCam;
    public LayerMask mainIdleCull, mainFlyingCull;

    [Header("RecallData")]
    public Vector3 throwDirection {get; private set;}
    [SerializeField] private AnimationCurve recallAnimationCurve;
    [SerializeField] float baseRecallSpeed = 50f; // Overall speed multiplier
    [SerializeField] float maxSpeedBoost = 3f;      // How much to speed up (3x = 300% speed)
   
    public float TotalRecallDistance { get; private set; }
    private float recallProgress;
    [SerializeField] private Collider thisCol;
    [SerializeField] private Transform centerTf;
    

    [Header("Blink Data")]
    public List<SpaceSample> spaceRecordingData;
    public int maxRecordingSlots;
    public SpaceSample blinkSample;
    public List<Collider> triggerList;
    public bool inTriggerCollision =>  triggerList == null || triggerList.Count != 0;

    
    [Serializable]
    public struct SpaceSample
    {
        public Vector3 position, direction, velocity;
    };

    
    private Rigidbody rb;
    private Collider col;

    // [SerializeField] private Animator animator;

    float elapsedTimeInFlyingState;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (knifeParticleManager != null) knifeParticleManager.InitParticleManager(this, playerTransform);
        spaceRecordingData = new List<SpaceSample>();
        triggerList = new List<Collider>();
        ReturnToIdle();
    }


    public Vector3 GetBlinkPosition()
    {

        bool inCollision = false;
        Collider[] colliders = Physics.OverlapBox(centerTf.position, col.bounds.extents, transform.rotation);
        
        for (int i = 0; i < colliders.Length; i++)
        {
            if(!colliders[i].isTrigger){
                inCollision = true;
                break;
            }
        }

        Debug.Log(inCollision);
        if(
            currentState == ProjectileState.Flying || 
            (currentState == ProjectileState.Recalling && !inCollision)
        )
            return transform.position;

        else{return blinkSample.position;}
    }
    
    void Update()
    {

        if (playerTransform != null && currentState == ProjectileState.Embedded)
        {   
            float distance = Vector3.Distance(playerTransform.position, transform.position);
            if (distance <= pickUpRadius){
                KnifeRetrievalInfo info = new ()
                {
                    pickupType = KnifeRetrievalType.Pickup,
                };
                knifeRetrieved.Invoke(info);
                Pickup(); 
            }
        }

        if(currentState == ProjectileState.Recalling)
        {
            RecallUpdate();
        }
        else
        {
            if(HUDManager.Instance.recallElement != null)
            {
                if( projectileAbilities.currentRecallCooldown <= 0 && currentState != ProjectileState.Recalling && currentState != ProjectileState.Idle) {
                    HUDManager.Instance.recallElement.SetFillFactor(1);
                    HUDManager.Instance.recallElement.SetReady(true);
                }
                else
                {
                    HUDManager.Instance.recallElement.SetFillFactor(( projectileAbilities.recallCooldown - projectileAbilities.currentRecallCooldown) / projectileAbilities.recallCooldown);
                    HUDManager.Instance.recallElement.SetReady(false);
                }
            }
        }
        if (currentState == ProjectileState.Flying)
        {
            elapsedTimeInFlyingState += Time.deltaTime;
            if (elapsedTimeInFlyingState > lifetime)
            {
                ReturnToIdle();
            }
        }
    }

    void FixedUpdate()
    {

        // Preserving position data
        if(currentState == ProjectileState.Idle || currentState == ProjectileState.Embedded){return;}
        
        SpaceSample spaceSample;
        
        spaceSample.position = transform.position;
        spaceSample.direction = transform.forward;
        spaceSample.velocity= rb.linearVelocity;

        if(currentState != ProjectileState.Embedded || spaceRecordingData.Count < maxRecordingSlots)
        {
            spaceRecordingData.Add(spaceSample);
            if(spaceRecordingData.Count > maxRecordingSlots) spaceRecordingData.RemoveAt(0);
        }
    }

    public void SetState(ProjectileState newState)
    {
        currentState = newState;
        
        switch (currentState)
        {
            case ProjectileState.Idle:
                rb.isKinematic = true;
                col.enabled = true;
                col.isTrigger = false;
                projectileAbilities.OnPickup();

                armRendererCam.cullingMask = armIdleCull;
                mainCam.cullingMask = mainIdleCull;
                break;
            case ProjectileState.Flying:
                elapsedTimeInFlyingState = 0.0f;
                armRendererCam.cullingMask = armFlyingCull;
                mainCam.cullingMask = mainFlyingCull;

                rb.isKinematic = false;
                col.isTrigger = false;
                
                break;
            case ProjectileState.Embedded:

                Debug.Log("Remvoing velocity, embedding");
                rb.isKinematic = true;
                col.isTrigger = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                SetEmbeddedPos();
                break;
            case ProjectileState.Recalling:
                // Stop physics immediately

                Debug.Log("Removing velocity, recalling");
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
        
                // Calculate recall parameters
                TotalRecallDistance = Vector3.Distance(transform.position, initProjectilePosition.position);
                recallProgress = 0f;

                // Estimate total time
                break;
            

        }
        stateChanged.Invoke(newState);
    }
    
    void RecallUpdate()
    {
        if (currentState != ProjectileState.Recalling)
            return;

        float currentDistance = Vector3.Distance(transform.position, initProjectilePosition.position);
        recallProgress = 1f - (currentDistance / TotalRecallDistance);
        HUDManager.Instance.recallElement.SetSpeed(recallProgress);
    
      
        // Get curve-based speed multiplier
        float speedMultiplier = recallAnimationCurve.Evaluate(recallProgress);
    
        // Combine both multipliers
        float currentSpeed = baseRecallSpeed * speedMultiplier;
    
        transform.position = Vector3.MoveTowards(transform.position, initProjectilePosition.position, currentSpeed * Time.deltaTime);
    
        // Rotate towards hand
        Vector3 directionToHand = (initProjectilePosition.position - transform.position).normalized;
        if (directionToHand != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToHand) * Quaternion.Euler(-90, 180, 0);
        }
    
        if (currentDistance < 0.1f)
        {
            KnifeRetrievalInfo info = new()
            {
                pickupType = KnifeRetrievalType.Recall
            };
            knifeRetrieved.Invoke(info);
            ReturnToIdle();
        }
    }

    private float CalculateEstimatedRecallTime(float distance)
    {
        // Sample the curve at several points to estimate total time
        float estimatedTime = 0f;
        int samples = 10;

        for (int i = 0; i < samples; i++)
        {
            float progress = (float)i / samples;
            float speedMultiplier = recallAnimationCurve.Evaluate(progress);
        
            // Calculate distance boost for this segment too!
            float segmentDistance = distance * (1f - progress);
            //float distanceBoost = CalculateDistanceBoost(segmentDistance);

            float segmentTime = (distance / samples) / (baseRecallSpeed * speedMultiplier); //* distanceBoost);
            estimatedTime += segmentTime;
        }

        return estimatedTime;
    }

    //private float CalculateDistanceBoost(float currentDistance)
    //{
    //    if (currentDistance > maxBoostDistance)
    //    {
    //        return maxSpeedBoost;
    //    }
    //    else if (currentDistance > maxBoostDistance * 0.7f)
    //    {
    //        float boostProgress = (currentDistance - (maxBoostDistance * 0.7f)) / (maxBoostDistance * 0.3f);
    //        return Mathf.Lerp(1f, maxSpeedBoost, boostProgress);
    //    }
    //    return 1f;
    //}

    public void SetEmbeddedPos(){
        blinkSample = spaceRecordingData[spaceRecordingData.Count - 1];
    }
    public void CastProjectile(Vector3 direction)
    {
        transform.SetParent(null);
        transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(-90, 180, 0);
        transform.localScale = targetLocalScale;
        throwDirection = direction;
        SetState(ProjectileState.Flying);
        rb.linearVelocity = direction * speed;
    }

    private void ReturnToIdle()
    {
        if(HUDManager.Instance != null) HUDManager.Instance.recallElement.Deactivate();
        
        if (currentState != ProjectileState.Flying && currentState != ProjectileState.Recalling)
            return;
        
        SetState(ProjectileState.Idle);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        transform.SetParent(heldParent);
        transform.localPosition = Vector3.up * -0.65f;
        transform.localRotation = Quaternion.identity;
        transform.localScale = targetLocalScale;
        
        gameObject.SetActive(true);
    }
    
    public void OnParry()
    {
        // animator.SetTrigger("Parried");
    }

    public void Pickup()
    {   
        rb.isKinematic = true;
        col.enabled = false;
        gameObject.SetActive(false);

        
        // Instantly return to hand + idle state
        transform.SetParent(heldParent);
        transform.localPosition = Vector3.up * -0.65f;
        transform.localRotation = Quaternion.identity;
        transform.localScale = targetLocalScale;
    
        gameObject.SetActive(true);
        SetState(ProjectileState.Idle);
        if(HUDManager.Instance != null) HUDManager.Instance.recallElement.Deactivate();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger) return;
        
        triggerList.Add(other);
        if(triggerList.Count > 1) return;
        
        int recordedPositionOffset = 3;
        if(currentState == ProjectileState.Recalling)
        {
            // blinkSample = spaceRecordingData[maxRecordingSlots - recordedPositionOffset - 1];
            blinkSample = spaceRecordingData[0];
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.isTrigger)return;
        triggerList.Remove(other);

        bool inCollision = false;
        Collider[] colliders = Physics.OverlapBox(centerTf.position, col.bounds.extents, transform.rotation);
        for (int i = 0; i < colliders.Length; i++)
        {
            if(!colliders[i].isTrigger){
                inCollision = true;
                break;
            }
        }

        if(!inCollision){triggerList = new();}

        if(currentState == ProjectileState.Recalling && !inCollision )
        {
            blinkSample = spaceRecordingData[maxRecordingSlots-1];
        }

    }
  
    public void OnEnemyStruck()
    {
        EmitKnifeCollisionSignal(true);
        if (projectileAbilities != null)
        {
            projectileAbilities.ResetRecallCooldown();
        }

        bool inParryState = projectileAbilities.ProjectileInParryState();

        if (!inParryState && currentState != ProjectileState.Recalling)
        {
            EmbedKnife();
        }
    }
    public void OnObjectStruck()
    {
        EmitKnifeCollisionSignal(false);
        EmbedKnife();
    }

    void EmitKnifeCollisionSignal(bool hitEnemy)
    {
        KnifeCollisionInfo collisionInfo = new()
        {
            normal = -rb.linearVelocity,
            point = rb.position,
            struckEnemy = hitEnemy
        };

        if (hitEnemy) enemyStruck.Invoke(collisionInfo);
        else terrainStruck.Invoke(collisionInfo);
    }

    void EmbedKnife()
    {
        Quaternion incomingRotation = transform.rotation;
        Vector3 travelDirection = rb.linearVelocity.normalized;

        transform.position += travelDirection * (embedDepth * 0.1f);

        // Restore the original rotation to maintain impact angle
        transform.rotation = incomingRotation;

        SetState(ProjectileState.Embedded);

    }
    void OnCollisionEnter(Collision collision) 
    {
        if (currentState != ProjectileState.Flying || currentState == ProjectileState.Recalling) return;
        OnObjectStruck();
    }
}


public struct KnifeCollisionInfo
{
    public Vector3 normal;
    public Vector3 point;
    public bool struckEnemy;
}

public struct KnifeThrowInfo
{
    public bool parried;
    public Vector3 direction;
}

public struct KnifeRetrievalInfo
{
    public KnifeRetrievalType pickupType;
    public float blinkDistance;
}
public enum KnifeRetrievalType
{
    Recall,
    Pickup,
    Blink
}

