using TMPro;
using UnityEngine;

public class LeaderboardEntry : MonoBehaviour
{
    public TMP_Text playerName;
    public TMP_Text levelTime;
    public TMP_Text rankNum;

    public void InitEntry(string playerName, string levelTime, int rank = 1)
    {
        this.playerName.text = playerName;
        this.levelTime.text = levelTime;
        this.rankNum.text = rank.ToString();
    }
}
