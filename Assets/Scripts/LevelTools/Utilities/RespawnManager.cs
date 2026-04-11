using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] Transform respawnPoint;
    [SerializeField] Transform checkpointHolder;

    [SerializeField] GameObject deathNotifier;
    bool playerDead = false;

    public InputActionReference respawn;
    private void Start()
    {
        StartCoroutine(InitializeRespawnManager());
    }
    IEnumerator InitializeRespawnManager()
    {
        deathNotifier.SetActive(false);
        yield return new WaitUntil(() => Player.instance != null);
        Player.instance.entityKilled.AddListener((runner) => OnPlayerKilled());
        respawnPoint.position = Player.instance.transform.position;

        var checkpoints = checkpointHolder.GetComponentsInChildren<PlayerCheckpoint>();
        foreach (PlayerCheckpoint checkpoint in checkpoints)
        {
            checkpoint.checkpointReached.AddListener(OnCheckpointReached);
        }
    }


    protected void OnPlayerKilled()
    {
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
        Debug.Log("Respawned player at " + respawnPoint.position);
    }

    public bool IsPlayerDead()
    {
        return playerDead;
    }

    protected void OnCheckpointReached(PlayerCheckpoint checkpoint)
    {
        respawnPoint.position = checkpoint.transform.position;
    }

    private void Update()
    {

    }
}

    
