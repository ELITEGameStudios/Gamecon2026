using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// This modifier ensures that the projectile's distance from its target. When max distance is exceeded, it'll either speed up or warp back in range.
/// </summary>
[RequireComponent(typeof(ProjectileVelocityModifier))]
public class ProjectileClampModifier : ProjectileModifier
{
    [HideInInspector] public UnityEvent teleportPerformed = new UnityEvent();

    public enum ClampMode
    {
        SpeedIncrease,
        WarpBack
    }
    /// <summary>
    /// The maximum distance the projectile can be from its target before it gets clamped back.
    /// </summary>
    [SerializeField] float maxDistanceBeforeClampAttempt = 10f;
    /// <summary>
    /// The maximum distance the projectile can be from its target before it gets forced back into range, even if the clamp mode isn't a warp type.
    /// Used to determine functional speed for speed increase as well.
    /// <remarks>
    /// Functional speed is calculated by interpolating between min and max speed increase based on the distance as percent between the current target distance and this max distance.
    /// </remarks>
    /// </summary>
    [SerializeField] float maxDistanceToTryClamp = 9999f;
    /// <summary>
    /// Time in frames before the projectile warps back to the target after exceeding max distance.
    /// </summary>
    [SerializeField, ShowIf(nameof(RequiresWarpAttributes))] int timeUntilWarp = 60;
    [Range(1, 5), SerializeField, HideIf(nameof(RequiresWarpAttributes))] float minSpeedIncreaseIfMaxDistanceExceeded = 5f;
    [Range(1, 5), SerializeField, HideIf(nameof(RequiresWarpAttributes))] float maxSpeedIncreaseIfMaxDistanceExceeded = 5f;


   [SerializeField] ClampMode clampType = ClampMode.WarpBack;

    Transform target;

    bool RequiresWarpAttributes() => clampType == ClampMode.WarpBack;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    ProjectileVelocityModifier velocityModifier;

    float warpTracker = 0.0f;

    void Start()
    {
        if (minSpeedIncreaseIfMaxDistanceExceeded > maxSpeedIncreaseIfMaxDistanceExceeded)
        {
            (minSpeedIncreaseIfMaxDistanceExceeded, maxSpeedIncreaseIfMaxDistanceExceeded) = (maxSpeedIncreaseIfMaxDistanceExceeded, minSpeedIncreaseIfMaxDistanceExceeded);
        }
    }

    public override void InitModifier(EnemyProjectile projectile)
    {
        base.InitModifier(projectile);
        projectile.projectileActivated.AddListener(OnProjectileActivated);
        velocityModifier = projectile.GetProjectileModifier<ProjectileVelocityModifier>();
    }

    void OnProjectileActivated(EnemyProjectile.ProjectileTarget target)
    {
       this. target = target.targetTransform;
    }
    public override void UpdateModifier()
    {
        base.UpdateModifier();
        if (target == null) return;

        float distanceToTarget = Vector2.Distance(projectile.transform.position, target.position);
        if (distanceToTarget > maxDistanceBeforeClampAttempt)
        {
            switch (clampType)
            {
                case ClampMode.SpeedIncrease:
                    float distanceAsPercent = Mathf.Lerp(0, 1, distanceToTarget / maxDistanceToTryClamp);
                    float speedIncrease = Mathf.Lerp(minSpeedIncreaseIfMaxDistanceExceeded, maxSpeedIncreaseIfMaxDistanceExceeded, distanceAsPercent);
                    projectile.projectileVelocity.Speed = velocityModifier.projectileSpeed * speedIncrease;
                    break;
                case ClampMode.WarpBack:
                    if (warpTracker != 0) warpTracker = timeUntilWarp;
                    else
                    {
                        warpTracker--;
                        if (warpTracker <= 0)
                        {
                            WarpCloserToTarget();
                            distanceToTarget = maxDistanceBeforeClampAttempt - 1; // Set distance to just under the clamp attempt distance to prevent immediate re-warping or speed increase on the next frame.
                        }
                    }
                        break;

            }
            if (distanceToTarget > maxDistanceToTryClamp)
            {
                WarpCloserToTarget();
            }


        }
    }

    void WarpCloserToTarget()
    {
        Vector3 targetDirectionToProjectile = (projectile.transform.position - target.position).normalized;
        projectile.rb.MovePosition(target.position + targetDirectionToProjectile * maxDistanceBeforeClampAttempt);
        warpTracker = 0;
        teleportPerformed.Invoke();
    }
}
