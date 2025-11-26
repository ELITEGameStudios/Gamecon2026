using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Cooldown UI Elements")]
    [SerializeField] private Image recallCooldownFill;
    [SerializeField] private TMP_Text recallCooldownText; // Optional text display

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
