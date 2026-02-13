using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(ProjectileVelocityModifier))]
public class ProjectileFreezeMod : ProjectileModifier
{
    [System.Serializable]
    enum FreezeType
    {
        FreezeOnContact,
        FreezeWhenPlayerDead,
        FreezeAfterTime
    }

    /// <summary>
    /// Time in frames for projectile to freeze after player killed
    /// </summary>
    [SerializeField, ShowIf(nameof(FreezesWhilePlayerDead))] int freezeDurationWhenPlayerDies = 300;

   [SerializeField] FreezeType freezeType;

    bool FreezesWhilePlayerDead() => freezeType == FreezeType.FreezeWhenPlayerDead;

    ProjectileContactModifier contactModifier;

    int duration = 0;

    RespawnManager respawnManager;

    Vector3 previousVelocity;

    bool playerDead = false;
    public override void InitModifier(EnemyProjectile projectile)
    {
        base.InitModifier(projectile);
        switch (freezeType)
        {
            case FreezeType.FreezeWhenPlayerDead:
                contactModifier = projectile.GetComponent<ProjectileContactModifier>();
                if (contactModifier != null)
                {
                    contactModifier.contactEvent.AddListener(OnPlayerStruck);
                }
                else
                {
                    Debug.LogWarning("Could not find contact mod but using on player struck which has a dependacy on it");
                }
                    respawnManager = FindFirstObjectByType<RespawnManager>();
                break;
                
        }
    }

    void OnPlayerStruck(EnemyProjectile projectile, Player player)
    {
        if (player == null || freezeType != FreezeType.FreezeWhenPlayerDead) return;
        playerDead = true;
        previousVelocity = projectile.projectileSpeed;
        duration = freezeDurationWhenPlayerDies;
        Debug.Log("Killed player " + player + " with projectile " + projectile.name);
        if (respawnManager != null)
        {
            respawnManager.respawn.action.performed += OnPlayerRespawned;
        }
    }

    public override void UpdateModifier()
    {
        Debug.Log("Updating freeze mod for projectile " + projectile.name + " with duration " + duration);
        base.UpdateModifier();
        if (duration > 0)
        {
            switch (freezeType)
            {
                case FreezeType.FreezeWhenPlayerDead:
                    ManageDurationWhenPlayerDead();
                    break;
            }
        }
    }

    void ManageDurationWhenPlayerDead()
    {
        if (respawnManager == null)
        {
            Debug.LogWarning("Could not find respawn manager");
            return;
        }
        if (!playerDead) duration--;
        if (duration == 0) projectile.projectileSpeed = previousVelocity;
        else projectile.projectileSpeed = Vector3.zero;
        Debug.Log("Freezing projectile " + projectile.name + " for duration " + duration + " because player is dead");
    }

    void OnPlayerRespawned(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        playerDead = false;
        respawnManager.respawn.action.performed -= OnPlayerRespawned;
        Debug.Log("Player respawned");
    }
}
