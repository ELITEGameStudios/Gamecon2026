using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSTracker : MonoBehaviour
{
    const int NUMBER_OF_FRAMES_TO_TRACK = 60;

    [SerializeField] TMP_Text fpsTracker;

    Queue<float> deltaTimes = new Queue<float>();


    private void Update()
    {
        if (deltaTimes.Count == NUMBER_OF_FRAMES_TO_TRACK)
        {
            deltaTimes.Dequeue();
        }
        deltaTimes.Enqueue(Time.unscaledDeltaTime);

        fpsTracker.text = "FPS: " + GetAverageFPS().ToString();
    }

    float GetAverageFPS()
    {
        float sum = 0;
        foreach (var t in deltaTimes)
        {
            sum += t;
        }
        return  Mathf.Floor(deltaTimes.Count / sum);
    }
}