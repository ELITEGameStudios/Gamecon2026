using UnityEngine;

public class ApplyAngleProportional : MonoBehaviour
{
    public Transform cameraTf;
    public float Kp = 0.01f;
    public float targetRoll = 0;
    public float testVar ;
    public float offset = -15;
    // public float ;
    float currentRoll => cameraTf.localEulerAngles.z < 180? cameraTf.localEulerAngles.z : cameraTf.localEulerAngles.z - 360;
    float gain => (currentRoll - targetRoll) * -Kp;


    public void SetAngle(float newAngle)
    {
        // targetRoll = newAngle + offset;
        targetRoll = newAngle;
    }
    // Update is called once per frame
    void Update()
    {
        // Debug.Log(gain + " => Gain");
        // Debug.Log(currentRoll + " => current roll");
        cameraTf.Rotate(Vector3.forward * gain);   
    }
}
