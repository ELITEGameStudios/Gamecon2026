using UnityEngine;
using System;


public interface IVictoryCondition
{
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

    const float TIME_TO_WIN = 5.0f;

    bool gameOver;
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
        if (gameOver) return;
        if (tracker > TIME_TO_WIN)
        {
            defeatAchieved?.Invoke();
            gameOver = true;
        }
    }
}
