using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    [SerializeField] TMP_Text timerDisplay;

    [SerializeField, Range(0.01f, 1.0f)] float spaceBetweenCharacters = 0.7f;

    float timerTracker = 0.0f;
    bool runTimer = false;
    TimeSpan timerTimespan;

    bool playerSpawned = false;
    private void Start()
    {
        runTimer = true;
        timerTracker = 0.0f;
        StartCoroutine(InitializeManager());
    }
    IEnumerator InitializeManager()
    {
        yield return new WaitUntil(() => Player.instance != null);
        Player.instance.entityKilled.AddListener((runner) => OnPlayerKilled());
        Player.instance.entitySpawned.AddListener((runner) => OnPlayerRespawned());
    }

    private void Update()
    {
        if (!runTimer) return;
        timerTracker += Time.deltaTime;
        timerTimespan = TimeSpan.FromSeconds(timerTracker);
        timerDisplay.text = $"<mspace={spaceBetweenCharacters}em >{timerTimespan.ToString("mm\\:ss")}</mspace>";
    }

    void OnPlayerKilled()
    {
        StartCoroutine(HandlePlayerDeath());
    }

    void OnPlayerRespawned()
    {
        playerSpawned = true;
    }

    IEnumerator HandlePlayerDeath()
    {
        runTimer = false;
        yield return new WaitUntil(() => playerSpawned);
        playerSpawned = false;
        runTimer = true;
    }

    public float GetCurrentLevelTime()
    {
        return timerTracker;
    }
}
