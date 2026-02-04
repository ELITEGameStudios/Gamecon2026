using System;
using System.Collections.Generic;
using System.Linq;
public class LeaderboardService
{
    char[] unallowedChars = 
    {
        '<',
        '>',
        ':',
        '"',
        '/',
        '\\',
        '|',
        '?',
        '*',
    };
    HashSet<string> unallowedNames = new()
    {
        "CON",
        "PRN",
        "AUX",
        "NUL",
        "COM1", "COM2","COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM0",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9", "LPT0"
    };
    public List<LevelAttempt> SortAttemptsByTime(List<LevelAttempt> data)
    {
        return data.OrderBy(entry => entry.time).ToList();
    }
    public string GetFormattedTime(float time)
    {
        var timespan = TimeSpan.FromSeconds(time);
        return timespan.ToString("mm\\:ss\\:fff");
    }
    public bool IsNameAllowed(string attemptName, List<LevelAttempt> attempts)
    {
        if (attemptName.Length == 0) return false;

        foreach (var unallowedChar in unallowedChars)
        {
            if (attemptName.Contains(unallowedChar))
            {
                return false;
            }
        }
        if (unallowedNames.Equals(attemptName)) return false;
        foreach (var attempt in attempts)
        {
            if (attempt.name.Equals(attemptName))
            {
                return false;
            }
        }
        //Will remove offensive attempt names later
        return true;
    }

    public static string GetDataFolderPath(LevelDatabase.LevelNames currentLevel)
    {
        return UnityEngine.Application.persistentDataPath + "/playerSaves/" + currentLevel.ToString();
    }
}

