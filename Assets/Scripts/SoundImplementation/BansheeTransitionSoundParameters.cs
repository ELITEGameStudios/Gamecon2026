using UnityEngine;
using FMODUnity;
using FMOD;

public class BansheeTransitionSoundParameters : MonoBehaviour
{
    public StudioEventEmitter studioEventEmitter;
    [SerializeField] AnimationCurve curve;
    [SerializeField] string parameterName;

    public void Update()
    {
        float distance = (PlayerMovementStateMachine.instance.transform.position - transform.position).magnitude;

        studioEventEmitter.SetParameter(parameterName, curve.Evaluate(distance));
    }
}
