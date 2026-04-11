using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnProfileData", menuName = "Wave System/Spawn Profiles/Generic")]
public class SpawnProfileData : ScriptableObject
{
    public EntityBase[] spawnList;
    public Transform positionsParentPrefab;

    public virtual SpawnProfile NewProfile()
    {
        return new SpawnProfile(this);
    }

}