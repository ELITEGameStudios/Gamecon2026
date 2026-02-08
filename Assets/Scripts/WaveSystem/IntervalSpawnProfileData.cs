using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Intervaled SpawnProfile", menuName = "Wave System/Spawn Profiles/Intervaled")]
public class IntervalSpawnProfileData : SpawnProfileData
{
    public float time;
    public bool timeRepresentsInterval = false;
    
    public override SpawnProfile NewProfile()
    {
        return new IntervalSpawnProfile(this);
    }

}