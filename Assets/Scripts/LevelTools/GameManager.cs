using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameManager : MonoBehaviour
{

    public IVictoryCondition victoryCondition;
    public IEntityManager entityManager;
    [SerializeField] TimerManager timerManager;
    [SerializeField] HUDManager hudManager;

    [SerializeField] LevelDatabase.LevelNames currentLevel;

    private async Task InitializeManager()
    {
        var handle = Addressables.LoadAssetAsync<LevelData>(currentLevel.ToString());
        var levelObject = await handle.Task;
        if (levelObject == null)
        {
            Debug.LogError("Couldn't find current level " + currentLevel.ToString());
            return;
        }
        switch (levelObject.levelType)
        {
            case LevelData.LevelType.KillTargets:
                victoryCondition = new KillTargets();
                
                entityManager = new WaveManager(levelObject);
                entityManager.Initialize();
                hudManager.InitManager(entityManager);

                break;
        }
        victoryCondition?.Initialize();
        victoryCondition.victoryAchieved += OnVictory;
        victoryCondition.defeatAchieved += OnDefeat;
    }

    private void Awake()
    {
        _ = InitializeManager();
    }
    void OnVictory()
    {
        Debug.Log("Won game");
    }
    void OnDefeat()
    {
        Debug.Log("Lost game");
    }
    private void OnDestroy()
    {
        victoryCondition.OnDisable();
    }
    public void OnTimerUpdated()
    {
        var time = timerManager.GetTimer();
        victoryCondition?.TimerLogic(time);
        entityManager?.TimerLogic(time);
    }
    private void Update()
    {
        OnTimerUpdated();
    }
}
