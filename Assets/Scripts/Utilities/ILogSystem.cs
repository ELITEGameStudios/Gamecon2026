using JetBrains.Annotations;
using UnityEngine;

public interface ILogSystem
{
    void Log(string message);
}

public class UnityLogger : ILogSystem
{
    public void Log(string message)
    {
        UnityEngine.Debug.Log(message);
    }
}

