using UnityEngine;

public class TrackerService 
{
    public static string GetDataFolderPathRoot()
    {
        return Application.persistentDataPath + "/TrackedData/";
    }
    public static string GetDataFolderPathForLevel(LevelDatabase.LevelNames currentLevel)
    {
        return GetDataFolderPathRoot() + currentLevel.ToString();
    }
}