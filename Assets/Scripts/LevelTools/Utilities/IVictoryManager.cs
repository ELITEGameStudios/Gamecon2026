using UnityEngine;
using System;



public interface IVictoryCondition
{

    public const float TIMER_DISABLED_VALUE = -1.0f;
    bool gameOver { get; set; }
    event Action victoryAchieved;
    event Action defeatAchieved;
    void Initialize();
    void OnDisable();
    void OnEnemiesDefeated();
    void TimerLogic(float tracker);
}
public class KillTargets : IVictoryCondition
{
    public event Action victoryAchieved;
    public event Action defeatAchieved;

    float timeToWin = 5.0f;

    public bool gameOver = false;
    bool IVictoryCondition.gameOver { get => gameOver; set => gameOver = value; }

    public void Initialize()
    {
      
    }
    public void OnEnemiesDefeated()
    {
        victoryAchieved.Invoke();
    }
    public void OnDisable()
    {
       
    }
    public void TimerLogic(float tracker)
    {
        if (gameOver || timeToWin <= IVictoryCondition.TIMER_DISABLED_VALUE) return;
        if (tracker > timeToWin)
        {
            defeatAchieved?.Invoke();
            gameOver = true;
        }
    }
    public KillTargets(float time)
    { 
        timeToWin = time;
    }
}

