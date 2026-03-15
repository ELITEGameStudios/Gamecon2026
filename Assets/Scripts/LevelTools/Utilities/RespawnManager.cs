using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RespawnManager : MonoBehaviour
{
    UnityEvent playerRespawned = new();

    [SerializeField] Transform respawnPoint;
    [SerializeField] Transform checkpointHolder;
    [SerializeField] Player player;

    [SerializeField] GameObject deathNotifier;
    bool playerDead = false;

    public InputActionReference respawn;

    public void InitManager(IVictoryCondition victoryCondition, IEntityManager entityManager)
    {
        if (player == null) player = FindFirstObjectByType<Player>();
        player.entityKilled.AddListener(OnPlayerKilled);
        deathNotifier.SetActive(false);
        respawnPoint.position = Player.instance.transform.position;

        var checkpoints = checkpointHolder.GetComponentsInChildren<PlayerCheckpoint>();
        foreach (PlayerCheckpoint checkpoint in checkpoints)
        {
            checkpoint.checkpointReached.AddListener(OnCheckpointReached);
        }
        if (entityManager is WaveManager waveManager)
        {
           // ConfigureWavePausingOnDeath(waveManager);
        }

        Debug.Log("init respawn system");
    }

    void ConfigureWavePausingOnDeath(WaveManager waveManager)
    {
        playerRespawned.AddListener(waveManager.RestartWave);
    }
    protected void OnPlayerKilled(EntityBase player)
    {
        Debug.Log("Player died");
        deathNotifier.SetActive(true);
        playerDead = true;
        respawn.action.performed += OnRespawnRequest;
    }

    void OnRespawnRequest(InputAction.CallbackContext ctx)
    {
        if (!playerDead) return;
        respawn.action.performed -= OnRespawnRequest;
        Player.instance.SpawnAtPosition(respawnPoint.position);
        deathNotifier.SetActive(false);
        playerDead = false;
    }

    public bool IsPlayerDead()
    {
        return playerDead;
    }

    protected void OnCheckpointReached(PlayerCheckpoint checkpoint)
    {
        respawnPoint.position = checkpoint.transform.position;
    }
}

    
