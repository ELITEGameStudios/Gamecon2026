using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// handles shooting, recalling, and parrying
public class ProjectileAbilities : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Projectile featherKnife;
    [SerializeField] private Transform projectileFirePoint;
    [SerializeField] private PlayerMovementStateMachine playerMovement;
    [SerializeField] private Camera cam => Camera.main;
    public ApplyShake camShaker;

    [Header("Settings")]
    [SerializeField] private float fireRate = 2f; // temporary
    [SerializeField] private float parryTiming = 0.2f;
    [SerializeField] private float recallCooldown, currentRecallCooldown;
    [SerializeField] private float parryForce = 500f;
    [SerializeField] private float hitStopTime = 0.1f;
    private float fireCooldown;
    public ApplyShake.CamShakeProfile parryCamShakeProfile;
    
    [Header("Input")]
    public InputActionReference fireAction;
    public InputActionReference recallAction;

    void OnEnable()
    {
        fireAction.action.started += OnFirePressed;
        recallAction.action.performed += OnRecallPressed;
    }

    void OnDisable()
    {
        fireAction.action.started -= OnFirePressed;
        recallAction.action.performed -= OnRecallPressed;
    }

    void Update()
    {
        if (fireCooldown > 0) 
            fireCooldown -= Time.deltaTime;
        
        if (currentRecallCooldown > 0) 
            currentRecallCooldown -= Time.deltaTime;
    }

    private void OnFirePressed(InputAction.CallbackContext ctx)
    {
        if(featherKnife.currentState == Projectile.ProjectileState.Recalling){
            if(featherKnife.recallCurrentTime < parryTiming)
            {
                Debug.Log("Parried!");
                TryShoot(true);
            }
        }
        TryShoot();
    }

    private void OnRecallPressed(InputAction.CallbackContext ctx)
    {
        if(currentRecallCooldown > 0 || featherKnife.currentState == Projectile.ProjectileState.Idle){return;}
        else{
            if(featherKnife.currentState == Projectile.ProjectileState.Flying) { featherKnife.SetEmbeddedPos(); }
            featherKnife.SetState(Projectile.ProjectileState.Recalling);
            currentRecallCooldown = recallCooldown;
        }
    }

    private void TryShoot(bool parry = false)
    {
        if (fireCooldown > 0)
            return;
        
        if (featherKnife.currentState != Projectile.ProjectileState.Idle && !parry)
            return;
        
        if (cam == null) 
            return;
        
        Shoot(parry);
        if (parry){ StartCoroutine(ParryCoroutine()); }
    }

    private void Shoot(bool parry)
    {
        Vector3 spawnPos;

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

        fireCooldown = 1f / fireRate;
    
        // Start hitstop immediately after spawning projectile
        if (parry)
        {
            StartCoroutine(ParryCoroutine());
        }
    }
    
    private IEnumerator ParryCoroutine()
    {
        if(hitStopTime > 0)
        {
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(hitStopTime);
            Time.timeScale = 1f;
        }

        playerMovement.rigidbody.linearVelocity = 
            ( transform.up - transform.forward  ).normalized * parryForce;

        camShaker.StartShake(parryCamShakeProfile);
    }
}