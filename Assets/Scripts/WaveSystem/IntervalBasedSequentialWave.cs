using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "IntervalWave", menuName = "Wave System/Wave Types/Sequential Interval")]
public class IntervalBasedSequentialWave : SequentialWave
{
    public float minTimeBetweenSpawns, time;
    public bool minTimeCondition => time <= 0;
    
    public override void WaveUpdate()
    {
        if(!minTimeCondition){
            time -= Time.deltaTime;
        }
        else
        {
            if(typeof(IntervalBasedSequentialWave) == GetType()) // Hopefully this limits this functionality to only if the class itself is in fact only this.
            {
                TryInitiateNextSpawn();
            }
        }         
    }
    public override void OnBeginSpawnProcess()
    {
        time = minTimeBetweenSpawns;
        Debug.Log("TIMER SET AT " + time);
    }
}