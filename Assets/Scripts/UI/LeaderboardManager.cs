using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
        leaderboardDisplay.SetActive(false);
        saveSystem = new ();
        leaderboardService = new();
    }
    public void InitManager(LevelDatabase.LevelNames currentLevel)
    {
        this.currentLevel = currentLevel;
        ClearExistingEntryGameObjects();//clear placeholders

        var saveDirectory = saveSystem.GetDirectory(LeaderboardService.GetDataFolderPath(currentLevel));
        levelCompletions.Clear();
        foreach (var save in saveDirectory)
        {
            var saveData = saveSystem.Read(save);
            Debug.Log("Save data == " + saveData);
            levelCompletions.Add(saveSystem.ParseFromJson<LevelAttempt>(saveData));
        }
        if (levelCompletions.Count == 0) return;
        levelCompletions = leaderboardService.SortAttemptsByTime(levelCompletions);
        foreach (var data in levelCompletions)
        {
            string formattedTime = leaderboardService.GetFormattedTime(data.time);
            LeaderboardEntry newEntry = Instantiate(entryPrefab, entriesHolder.transform);
            newEntry.InitEntry(data.name, formattedTime);
        }
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