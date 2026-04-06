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
    public FillTimerHudElement blinkElement;
    
    [Space(20)] 
    // public GameUIPopup dashTutPrompt;
    public GameUIPopup blinkPrompt;
    public GameUIPopup shootPrompt;
    public GameUIPopup recallPrompt;
    public GameUIPopup jumpPrompt;
    public GameUIPopup soarPrompt;
    

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
            waveDisplay.text = "Wave 1";
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


    public void UpdateDashSoarState(float windAsPercent)
    {
        if(dashElement != null){
            dashElement.SetSoarTime(windAsPercent);
        }
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
