using System;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityManager
{
     bool spawnEnemies { set; get; }
     event Action allEnemiesDefeated; 
    void Initialize();

    void OnEnemyDefeated(EnemyBase enemy);

    void TimerLogic(float tracker);
}

[System.Serializable]
public class WaveManager : IEntityManager
{
    public static WaveManager Instance {get; private set;}
    public event Action<int> waveStarted;
    public event Action waveEnded;
    public event Action spawnedGate;
    public event Action allEnemiesDefeated;
    bool IEntityManager.spawnEnemies { get => spawnEnemies; set => spawnEnemies = value; }

    bool spawnEnemies;

    /// <summary>
    /// Key: Wave data, Value: time to spawn wave at
    /// </summary>
    /// 
    List<WaveBase> waveData;
    WaveBase currentWave;
    public List<SpawnProfile> activeProfiles;

    float gateTracker;
    int gateIndex = 0;
    public int waveIndex = 0;

    bool initiatedWave = false;

    public List<EnemyBase> enemiesInWaveRemaining;

    public float GetTimeUntilNextWave()
    {
        return currentWave.timeBeforeFirstGate;
    }

    public void StartNextWave()
    {
        int nextWaveIndex = waveData.IndexOf(currentWave) + 1;
        var nextWave = waveData[nextWaveIndex];
        waveIndex++;
        InitWave(nextWave);
    }

    public void UpdateWaves()
    {
        if (currentWave == null) return;
        currentWave.InternalWaveUpdate();
        if (!currentWave.finishedProcesses)
        {
            for (int i = activeProfiles.Count-1; i >= 0; i--) { 
                activeProfiles[i].SpawnProcessUpdate(); 
            }
        }
    }

    // public void StartNextGate()
    // {
    //     int nextGateIndex = currentWave.enemyGates.IndexOf(currentGate) + 1;
    //     var nextGate = currentWave.enemyGates[nextGateIndex];
    //     InitGate(nextGate);
    //     gateIndex++;
    //     spawnedGate?.Invoke();
    // }
    
    public void InitWave(WaveBase data)
    {
        Debug.Log("Starting Wave...");
        currentWave = data;

        activeProfiles = new();
        enemiesInWaveRemaining = new();

        currentWave.InternalStart();
        // gateTracker = currentWave.timeBeforeFirstGate;
        // spawnedFirstGate = false;
        // gateIndex = 0;
        waveStarted?.Invoke(waveIndex);
    }

    // public void InitGate(GateData data)
    // {
    //     foreach (var enemy in data.enemiesInGate)
    //     {
    //         var newEnemy = UnityEngine.Object.Instantiate(enemy);
    //         newEnemy.transform.position = data.spawnPos;

    //     }
    // }

    public int GetCurrentWaveIndex()
    {
        return waveData.IndexOf(currentWave);
    }

    public void OnEnemyDefeated(EnemyBase defeatedEnemy)
    {
        if (!enemiesInWaveRemaining.Contains(defeatedEnemy)) return;
        
        defeatedEnemy.entityKilled.RemoveListener((entity) => OnEnemyDefeated(defeatedEnemy));
        enemiesInWaveRemaining.Remove(defeatedEnemy);
        CheckUniversalEndCondition();
    }

    public void CheckUniversalEndCondition() // This ensures that no matter which type of wave this is, the wave can only end once this final condition passes
    {
        if(enemiesInWaveRemaining.Count == 0 && currentWave.finishedProcesses){
            currentWave.EndWave();
        }    
    }

    public void AddActiveSpawnProcess(SpawnProfile profile){
        activeProfiles.Add(profile);
    }

    public void RemoveActiveSpawnProcess(SpawnProfile profile){
        activeProfiles.Remove(profile);
    }

    public void OnWaveEnd()
    {
        // bool anotherGateInWave = gateIndex < currentWave.enemyGates.Count;
        bool anotherWaveInLevel = waveIndex < waveData.Count;

        // if (anotherGateInWave) StartNextGate();
        /* else */ if (anotherWaveInLevel)
        {
            waveEnded?.Invoke();
            StartNextWave();
        }
        else
        {
            allEnemiesDefeated?.Invoke();
        }
    }

    public void Initialize()
    {
        if(Instance == null){Instance = this;}
        else if(Instance != this){Debug.Log("idk rn");}

        Debug.Log("Initializing Wave System...");
        waveIndex = 1;
        InitWave(waveData[0]);
    }

    public void TimerLogic(float timer)
    {
        // if (initiatedWave) return;
        // gateTracker = timer;
        // if (gateTracker > currentWave.timeBeforeFirstGate)
        // {
        //     initiatedWave = true;
        //     // InitGate(currentWave.enemyGates[0]);
        //     gateIndex = 1;
            
        // }
    }

    public EntityBase SpawnEntity(EntityBase enemyBase, Transform position)
    {
        
        // Transform newEntityHost = enemyBase.transform;
        Debug.Log("Spawned entity?");
        EntityBase newEntity = EntityBase.Instantiate(enemyBase, position.position, position.rotation);
        // newEntityHost.transform.SetParent(null);
        
        if(newEntity is EnemyBase)
        {
            EnemyBase newEnemy = newEntity as EnemyBase;
            newEnemy.entityKilled.AddListener((entity) => OnEnemyDefeated(newEnemy));
            enemiesInWaveRemaining.Add(newEnemy);
            // enemiesInWaveRemaining.Add(newEnemy);
            
        }

        return newEntity;
    }
    
    public WaveManager(LevelData data)
    {
        waveData = data.levelWaves;
    }

}

