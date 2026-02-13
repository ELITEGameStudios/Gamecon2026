using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenericWave", menuName = "Wave System/Wave Types/Generic")]
public class GenericWave : IntervalBasedSequentialWave
{
    public int maxEnemyCount = 1;

    public override void WaveUpdate()
    {
        Debug.Log("AAA");
        base.WaveUpdate();
        if(WaveManager.Instance.enemiesInWaveRemaining.Count < maxEnemyCount && minTimeCondition)
        {
            TryInitiateNextSpawn();
        }
    }
}
