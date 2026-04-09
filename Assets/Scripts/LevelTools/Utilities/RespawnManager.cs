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
    static public bool PlayerDead { private set; get; } = false;

    public InputActionReference respawn;

    bool gameOver = false;

    public void InitManager(IVictoryCondition victoryCondition, IEntityManager entityManager)
    {
        if (player == null) player = FindFirstObjectByType<Player>();
        player.entityKilled.AddListener(OnPlayerKilled);
        victoryCondition.victoryAchieved += OnGameOver;
        deathNotifier.SetActive(false);
        respawnPoint.position = player.transform.position;

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

    void OnGameOver()
    {
        gameOver = true;
    }

    void ConfigureWavePausingOnDeath(WaveManager waveManager)
    {
        playerRespawned.AddListener(waveManager.RestartWave);
    }
    protected void OnPlayerKilled(EntityBase player)
    {
        if (gameOver) return;
        deathNotifier.SetActive(true);
        PlayerDead = true;
        respawn.action.performed += OnRespawnRequest;

        Time.timeScale = 0.0f;
    }

    void OnRespawnRequest(InputAction.CallbackContext ctx)
    {
        if (!PlayerDead) return;
        respawn.action.performed -= OnRespawnRequest;
        var respawnPoint = GetRespawnLocation(PlayerMovementStateMachine.instance.lastGroundedPos);
        Player.instance.SpawnAtPosition(respawnPoint.position);
        player.transform.rotation = respawnPoint.rotation;

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
        if(respawnPoints.Length == 0){return respawnPoint;}
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

    
