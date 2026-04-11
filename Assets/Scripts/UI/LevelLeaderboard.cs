using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LevelAttempt
{
    public string name;
    public float time;

    public LevelAttempt(string name, float time)
    {
        this.name = name;
        this.time = time;
    }
}