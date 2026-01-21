using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] Transform respawnPoint;
    [SerializeField] Transform checkpointHolder;
    private void Start()
    {
        StartCoroutine(InitializeRespawnManager());
    }
    IEnumerator InitializeRespawnManager()
    {
        yield return new WaitUntil (() => Player.instance != null);
        Player.instance.entityKilled.AddListener(OnPlayerKilled);
        respawnPoint.position = Player.instance.transform.position;

        var checkpoints = checkpointHolder.GetComponentsInChildren<PlayerCheckpoint>();
        foreach (PlayerCheckpoint checkpoint in checkpoints)
        {
            checkpoint.checkpointReached.AddListener(OnCheckpointReached);
        }
    }


    protected void OnPlayerKilled(EntityBase runner)
    {
        runner.SpawnAtPosition(respawnPoint.position);
    }

    protected void OnCheckpointReached(PlayerCheckpoint checkpoint)
    {
        respawnPoint.position = checkpoint.transform.position;
    }
}
