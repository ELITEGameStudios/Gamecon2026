using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{

    [System.Serializable]
   public enum LevelType
    {
        KillTargets,
        Survive,
    }
    public LevelType levelType = LevelType.KillTargets;
//    public List<WaveData> levelWaves = new();
   public List<WaveBase> levelWaves = new();
    /// <summary>
    /// Length of time before the level is deemed failed. Setting the value to -1 makes the level infinite duration.
    /// </summary>
    [SerializeField, ShowIf(nameof(RequiresTimer))] public float levelDuration = -1.0f; // negative numbers make the level infinite duration

    bool RequiresTimer() => levelType == LevelType.KillTargets;
}
