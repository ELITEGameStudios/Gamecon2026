using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GateData", menuName = "Scriptable Objects/GateData")]
public class GateData : ScriptableObject
{
    public List<EnemyBase> enemiesInGate = new();
    public float timeBeforeNextGateTriggersAfterPreviousCleared = 0.0f; //spawn next gate of enemies immediately by default
    public bool requirePreviousGateCleared = false;
    public Vector3 spawnPos;
}
