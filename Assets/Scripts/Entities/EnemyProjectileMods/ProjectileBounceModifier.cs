using UnityEngine;
[RequireComponent(typeof(ProjectileVelocityModifier))]
public class ProjectileBounceModifier : ProjectileModifier
{
    [SerializeField] LayerMask terrainMask;
    [SerializeField] float bounceCooldown = 1.0f;


    Vector3 previousProjectilePosition;

    float cooldownTracker = 0.0f;

    public override void UpdateModifier()
    {
        if (!projectile.Active) return;
        if (previousProjectilePosition != Vector3.zero && cooldownTracker <= 0.0f)
        {
            var terrainCheck = PerformRaycastCheck(previousProjectilePosition, projectile.rb.position);
            if (terrainCheck.collider != null)
            {
                ReflectProjectileVelocity(terrainCheck);
                cooldownTracker = bounceCooldown;
                Debug.Log("Bouncing");
            }
        }
        previousProjectilePosition = projectile.rb.position;
        if (cooldownTracker > 0.0f)
        {
            cooldownTracker -= Time.deltaTime;
            if (cooldownTracker < 0.0f) cooldownTracker = 0.0f;
        }
    }

    void ReflectProjectileVelocity(RaycastHit hit)
    {
        Vector3 reflectedSpeed = projectile.projectileVelocity.Direction;
        reflectedSpeed = Vector3.Reflect(reflectedSpeed, hit.normal);
        Debug.Log("Reflecting projectile speed from " + projectile.rb.linearVelocity + " to " + reflectedSpeed);
        projectile.projectileVelocity.Direction = reflectedSpeed;
    }
    RaycastHit PerformRaycastCheck(Vector3 previous, Vector3 current)
    {
        Vector3 travelVector = current - previous;
        float checkerDistance = travelVector.magnitude;
        if (checkerDistance < 0.001f) return new RaycastHit();

        Ray ray = new(previous, travelVector.normalized);

        var hit = Physics.Raycast(ray, out RaycastHit rayHit, checkerDistance, terrainMask);
        if (hit)
        {
            return rayHit;
        }
        return new RaycastHit();
    }
}