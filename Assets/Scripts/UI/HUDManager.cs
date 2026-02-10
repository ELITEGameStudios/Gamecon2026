using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Cooldown UI Elements")]
    [SerializeField] private Image recallCooldownFill;
    [SerializeField] private TMP_Text recallCooldownText; // Optional text display

    [SerializeField] private TMP_Text waveDisplay;
    public ScreenDimmer screenDimmer;
    

    public static HUDManager Instance { get; private set;}

    void Awake()
    {
        if(Instance == null){Instance = this;}
        else if(Instance != this){Destroy(gameObject);}
    }

    public void InitManager(IEntityManager entityManager, IVictoryCondition victoryCondition)
    {
        InitWaveDisplay(entityManager, victoryCondition);
    }

    void InitWaveDisplay(IEntityManager entityManager, IVictoryCondition victoryCondition)
    {
        if (waveDisplay == null) return;
        if (entityManager is WaveManager waveManager)
        {
            Debug.Log("Init HUD manager");
            waveDisplay.gameObject.SetActive(true);
            waveManager.waveStarted += OnWaveChanged;
        }
        else
        {
            waveDisplay.gameObject.SetActive(false);
        }
        victoryCondition.victoryAchieved += OnLevelWon;
        victoryCondition.defeatAchieved += OnLevelLost;
    }


    void OnWaveChanged(int index)
    {
        Debug.Log("Wave changed");
        waveDisplay.text = "Wave " + index;
    }

    void OnLevelWon()
    {
        Debug.Log("Level complete");
        waveDisplay.text = "VICTORY";
    }

    void OnLevelLost()
    {
        waveDisplay.text = "DEFEAT";
    }
    public void UpdateRecallCooldown(float currentCooldown, float maxCooldown)
    {
        if (recallCooldownFill == null) return;

        if (currentCooldown > 0)
        {
            float fillAmount = currentCooldown / maxCooldown;
            recallCooldownFill.fillAmount = fillAmount;
            
            if (recallCooldownText != null)
                recallCooldownText.gameObject.SetActive(true);
                recallCooldownText.text = currentCooldown.ToString("F1");
        }
        else
        {
            if (recallCooldownText != null)
                recallCooldownText.gameObject.SetActive(false);
            recallCooldownFill.fillAmount = 0;
        }
    }
}
