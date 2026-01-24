using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

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
   public List<WaveData> levelWaves = new();
}
