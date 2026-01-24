using System;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardService
{
    public List<LevelAttempt> SortAttemptsByTime(List<LevelAttempt> data)
    {
        return data.OrderBy(entry => entry.time).ToList();
    }
    public string GetFormattedTime(float time)
    {
        var timespan = TimeSpan.FromSeconds(time);
        return timespan.ToString("mm\\:ss\\ff");
    }
    public bool IsNameAllowed(string attemptName, List<LevelAttempt> attempts)
    {
        if (attemptName.Length == 0) return false;
        foreach (var attempt in attempts)
        {
            if (attempt.name == attemptName)
            {
                return false;
            }
        }
        //Will remove offensive attempt names later
        return true;
    }

}
