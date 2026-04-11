using TMPro;
using UnityEngine;

public class LeaderboardEntry : MonoBehaviour
{
    public TMP_Text playerName;
    public TMP_Text levelTime;

    public void InitEntry(string playerName, string levelTime)
    {
        this.playerName.text = playerName;
        this.levelTime.text = levelTime;
    }
}
