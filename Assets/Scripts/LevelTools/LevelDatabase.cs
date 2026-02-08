using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Scriptable Objects/LevelDatabase")]
public class LevelDatabase : ScriptableObject
{
    [SerializeField] List<LevelData> gameLevels = new();

    [System.Serializable]
    public enum LevelNames
    {
        TutorialLevel,
        TestLevel,
        DevRoom_Temi,
        MapSystemTestLevel,
    }


}
