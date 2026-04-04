
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnProfile
{
    public EntityBase[] spawnList;
    public Transform positionsParentPrefab;
    public Transform positionsParent;
    public Transform[] spawnPositions;
    public delegate void FinishProcessDelegate(SpawnProfile finishedProfile);
    public FinishProcessDelegate OnFinishSpawnProcess;

    public SpawnProfile(SpawnProfileData data)
    {
        spawnList = data.spawnList;
        positionsParentPrefab = data.positionsParentPrefab;
    }

    public virtual void StartProcess()
    {
        Transform origin = GameManager.Instance.customLevelOrigin; 
        if(positionsParent == null){
            
            if(origin != null){ positionsParent = Transform.Instantiate(positionsParentPrefab, origin); }
            else { positionsParent = Transform.Instantiate(positionsParentPrefab, Vector3.zero, Quaternion.identity); }
        }

        List<Transform> spawnPosTempList = new();
        for (int i = 0; i < positionsParent.childCount; i++){ spawnPosTempList.Add(positionsParent.GetChild(i)); }
        spawnPositions = spawnPosTempList.ToArray();

        OnStartProcess();
    }


    public virtual void OnStartProcess()
    {
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            int spawnListIndex = (i % spawnList.Length) % spawnPositions.Length;
            EntityBase entity = WaveManager.Instance.SpawnEntity(spawnList[spawnListIndex], spawnPositions[i]);
        }
        End();
    }

    public virtual void SpawnProcessUpdate(){}

    public virtual void End(){
        OnFinishSpawnProcess.Invoke(this);
    }
}

public class IntervalSpawnProfile : SpawnProfile
{
    public float interval, totalTime, timer;
    int currentIndex;

    public IntervalSpawnProfile(IntervalSpawnProfileData data) : base(data)
    {
        if (data.timeRepresentsInterval)
        {
            interval = data.time;
            totalTime = interval * (data.positionsParentPrefab.transform.childCount-1);
        }
        else
        {
            totalTime = data.time;
            interval = totalTime / (data.positionsParentPrefab.transform.childCount-1);
        }
        Debug.Log("New spanwer, interval: "+ interval + " | Time: "+totalTime);
    }

    public override void OnStartProcess()
    {
        currentIndex = 0;
        SpawnOperation();
    }

    void SpawnOperation()
    {
        int spawnListIndex = (currentIndex % spawnList.Length) % spawnPositions.Length;
        EntityBase entity = WaveManager.Instance.SpawnEntity(spawnList[spawnListIndex], spawnPositions[currentIndex]);
        currentIndex++;
        timer = interval;

        if(currentIndex == spawnPositions.Length){End();}
    }

    public override void SpawnProcessUpdate()
    {
        if(timer <= 0){
            SpawnOperation();
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
}

public class RandomSpawnProfile : SpawnProfile
{
    public int spawnCount;
    public bool trueRandomizedEnemies;
    List<Transform> unusedSpawns;

    public RandomSpawnProfile(RandomSpawnProfileData data) : base(data)
    {
        spawnCount = data.spawnCount;
        
        if(spawnCount > data.positionsParentPrefab.transform.childCount){
            spawnCount = data.positionsParentPrefab.transform.childCount;
        }

        trueRandomizedEnemies = data.trueRandomizedEnemies;
    }

    public override void OnStartProcess()
    {
        unusedSpawns = spawnPositions.ToList();
        
        for (int i = 0; i < spawnCount; i++)
        {
            int spawnListIndex = trueRandomizedEnemies ? Random.Range(0, spawnList.Length) : i % spawnList.Length % spawnPositions.Length;
            Transform spawn = unusedSpawns[Random.Range(0, unusedSpawns.Count)];
            
            EntityBase entity = WaveManager.Instance.SpawnEntity(spawnList[spawnListIndex], spawn);
            
            unusedSpawns.Remove(spawn);
        }

        End();
    }
}