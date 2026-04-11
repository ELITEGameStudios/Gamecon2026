using UnityEngine;

[CreateAssetMenu(fileName = "GenericWave", menuName = "Wave System/Wave Types/Generic")]
public class GenericWave : IntervalBasedSequentialWave
{
    [SerializeField] int maxEnemyCount = 1;

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
