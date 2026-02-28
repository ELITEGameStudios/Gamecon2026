using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    public IVictoryCondition victoryCondition;
    public IEntityManager entityManager;
    public bool initialized;

    event Action<float> gameEnding;

    [SerializeField] Player player;
    [Header("Managers")]
    [SerializeField] TimerManager timerManager;
    [SerializeField] HUDManager hudManager;
    [SerializeField] LeaderboardManager leaderboardManager;
    [SerializeField] SettingsMenu settingsScreen;

    [Header("Temporary Level Picker")]
    [SerializeField] LevelDatabase.LevelNames currentLevel;

    bool gameOver = false;
    private async Task InitializeManager()
    {
        Debug.Log("Starting game manager init");
        var handle = Addressables.LoadAssetAsync<LevelData>(currentLevel.ToString());
        var levelObject = await handle.Task;
        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed)
        {
            Debug.LogError("Couldn't find current level " + currentLevel.ToString() + ": " + handle.OperationException);
            return;
        }

        if (hudManager == null)
        {
            hudManager = FindFirstObjectByType<HUDManager>();
        }
        switch (levelObject.levelType)
        {
            case LevelData.LevelType.KillTargets:
                victoryCondition = new KillTargets(levelObject.levelDuration);
                
                entityManager = new WaveManager(levelObject);
                entityManager.Initialize();
                if (hudManager != null) hudManager.InitManager(entityManager, victoryCondition);
                entityManager.allEnemiesDefeated += victoryCondition.OnEnemiesDefeated;
                break;
        }
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
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }
        player.entityDetectionSystem.InitDetectionSystem(entityManager);
        Debug.Log("Finished game manager init");

    }

    private void Awake()
    {
        if(Instance == null){Instance = this;}
        else if(Instance != this){Destroy(this);}

         Initialize();

    }

    public void Initialize()
    {
        if (initialized) return;
        Debug.Log("Attempting initialization of game manager");
        _ = InitializeManager();
        initialized = true;
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
        if(!initialized) return;

        OnTimerUpdated();
        if(entityManager is WaveManager){(entityManager as WaveManager).UpdateWaves();}
    }
}
