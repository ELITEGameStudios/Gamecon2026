using UnityEngine;
using FMODUnity;
using FMOD;
using FMOD.Studio;


public class InGameMusicManager : MonoBehaviour
{
    [SerializeField] StudioEventEmitter eventEmitter, inGameMusicEmitter;
    [SerializeField] EventInstance footstepEmitter;
    [SerializeField] string caveFootstepProperty;
    [SerializeField] float caveFootstepValue = 1;
    bool changed;

    void Awake()
    {
        caveFootstepValue = 1;
    }

    public void SetFootstepEmitter(EventInstance eventInstance)
    {
        footstepEmitter = eventInstance;    
    }

    void Update()
    {
        if(changed && caveFootstepValue > 0){caveFootstepValue -= Time.deltaTime;}
        else{caveFootstepValue = 0;}
        
        PLAYBACK_STATE playback;
        RESULT result = footstepEmitter.getPlaybackState(out playback);
        if(result == RESULT.OK){
            if(playback != PLAYBACK_STATE.STOPPED){ footstepEmitter.setParameterByName(caveFootstepProperty, caveFootstepValue); UnityEngine.Debug.Log("Feet are getting set");}
        }
    }

    public void ToStage1()
    {
        if (eventEmitter != null)
        {
            eventEmitter.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            // inGameMusicEmitter.EventInstance.start();

            changed = true;
            // eventEmitter.SetParameter("Change Tracks", 1);
        }
    }
}
