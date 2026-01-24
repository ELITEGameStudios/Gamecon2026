using System;
using System.Collections.Generic;
using System.Diagnostics;
public interface IEntityManager
{
     bool spawnEnemies { set; get; }
     event Action allEnemiesDefeated; 
    void Initialize();

    void OnEnemyDefeated(EnemyBase enemy);

    void TimerLogic(float tracker);
}
public class WaveManager : IEntityManager
{
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
    List<WaveData> waveData;

    WaveData currentWave;
    GateData currentGate;

    float gateTracker;
    int gateIndex = 0;
    public int waveIndex = 0;

    bool spawnedFirstGate = false;

    List<EnemyBase> enemiesInGateRemaining = new();

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

    public void StartNextGate()
    {
        int nextGateIndex = currentWave.enemyGates.IndexOf(currentGate) + 1;
        var nextGate = currentWave.enemyGates[nextGateIndex];
        InitGate(nextGate);
        gateIndex++;
        spawnedGate?.Invoke();
    }
    public void InitWave(WaveData data)
    {
        currentWave = data;
        gateTracker = currentWave.timeBeforeFirstGate;
        spawnedFirstGate = false;
        gateIndex = 0;
        waveStarted?.Invoke(waveIndex);
    }

    public void InitGate(GateData data)
    {
        foreach (var enemy in data.enemiesInGate)
        {
            var newEnemy = UnityEngine.Object.Instantiate(enemy);
            newEnemy.transform.position = data.spawnPos;
            newEnemy.entityKilled.AddListener((entity) => OnEnemyDefeated(newEnemy));
            enemiesInGateRemaining.Add(newEnemy);
        }
        UnityEngine.Debug.Log("Spawned " + enemiesInGateRemaining.Count + " enemies");
        currentGate = data;
    }

    public int GetCurrentWaveIndex()
    {
        return waveData.IndexOf(currentWave);
    }

    public void OnEnemyDefeated(EnemyBase defeatedEnemy)
    {
        if (!enemiesInGateRemaining.Contains(defeatedEnemy)) return;
        
        defeatedEnemy.entityKilled.RemoveListener((entity) => OnEnemyDefeated(defeatedEnemy));
        enemiesInGateRemaining.Remove(defeatedEnemy);
        if (enemiesInGateRemaining.Count == 0)
        {
            bool anotherGateInWave = gateIndex < currentWave.enemyGates.Count;
            bool anotherWaveInLevel = waveIndex < waveData.Count;

            if (anotherGateInWave) StartNextGate();
            else if (anotherWaveInLevel)
            {
                waveEnded?.Invoke();
                StartNextWave();
                UnityEngine.Debug.Log("Starting new wave");
            }
            else
            {
                allEnemiesDefeated?.Invoke();
            }
        }
    }
    public void Initialize()
    {
        waveIndex = 1;
        InitWave(waveData[0]);
    }

    public void TimerLogic(float timer)
    {
        if (spawnedFirstGate) return;
        gateTracker = timer;
        if (gateTracker > currentWave.timeBeforeFirstGate)
        {
            spawnedFirstGate = true;
            InitGate(currentWave.enemyGates[0]);
            gateIndex = 1;
            
        }
    }
    public WaveManager(LevelData data)
    {
        waveData = data.levelWaves;
    }

}

