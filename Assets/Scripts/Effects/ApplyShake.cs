using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyShake : MonoBehaviour
{

    [SerializeField] private Transform camTf;
    public bool isShaking;
    public CamShakeProfile profile;

    [System.Serializable]
    public struct CamShakeProfile
    {
        public AnimationCurve amplitudeCurve;
        public AnimationCurve lerpCurve;
        public float time;
        public int frequency;
        public float interval => time / frequency;
    }


    public void StartShake(CamShakeProfile profile)
    {
        if(isShaking){StopCoroutine(ShakeCoroutine());}
        
        this.profile = profile;
        isShaking = true;
        StartCoroutine(ShakeCoroutine());
    }

    Vector2[] GenerateOffsets()
    {
        int totalPoints = profile.frequency;
        float timeInterval = profile.interval;
        // float simTime = profile.time;

        List<Vector2> points = new();
        for (int i = 0; i < totalPoints - 1; i++)
        {
            float simTime = profile.time - (timeInterval * (i+1));
            Debug.Log(simTime);
            
            float randomAngle = Random.Range(0, 360);
            points.Add(
                new Vector2(
                    Mathf.Cos(randomAngle * Mathf.Rad2Deg),
                    Mathf.Sin(randomAngle * Mathf.Rad2Deg)
                ).normalized
                * profile.amplitudeCurve.Evaluate(1 - simTime / profile.time)
            );
        }

        points.Add(Vector2.zero);

        return points.ToArray();
    }

    IEnumerator ShakeCoroutine()
    {

        Vector2[] points = GenerateOffsets();
        float timer = 0;
        foreach (Vector2 point in points)
        {
            timer = profile.interval;
            Vector2 prevPosition = camTf.localPosition;
            
            while (timer > 0)
            {
                camTf.localPosition = Vector2.Lerp(prevPosition, point, profile.lerpCurve.Evaluate(1 - timer / profile.interval));
                timer -= Time.deltaTime;
                yield return null;
            }
        }

        camTf.localPosition = Vector2.zero;
        isShaking = false;
    }
}
