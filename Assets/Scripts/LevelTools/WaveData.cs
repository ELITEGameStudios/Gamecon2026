using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveObjects/WaveData")]
public class WaveData : ScriptableObject
{
    /// <summary>
    /// Batches of enemies to spawn at a time for each wave.
    /// </summary>
    public List<GateData> enemyGates = new();
    public float timeBeforeFirstGate = 10.0f;
}

