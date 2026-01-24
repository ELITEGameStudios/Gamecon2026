using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameManager : MonoBehaviour
{
    public IVictoryCondition victoryCondition;
    public IEntityManager entityManager;

    event Action<float> gameEnding;
    [Header("Managers")]
    [SerializeField] TimerManager timerManager;
    [SerializeField] HUDManager hudManager;
    [SerializeField] LeaderboardManager leaderboardManager;

    [Header("Temporary Level Picker")]
    [SerializeField] LevelDatabase.LevelNames currentLevel;

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
        switch (levelObject.levelType)
        {
            case LevelData.LevelType.KillTargets:
                victoryCondition = new KillTargets(levelObject.levelDuration);
                
                entityManager = new WaveManager(levelObject);
                entityManager.Initialize();
                hudManager.InitManager(entityManager, victoryCondition);
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

    }

    private void Awake()
    {
        _ = InitializeManager();
    }


    void OnVictory()
    {   
        if (gameOver) return;
        Debug.Log("gameOver list = " + gameEnding.GetInvocationList());
        gameOver = true;
        gameEnding.Invoke(timerManager.GetCurrentLevelTime());
        Debug.Log("Won game");
    }
    void OnDefeat()
    {
        if (gameOver) return;
        Debug.Log("gameOver list = " + gameEnding.GetInvocationList());
        gameOver = true;
        gameEnding?.Invoke(LeaderboardManager.FAILURE_LEVEL_TIME);
        Debug.Log("Lost game");
    }
    private void OnDestroy()
    {
        victoryCondition.OnDisable();
    }
    public void OnTimerUpdated()
    {
        var time = timerManager.GetCurrentLevelTime();
        victoryCondition?.TimerLogic(time);
        entityManager?.TimerLogic(time);
    }
    private void Update()
    {
        OnTimerUpdated();
    }
}
