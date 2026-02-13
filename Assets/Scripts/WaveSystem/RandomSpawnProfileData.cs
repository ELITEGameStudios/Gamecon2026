using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Random SpawnProfile", menuName = "Wave System/Spawn Profiles/Random Spawnpoint")]
public class RandomSpawnProfileData : SpawnProfileData
{
    public int spawnCount = 1;
    public bool trueRandomizedEnemies = false;
    
    public override SpawnProfile NewProfile()
    {
        return new RandomSpawnProfile(this);
    }

}