using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Cooldown UI Elements")]
    [SerializeField] private Image recallCooldownFill;
    [SerializeField] private TMP_Text recallCooldownText; // Optional text display

    [SerializeField] private TMP_Text waveDisplay;

    public void InitManager(IEntityManager entityManager)
    {

        InitWaveDisplay(entityManager);
    }

    void InitWaveDisplay(IEntityManager entityManager)
    {
        if (waveDisplay == null) return;
        if (entityManager is WaveManager waveManager)
        {
            Debug.Log("Init HUD manager");
            waveDisplay.gameObject.SetActive(true);
            waveManager.waveStarted += OnWaveChanged;
            waveManager.allEnemiesDefeated += OnLevelComplete;
        }
        else
        {
            waveDisplay.gameObject.SetActive(false);
        }
    }


    void OnWaveChanged(int index)
    {
        Debug.Log("Wave changed");
        waveDisplay.text = "Wave " + index;
    }

    void OnLevelComplete()
    {
        Debug.Log("Level complete");
        waveDisplay.text = "VICTORY";
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
