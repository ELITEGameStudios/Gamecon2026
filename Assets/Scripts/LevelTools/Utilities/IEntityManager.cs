using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public interface IEntityManager
{
     bool spawnEnemies { set; get; }
     event Action allEnemiesDefeated;

    event Action<EnemyBase> enemyDefeated;
    void Initialize(List<EnemyBase> sceneEnemies);

    void OnEnemyDefeated(EnemyBase enemy);

    void TimerLogic(float tracker);

    EnemyBase GetClosestEnemyToPosition(Vector3 position, List<EnemyType> blacklist);

    int GetEnemiesRemaining();
}
/// <summary>
/// Class for preset enemies in the scene instead of wave based combat
/// </summary>
public class ArenaManager : IEntityManager
{

    
    public bool spawnEnemies { get => false; set { } }

    public event Action allEnemiesDefeated;
    public event Action<EnemyBase> enemyDefeated;

    List<EnemyBase> arenaEnemies = new();
    public EnemyBase GetClosestEnemyToPosition(Vector3 position, List<EnemyType> blacklist)
    {
        float distanceToBeat = float.MaxValue;
        EnemyBase closest = null;


        foreach (var enemy in arenaEnemies)
        {
            //Use sqr magnitude because the relative sizes between each element is what matters, not absolute
            //for example:
            //a = 100
            //b = 1000
            //spending time doing sqr root is unnecessary sqrt(b) == 100 > sqrt(a) == 10 

            if (blacklist.Contains(enemy.EnemyType)) continue;
            else if (enemy.health <= 0) continue;
            float distance = (position - enemy.transform.position).sqrMagnitude;
            if (distance < distanceToBeat)
            {
                distanceToBeat = distance;
                closest = enemy;
            }
        }
        return closest;
    }
    public void Initialize(List<EnemyBase> sceneEnemies)
    {
        arenaEnemies = sceneEnemies;
        foreach (var enemy in arenaEnemies)
        {
            enemy.entityKilled.AddListener((entity) => OnEnemyDefeated(enemy));
        }
    }

    public void OnEnemyDefeated(EnemyBase defeatedEnemy)
    {
        if (arenaEnemies.Contains(defeatedEnemy))
        {
            arenaEnemies.Remove(defeatedEnemy);
            defeatedEnemy.entityKilled.RemoveListener((entity) => OnEnemyDefeated(defeatedEnemy));
            enemyDefeated.Invoke(defeatedEnemy);
        }
    }

    public void TimerLogic(float tracker)
    {
        //doesn't need a timer
    }

    public int GetEnemiesRemaining()
    {
        return arenaEnemies.Count;
    }
}
public class WaveManager : IEntityManager
{
    public static WaveManager Instance {get; private set;}
    public event Action<int> waveStarted;
    public event Action waveEnded;
    public event Action spawnedGate;
    public event Action allEnemiesDefeated;
    public event Action<EnemyBase> enemyDefeated;

    bool IEntityManager.spawnEnemies { get => spawnEnemies; set => spawnEnemies = value; }


    bool spawnEnemies;

    List<WaveBase> waveData;
    WaveBase currentWave;
    public List<SpawnProfile> activeProfiles;

    public int waveIndex = 0;


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
        if(!GameManager.Instance.Initialized){return;}

        if (currentWave == null) return;
        currentWave.InternalWaveUpdate();
        if (!currentWave.finishedProcesses)
        {
            for (int i = activeProfiles.Count-1; i >= 0; i--) { 
                activeProfiles[i].SpawnProcessUpdate(); 
            }
        }
    }
    
    public void InitWave(WaveBase data)
    {
        Debug.Log("Starting Wave...");
        currentWave = data;

        activeProfiles = new();
        enemiesInWaveRemaining = new();

        Debug.Log(currentWave);
        currentWave.InternalStart();
        if(GameManager.Instance.waveCounter != null)
        {
            GameManager.Instance.waveCounter.OnStartWave(waveIndex);
        }
        waveStarted?.Invoke(waveIndex);
    }

    public void RestartWave()
    {
        InitWave(currentWave);
    }

    public void StartSystem()
    {
        GameManager.Instance.SetInitialized(true);
        waveIndex = 0;
        InitWave(waveData[0]);
    }

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
        enemyDefeated.Invoke(defeatedEnemy);
    }

    public void CheckUniversalEndCondition() // This ensures that no matter which type of wave this is, the wave can only end once this final condition passes
    {
        if (enemiesInWaveRemaining.Count == 0 && currentWave.finishedProcesses)
        {
            currentWave.EndWave();
        }    
    }

    public void AddActiveSpawnProcess(SpawnProfile profile)
    {
        activeProfiles.Add(profile);
    }

    public void RemoveActiveSpawnProcess(SpawnProfile profile)
    {
        activeProfiles.Remove(profile);
    }

    public void OnWaveEnd()
    {
        // bool anotherGateInWave = gateIndex < currentWave.enemyGates.Count;
        bool anotherWaveInLevel = waveIndex < waveData.Count-1;

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

    public void Initialize(List<EnemyBase> sceneEnemies)
    {
        if(Instance == null){Instance = this;}
        else if(Instance != this){}

        Debug.Log("Initializing Wave System...");

        if (GameManager.Instance.autoStartWaves)
        {
            StartSystem();
        }

        if (sceneEnemies == null) return;
        foreach (var enemy in sceneEnemies)
        {
            if (!enemiesInWaveRemaining.Contains(enemy))
            {
                enemy.entityKilled.AddListener((entity) => OnEnemyDefeated(enemy));
                enemiesInWaveRemaining.Add(enemy);
            }
        }
    }

    public void TimerLogic(float timer)
    {

    }

    public EntityBase SpawnEntity(EntityBase enemyBase, Transform position)
    {
        
        // Transform newEntityHost = enemyBase.transform;
        EntityBase newEntity = EntityBase.Instantiate(enemyBase, position.position, position.rotation);
        // newEntityHost.transform.SetParent(null);
        
        if(newEntity is EnemyBase)
        {
            EnemyBase newEnemy = newEntity as EnemyBase;
            newEnemy.entityKilled.AddListener((entity) => OnEnemyDefeated(newEnemy));
            enemiesInWaveRemaining.Add(newEnemy);
        }

        return newEntity;
    }

    public EnemyBase GetClosestEnemyToPosition(Vector3 position, List<EnemyType> blacklist)
    {
        if (enemiesInWaveRemaining == null) return null;
        if (enemiesInWaveRemaining.Count == 1) return enemiesInWaveRemaining[0];
        float distanceToBeat = float.MaxValue;
        EnemyBase closest = null;
        

        foreach (var enemy in enemiesInWaveRemaining)
        {
            //Use sqr magnitude because the relative sizes between each element is what matters, not absolute
            //for example:
            //a = 100
            //b = 1000
            //spending time doing sqr root is unnecessary sqrt(b) == 100 > sqrt(a) == 10 

            if (blacklist.Contains(enemy.EnemyType)) continue;

            float distance = (position - enemy.transform.position).sqrMagnitude;
            if (distance < distanceToBeat)
            {
                distanceToBeat = distance;
                closest = enemy;
            }
        }


        return closest;
    }

    public int GetEnemiesRemaining()
    {
        return enemiesInWaveRemaining.Count;
    }

    public WaveManager(LevelData data)
    {
        waveData = data.levelWaves;
    }

}

