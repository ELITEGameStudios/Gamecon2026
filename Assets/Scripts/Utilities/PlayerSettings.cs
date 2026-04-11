using UnityEngine;

public class PlayerSettings 
{
    //Video
    public Vector2Int resolution = new Vector2Int(1920, 1080);
    public bool fullscreen = true;

    //Audio
    public float bgmVolume = 1.0f;
    public float sfxVolume = 1.0f;

    //Input
    public float horizontalSensitivity = 0.5f;
    public float verticalSensitivity = 0.5f;

    public bool IsEqual(PlayerSettings other)
    {
        var currentAsJson = JsonUtility.ToJson(this);
        var otherAsJson = JsonUtility.ToJson(other);
        return currentAsJson.Equals(otherAsJson);
    }

    public void Copy(PlayerSettings other)
    {
        this.resolution = other.resolution;
        this.fullscreen = other.fullscreen;
        this.bgmVolume = other.bgmVolume;
        this.sfxVolume = other.sfxVolume;
        this.horizontalSensitivity = other.horizontalSensitivity;
        this.verticalSensitivity = other.verticalSensitivity;
    }

    public static string GetPlayerSettingsDirectory()
    {
        return Application.persistentDataPath + "/playerSettings";
    }

    public static string GetPlayerSettingsFilePath()
    {
        return GetPlayerSettingsDirectory() + "/playerSettings.json";
    }
}
