using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RespawnManager : MonoBehaviour
{
    UnityEvent playerRespawned = new();

    [SerializeField] Transform respawnPoint;
    [SerializeField] Transform[] respawnPoints;
    [SerializeField] Transform checkpointHolder;
    [SerializeField] Player player;
    [SerializeField] Projectile knife;

    [SerializeField] GameObject deathNotifier;
    public static bool PlayerDead { private set; get; } = false;

    public InputActionReference respawn;

    public void InitManager(IVictoryCondition victoryCondition, IEntityManager entityManager)
    {
        if (player == null) player = FindFirstObjectByType<Player>();
        player.entityKilled.AddListener(OnPlayerKilled);
        deathNotifier.SetActive(false);
        respawnPoint.position = player.transform.position;
        PlayerDead = false;

        var checkpoints = checkpointHolder.GetComponentsInChildren<PlayerCheckpoint>();
        foreach (PlayerCheckpoint checkpoint in checkpoints)
        {
            checkpoint.checkpointReached.AddListener(OnCheckpointReached);
        }
        if (entityManager is WaveManager waveManager)
        {
           ConfigureWavePausingOnDeath(waveManager);
        }
    }

    void ConfigureWavePausingOnDeath(WaveManager waveManager)
    {
        playerRespawned.AddListener(waveManager.RestartWave);
    }
    protected void OnPlayerKilled(EntityBase player)
    {
        deathNotifier.SetActive(true);
        PlayerDead = true;
        respawn.action.performed += OnRespawnRequest;
        Time.timeScale = 0.0f;
    }
    void OnRespawnRequest(InputAction.CallbackContext ctx)
    {
        if (!PlayerDead || SettingsMenu.Paused) return;
        respawn.action.performed -= OnRespawnRequest;
        var respawnPoint = GetRespawnLocation(PlayerMovementStateMachine.instance.lastGroundedPos);
        Player.instance.SpawnAtPosition(respawnPoint.position);
        var rotateTowardsRespawn = Quaternion.LookRotation (respawnPoint.transform.forward).eulerAngles;
        player.transform.eulerAngles = rotateTowardsRespawn;

        deathNotifier.SetActive(false);
        PlayerDead = false;

        if (knife != null)
        {
            knife.Pickup();
        }

        Time.timeScale = 1.0f;
    }

    public bool IsPlayerDead()
    {
        return PlayerDead;
    }

    protected void OnCheckpointReached(PlayerCheckpoint checkpoint)
    {
        respawnPoint.position = checkpoint.transform.position;
    }

    private Transform GetRespawnLocation(Vector3 worldPos)
    {
        if (respawnPoints == null) return respawnPoint;
        if (respawnPoints.Length == 0){return respawnPoint;}
        int winningIndex = 0;
        float winningDist = Vector3.Distance(worldPos, respawnPoints[0].position);
        for (int i = 1; i < respawnPoints.Length; i++)
        {
            float dist = Vector3.Distance(worldPos,respawnPoints[i].position);
            if(dist < winningDist)
            {
                winningIndex = i;
                winningDist = dist;
            }
        }

        return respawnPoints[winningIndex];
    }
}

    
