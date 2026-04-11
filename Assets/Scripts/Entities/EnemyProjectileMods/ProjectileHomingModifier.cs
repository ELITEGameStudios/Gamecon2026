using UnityEngine;

[RequireComponent (typeof(ProjectileVelocityModifier))]
public class ProjectileHomingModifier : ProjectileModifier
{

    [SerializeField] Vector3 rotationOffset;
    public float projectileHoming = 7.0f;
    public float maxRange = 100.0f;

   [HideInInspector]  public Transform target;

    /// <summary>
    /// How much homing should be added/taken away depending on proximity.
    /// </summary>

    [Range(0.0f, 10.0f)] public float minHomingProximityModifier = 1.0f;
    [Range(1.0f, 10.0f)] public float maxHomingProximityModifier = 1.0f;

    /// <summary>
    /// Makes the homing stronger the further away the target is.
    /// </summary>
    [SerializeField]  bool invertProximity;


    public override void InitModifier(EnemyProjectile projectile)
    {
        base.InitModifier(projectile);
        projectile.projectileActivated.AddListener(OnProjectileActivated);
    }
    public void OnProjectileActivated(EnemyProjectile.ProjectileTarget target)
    {
        this.target = target.targetTransform;
    }
    public void HomingLogic()
    {
        if (target == null) return;
        Rigidbody rb = projectile.rb;
        var desired = (target.transform.position - rb.position).normalized;

        float distance = Vector3.Distance(target.position, projectile.enemy.transform.position);

        float proximityModifier = CalculateProximityModfier(distance);
        projectile.projectileVelocity.Direction = Vector3.Lerp(projectile.projectileVelocity.Direction, desired, projectileHoming * proximityModifier * Time.fixedDeltaTime);
        projectile.meshObjects.transform.rotation = Quaternion.LookRotation(projectile.projectileVelocity.Direction) * Quaternion.Euler(rotationOffset);
    }

    float CalculateProximityModfier(float distance)
    {
        float proximityModifier = 0.0f;
        if (distance > maxRange) // if target is out of range just use the min or max modifier depending on if we are inverting or not, this is cheaper
        {
            proximityModifier = invertProximity ? minHomingProximityModifier : maxHomingProximityModifier;
        }
        else
        {
            float distanceAsPercentage = Mathf.Clamp01(distance / maxRange);
            if (invertProximity)
            {
                proximityModifier = Mathf.Lerp(minHomingProximityModifier, maxHomingProximityModifier, distanceAsPercentage);
            }
            else
            {
                proximityModifier = Mathf.Lerp(maxHomingProximityModifier, minHomingProximityModifier, distanceAsPercentage);
            }
        }
        return proximityModifier;
    }

    public override void UpdateModifier()
    {
        if (projectile.Active)
        {
            HomingLogic();
        }
    }


}