using UnityEngine;
using UnityEngine.InputSystem;

public class KnifeShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileFirePoint;

    [Header("Settings")]
    [SerializeField] private float fireRate = 2f; // temporary

    private bool coolingDown {get {return nextFire > 0;}}
    private float nextFire = 0f;
    public InputSystem_Actions inputActions;
    public InputActionReference fire;

    void Update()
    {
        if(coolingDown) nextFire -= Time.deltaTime;
    }

    void OnEnable()
    {
        fire.action.started += ProcessFireInput;
    }

    void ProcessFireInput(InputAction.CallbackContext ctx)
    {
        if (!coolingDown){
            Fire();
            nextFire = Time.time + 1f / fireRate;
        }
        
    }

    void Fire()
    {
        if (projectilePrefab == null || projectileFirePoint == null)
        {
            Debug.LogError("Missing prefab or fire point on KnifeShooting.");
            return;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("No MainCamera found. Tag your camera as 'MainCamera'.");
            return;
        }

        // Fire where the camera is aiming
        Vector3 fireDirection = cam.transform.forward;

        // Spawn the projectile facing that direction
        Instantiate(
            projectilePrefab,
            projectileFirePoint.position,
            Quaternion.LookRotation(fireDirection)
        );

        Debug.Log("BANG " + fireDirection);
    }
}