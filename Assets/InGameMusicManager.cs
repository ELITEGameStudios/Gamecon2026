using UnityEngine;
using FMODUnity;
using FMOD;


public class InGameMusicManager : MonoBehaviour
{
    [SerializeField] StudioEventEmitter eventEmitter, footstepEmitter;
    [SerializeField] string eventA, eventB;
    [SerializeField] string caveFootstepProperty;
    [SerializeField] float caveFootstepValue = 1;
    bool changed;

    void Awake()
    {
        caveFootstepValue = 1;
    }

    void Update()
    {
        if(footstepEmitter != null)
        {
            if(changed && caveFootstepValue > 0){caveFootstepValue -= Time.deltaTime;}
            else{caveFootstepValue = 0;}

            footstepEmitter.SetParameter(caveFootstepProperty, caveFootstepValue);
        }
    }

    public void ToStage1()
    {
        if (eventEmitter != null)
        {
            eventEmitter.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            eventEmitter.EventReference.Path = eventB;
            eventEmitter.EventInstance.start();

            changed = true;
            // eventEmitter.SetParameter("Change Tracks", 1);
        }
    }
}
