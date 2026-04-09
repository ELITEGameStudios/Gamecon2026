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
    
    [Header("Gameplay UI Elements")]
    public DashHUDElement dashElement; 
    public RecallHUD recallElement; 
    public BlinkHUDElement blinkElement;
    [SerializeField] TMP_Text enemiesRemainingDisplay;
    
    [Space(20)] 
    // public GameUIPopup dashTutPrompt;
    public GameUIPopup blinkPrompt;
    public GameUIPopup shootPrompt;
    public GameUIPopup recallPrompt;
    public GameUIPopup jumpPrompt;
    public GameUIPopup soarPrompt;
    

    public static HUDManager Instance { get; private set;}

    IEntityManager entityManager;

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
            waveDisplay.text = "1";
            waveDisplay.gameObject.SetActive(true);
            waveManager.waveStarted += OnWaveChanged;
        }
        else
        {
            waveDisplay.gameObject.SetActive(false);
        }
        enemiesRemainingDisplay.text = entityManager.GetEnemiesRemaining().ToString() ;
        entityManager.enemyDefeated += OnEnemyDefeated;
        this.entityManager = entityManager;
        victoryCondition.victoryAchieved += OnLevelWon;
        victoryCondition.defeatAchieved += OnLevelLost;
    }


    public void UpdateDashSoarState(float windAsPercent)
    {
        if(dashElement != null){
            dashElement.SetSoarTime(windAsPercent);
        }
    }

    void OnEnemyDefeated(EnemyBase enemy)
    {
        enemiesRemainingDisplay.text = entityManager.GetEnemiesRemaining().ToString();
    }
    void OnWaveChanged(int index)
    {
        waveDisplay.text = index.ToString();
        enemiesRemainingDisplay.text = entityManager.GetEnemiesRemaining().ToString();
    }

    void OnLevelWon()
    {
        waveDisplay.text = "X";
    }

    void OnLevelLost()
    {
        waveDisplay.text = "X";
    }
    public void UpdateRecallCooldown(float currentCooldown, float maxCooldown)
    {
        // if (recallCooldownFill == null) return;

        if (currentCooldown > 0)
        {
            float fillAmount = currentCooldown / maxCooldown;

            
            // recallCooldownFill.fillAmount = fillAmount;
            
            // if (recallCooldownText != null)
            //     recallCooldownText.gameObject.SetActive(true);
            //     recallCooldownText.text = currentCooldown.ToString("F1");
        }
        else
        {


            if (recallCooldownText != null)
                recallCooldownText.gameObject.SetActive(false);
            // recallCooldownFill.fillAmount = 0;
        }
    }

    public void OnSoar() {if(soarPrompt.active) {soarPrompt.Deactivate(); soarPrompt.locked = true;} }
    public void OnWalljump() {jumpPrompt.Deactivate();}
    public void TriggerBlinkPrompt() {blinkPrompt.Deactivate(); blinkPrompt.locked = true;}
    public void TriggerRecallPrompt() {recallPrompt.Deactivate(); recallPrompt.locked = true;}
    public void TriggerShootPrompt() { 
        shootPrompt.Deactivate(); 
        shootPrompt.locked = true; 
        if(PlayerMovementStateMachine.instance.parryTutorialEvent != null){PlayerMovementStateMachine.instance.parryTutorialEvent.End();}
    }
    // public void TriggerDashPrompt() {recallPrompt.Deactivate();}
}
