using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] Transform respawnPoint;
    [SerializeField] Transform checkpointHolder;

    [SerializeField] GameObject deathNotifier;
    bool playerDead = true;

    [SerializeField] InputActionReference respawn;
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
        respawn.action.performed += OnPlayerKilled;
    }

    void OnPlayerKilled(InputAction.CallbackContext ctx)
    {
        respawn.action.performed -= OnPlayerKilled;
        Player.instance.SpawnAtPosition(respawnPoint.position);
        playerDead = false;
        deathNotifier.SetActive(false);
    }



    protected void OnCheckpointReached(PlayerCheckpoint checkpoint)
    {
        respawnPoint.position = checkpoint.transform.position;
    }

    private void Update()
    {

    }
}

    
