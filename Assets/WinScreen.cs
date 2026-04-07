using TMPro;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    [SerializeField] GameObject winScreen;
    [SerializeField] LeaderboardManager leaderboardManager;
    [SerializeField] TimerManager timer;
    [SerializeField] TMP_Text timerText, deathsText, leaderboardPos;

    public void OpenWinScreen(string finalTime)
    {
        winScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        timerText.text = finalTime;
        deathsText.text = Player.instance.deathsCount.ToString();
        leaderboardPos.text = leaderboardManager.GetProjectedPlacement(timer.GetCurrentLevelTime()).ToString();
    }
}
