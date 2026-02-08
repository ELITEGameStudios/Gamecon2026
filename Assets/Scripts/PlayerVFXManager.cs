using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerVFXManager : MonoBehaviour
{
    public static PlayerVFXManager instance {get; private set;}

    public Volume blinkVolume, ambientSpeedVolume, wallrunVolume, dashVolume;
    public ParticleSystem speedParticles;
    public AnimationCurve speedParticleEffectCurve, blinkPPCurve;
    public Camera mainCam;

    public float dashCenterOffset = 0.1f; 
    public float wallrunEffectKp = 0.01f; 
    public float wallrunCameraFOV = 90; 
    private float dashEffectTime = 0, currentDashTimer = 0;
    private float wallRunTarget => (PlayerMovementStateMachine.instance.currentState is WallRunState) ? 1 : 0;
    private float cameraTarget => (PlayerMovementStateMachine.instance.currentState is WallRunState) ? wallrunCameraFOV : defaultCamFOV;
    private float defaultCamFOV;
    [SerializeField]private float defaultDashIntensity;

    void Update()
    {
        if(speedParticles != null){
            ParticleSystem.TrailModule trailModule = speedParticles.trails;
            trailModule.colorOverTrail = Color.Lerp(Color.clear, Color.white, speedParticleEffectCurve.Evaluate(PlayerMovementStateMachine.instance.currentVelocity));
        }

        if(ambientSpeedVolume != null) ambientSpeedVolume.weight = speedParticleEffectCurve.Evaluate(PlayerMovementStateMachine.instance.currentVelocity);
        if(wallrunVolume != null) wallrunVolume.weight += (wallRunTarget - wallrunVolume.weight) * wallrunEffectKp;

        if(currentDashTimer > 0){
            currentDashTimer -= Time.deltaTime;
            dashVolume.weight = currentDashTimer / dashEffectTime;
        }
        else if(dashVolume.weight > 0){dashVolume.weight = 0;}

        // mainCam.fieldOfView += (cameraTarget - Camera.main.fieldOfView) * wallrunEffectKp;
    }

    public void DashEffect(Vector2 input, float dashTime = 0.5f)
    {
        dashEffectTime = dashTime;
        currentDashTimer = dashEffectTime;
        // for(int i = 0; i < dashVolume.profile.components.Count; i++)
        // {
        //     if(dashVolume.profile.components[i] is LensDistortion)
        //     {
        //         (dashVolume.profile.components[i] as LensDistortion).center = new Vector2Parameter(new Vector2(0.5f - (input.x * dashCenterOffset), 0));
        //         (dashVolume.profile.components[i] as LensDistortion).intensity = new ClampedFloatParameter(defaultDashIntensity * input.y, 0, 1);
        //     }   
        //     if(dashVolume.profile.components[i] is Vignette)
        //     {
        //         (dashVolume.profile.components[i] as Vignette).center = new Vector2Parameter(new Vector2(0.5f - (input.x * dashCenterOffset), 0));
        //     }   
        // }
    }
    void Start()
    {
        defaultCamFOV = mainCam.fieldOfView;
        for(int i = 0; i < dashVolume.profile.components.Count; i++)
        {
            if(dashVolume.profile.components[i] is LensDistortion)
            {
                defaultDashIntensity = (float)(dashVolume.profile.components[i] as LensDistortion).intensity;
                break;
            }
        }
    }

    void Awake()
    {
        if(instance == null){ instance = this;}
        else if(instance != this){Destroy(this);}
    }
}
