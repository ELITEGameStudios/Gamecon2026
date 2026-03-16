using UnityEngine;
using FMODUnity;
using FMOD;


public class InGameMusicManager : MonoBehaviour
{
    [SerializeField] StudioEventEmitter eventEmitter;

    public void ToStage1()
    {
        if (eventEmitter != null)
        {
            
            eventEmitter.SetParameter("Change Tracks", 1);
        }
    }
}
