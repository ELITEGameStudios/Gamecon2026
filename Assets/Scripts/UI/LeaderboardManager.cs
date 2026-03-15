
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    public const float FAILURE_LEVEL_TIME = -1.0f;

    [SerializeField] GameObject leaderboardDisplay;
    [SerializeField] GameObject entriesHolder;
    [SerializeField] LeaderboardEntry entryPrefab;
    [SerializeField] TMP_InputField attemptNamer;
    [SerializeField] Button saveAttemptButton;
 
    LeaderboardService leaderboardService;
    LevelDatabase.LevelNames currentLevel;
    SaveSystem saveSystem;

    float completionTime = 0;

    List<LevelAttempt> levelCompletions = new();

    private void Awake()
    {
        if (leaderboardDisplay != null)  leaderboardDisplay.SetActive(false);
        saveSystem = new ();
        leaderboardService = new();
    }
    public void InitManager(LevelDatabase.LevelNames currentLevel)
    {
        Debug.Log("Starting leaderboard manager init");
        this.currentLevel = currentLevel;
        ClearExistingEntryGameObjects();//clear placeholders

        string[] saveDirectory = saveSystem.GetDirectory(LeaderboardService.GetDataFolderPath(currentLevel));


        Debug.Log("Looking at path " + LeaderboardService.GetDataFolderPath(currentLevel));
        if (saveDirectory == null)
        {
            Debug.Log("No directory found, leaving early");
            return;
        }
        levelCompletions.Clear();
        foreach (var save in saveDirectory)
        {
            var saveData = saveSystem.Read(save);
            levelCompletions.Add(saveSystem.ParseFromJson<LevelAttempt>(saveData));
        }
        Debug.Log("Loaded previous attempts");
        if (levelCompletions.Count == 0) return;
        levelCompletions = leaderboardService.SortAttemptsByTime(levelCompletions);
        foreach (var data in levelCompletions)
        {
            LeaderboardEntry newEntry = Instantiate(entryPrefab, entriesHolder.transform);
            newEntry.InitEntry(data.name, leaderboardService.GetFormattedTime(data.time));
        }
        Debug.Log("Finished leaderboard manager init");
    }

    void ClearExistingEntryGameObjects()
    {
        foreach (Transform entry in entriesHolder.transform)
        {
            Destroy(entry.gameObject);
        }
    }

    public void OnLevelOver(float time)
    {
        completionTime = time;
        leaderboardDisplay.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        saveAttemptButton.interactable = (time != FAILURE_LEVEL_TIME);
        attemptNamer.interactable = (time != FAILURE_LEVEL_TIME);
    }
    public void SaveNewAttempt()
    {
        string attemptName = attemptNamer.text;
        if (!leaderboardService.IsNameAllowed(attemptName, levelCompletions) || completionTime <= 0)
        {
            Debug.LogWarning("Invalid attempt.");
            return;
        }
        saveAttemptButton.interactable = false;
        LevelAttempt newEntry = new(attemptName, completionTime);
        saveSystem.EnsureSave(LeaderboardService.GetDataFolderPath(currentLevel), attemptName, newEntry);
        InitManager(currentLevel);
    }

    public void CloseLeaderboard()
    {
        leaderboardDisplay.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
}