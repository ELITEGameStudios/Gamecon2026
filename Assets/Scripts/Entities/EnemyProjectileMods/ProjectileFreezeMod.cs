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
    [SerializeField] bool fireOnUnfreeze = true;

    bool FreezesWhilePlayerDead() => freezeType == FreezeType.FreezeWhenPlayerDead;

    ProjectileContactModifier contactModifier;

    int duration = 0;

    RespawnManager respawnManager;

    float previousVelocity;

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
                    Player.instance.entityKilled.AddListener(OnPlayerStruck);
                }
                else
                {
                    Debug.LogWarning("Could not find contact mod but using on player struck which has a dependacy on it");
                }
                respawnManager = FindFirstObjectByType<RespawnManager>();
                break;
                
        }
        duration = 0;
    }

    void OnPlayerStruck(EntityBase player)
    {
        if (player == null || freezeType != FreezeType.FreezeWhenPlayerDead) return;
        playerDead = true;
        previousVelocity = projectile.projectileVelocity.Speed;
        duration = freezeDurationWhenPlayerDies;
        if (respawnManager != null)
        {
            respawnManager.respawn.action.performed += OnPlayerRespawned;
        }
        Debug.Log("Player struck, freezing projectile for " + duration + " frames");
    }

    public override void UpdateModifier()
    {
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
        if (!playerDead)
        {
            duration--;
        }
        if (duration <= 0)
        {
            if (!fireOnUnfreeze) projectile.projectileVelocity.Speed = previousVelocity;
            else projectile.Activate(projectile.enemy, projectile.rb.position);
        }
        else projectile.projectileVelocity.Speed = 0.0f;
    }

    void OnPlayerRespawned(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        playerDead = false;
        respawnManager.respawn.action.performed -= OnPlayerRespawned;
    }
}
