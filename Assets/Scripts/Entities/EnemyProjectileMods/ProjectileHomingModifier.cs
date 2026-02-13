using UnityEngine;

[RequireComponent (typeof(ProjectileVelocityModifier))]
public class ProjectileHomingModifier : ProjectileModifier
{
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
        var desired = (target.transform.position - rb.position).normalized * projectile.projectileSpeed.magnitude;

        float distance = Vector3.Distance(target.position, projectile.enemy.transform.position);

        float proximityModifier;
        if (distance > maxRange)
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
        projectile.projectileSpeed = Vector3.Lerp(projectile.projectileSpeed, desired, projectileHoming * proximityModifier * Time.fixedDeltaTime);
    }

    public override void UpdateModifier()
    {
        if (projectile.Active)
        {
            HomingLogic();
        }
    }


}