using System;
using UnityEngine;
using UnityEngine.InputSystem;

// handles shooting, recalling, and parrying
public class ProjectileAbilities : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Projectile featherKnife;
    [SerializeField] private Transform projectileFirePoint;

    [Header("Settings")]
    [SerializeField] private float fireRate = 2f; // temporary
    private float fireCooldown;
    
    [Header("Input")]
    public InputActionReference fireAction;

    void OnEnable()
    {
        fireAction.action.started += OnFirePressed;
    }

    void OnDisable()
    {
        fireAction.action.started -= OnFirePressed;
    }

    void Update()
    {
        if (fireCooldown > 0) 
            fireCooldown -= Time.deltaTime;
    }

    private void OnFirePressed(InputAction.CallbackContext ctx)
    {
        TryShoot();
    }

    private void TryShoot()
    {
        if (fireCooldown > 0)
            return;
        
        if (featherKnife.currentState != Projectile.ProjectileState.Idle)
            return;
        
        Camera cam = Camera.main;
        if (cam == null) 
            return;

        Vector3 direction = cam.transform.forward;
        
        featherKnife.CastProjectile(direction);
        
        fireCooldown = 1f / fireRate;
    }
}