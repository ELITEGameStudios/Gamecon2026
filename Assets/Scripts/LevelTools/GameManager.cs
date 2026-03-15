using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    public IVictoryCondition victoryCondition;
    public IEntityManager entityManager;
    public bool Initialized { get; private set;}

    public event Action<float> gameEnding;

    [SerializeField] EntityDetectionSystem feds;
    [Header("Managers")]
    [SerializeField] TimerManager timerManager;
    [SerializeField] HUDManager hudManager;
    [SerializeField] LeaderboardManager leaderboardManager;
    [SerializeField] SettingsMenu settingsScreen;
    [SerializeField] RespawnManager respawnManager;

    [Header("Temporary Level Picker")]
    [SerializeField] LevelDatabase.LevelNames currentLevel;

    public LevelDatabase.LevelNames CurrentLevel { get; private set; }

    public SettingsMenu GetSettingsMenu(){return settingsScreen;}
    bool gameOver = false;
    private async Task InitializeManager()
    {
        var handle = Addressables.LoadAssetAsync<LevelData>(currentLevel.ToString());
        var levelObject = await handle.Task;
        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed)
        {
            Debug.LogError("Couldn't find current level " + currentLevel.ToString() + ": " + handle.OperationException);
            return;
        }        
       // var preExistingEnemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.InstanceID).ToList();
        switch (levelObject.levelType)
        {
            case LevelData.LevelType.KillTargets:
                victoryCondition = new KillTargets(levelObject.levelDuration);
                entityManager = new WaveManager(levelObject);
                entityManager.Initialize(null);
                entityManager.allEnemiesDefeated += victoryCondition.OnEnemiesDefeated;
                break;
            case LevelData.LevelType.Survive:
                victoryCondition = new KillTargets(levelObject.levelDuration);
                entityManager = new ArenaManager();
                var enemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.InstanceID).ToList();
                entityManager.Initialize(enemies);
                entityManager.allEnemiesDefeated += victoryCondition.OnEnemiesDefeated;
                break;
        }
        if (hudManager != null) hudManager.InitManager(entityManager, victoryCondition);

        if (respawnManager != null) respawnManager.InitManager(victoryCondition, entityManager);
        if (feds != null) feds.InitDetectionSystem(entityManager);
        
        victoryCondition?.Initialize();
        victoryCondition.victoryAchieved += OnVictory;
        victoryCondition.defeatAchieved += OnDefeat;
        if (leaderboardManager != null)
        {
            gameEnding += leaderboardManager.OnLevelOver;
            leaderboardManager.InitManager(currentLevel);            
        }
        if (settingsScreen != null)
        {
            gameEnding += settingsScreen.OnGameOver;
        }
    }

    private void Awake()
    {
        if(Instance == null){Instance = this;}
        else if(Instance != this){Destroy(this);}

        try
        {
            _ = InitializeManager();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

    }



    void OnVictory()
    {   
        if (gameOver) return;
        gameOver = true;
        gameEnding?.Invoke(timerManager.GetCurrentLevelTime());
    }
    void OnDefeat()
    {
        if (gameOver) return;
        gameOver = true;
        gameEnding?.Invoke(LeaderboardManager.FAILURE_LEVEL_TIME);
    }
    private void OnDestroy()
    {
        victoryCondition?.OnDisable();
    }
    public void OnTimerUpdated()
    {
        var time = timerManager.GetCurrentLevelTime();
        victoryCondition?.TimerLogic(time);
        entityManager?.TimerLogic(time);
    }
    private void Update()
    {
        if(!Initialized) return;
        OnTimerUpdated();
        if(entityManager is WaveManager){(entityManager as WaveManager).UpdateWaves();}
    }
}
