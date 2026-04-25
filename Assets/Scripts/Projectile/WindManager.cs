using TMPro;
using UnityEngine;

public class WindManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Rigidbody playerRB;
    [SerializeField] Projectile knife;
    [SerializeField] AnimationCurve speedToWind;
    [SerializeField] TMP_Text windDisplay;
    [SerializeField] TMP_Text speedDisplay;

    [Header("Stats")]
    [SerializeField] float minSpeedForWind;
    [SerializeField] float maxSpeedForWind;
    [SerializeField] float speedToWindRatio = 4.0f;
    [SerializeField] float empoweredDashDrainRate = 120.0f;

    [Header("VFX")]
    [SerializeField] Material tattooMaterial;
    [SerializeField] ParticleSystem windWraps;
    [SerializeField] float maxWindWraps = 20.0f;
    [SerializeField, ColorUsage(true, true)] Color baseWindColor = Color.white;
    [SerializeField, ColorUsage(true, true)] Color maxWindColor = Color.blue;
    [SerializeField, ColorUsage(true, true)] Color minWindTattooColor = Color.white;
    [SerializeField, ColorUsage(true, true)] Color maxWindTattooColor = Color.blue;
    [SerializeField] SkinnedMeshRenderer leftArm;
    [SerializeField] AnimationCurve colorTransitionCurve;
    float currentWind = 0;
    public float CurrentWind
    {
        get => currentWind;
        set => currentWind = Mathf.Clamp(value, 0, 100);
    }

    public float MaxSpeed
    {
        get => maxSpeedForWind;
        private set => maxSpeedForWind = value;
    }

    public float MinSpeed
    {
        get => minSpeedForWind;
        private set => minSpeedForWind = value;
    }

    public float EmpoweredDashDrainRate
    {
        get => empoweredDashDrainRate;
        private set => empoweredDashDrainRate = value;
    }

    [HideInInspector] public bool pauseWindGeneration = false;


    //Square it because getting the magnitude every frame is expensive
    //we're looking for relative values (i.e. 40% max speed) instead of absolute (i.e. moving at 40 m/s)
    float minSpeedSquared;
    float maxSpeedSquared;

    ParticleSystem.EmissionModule wrapsEmission;
    ParticleSystem.MainModule wrapsMainModule;

    Material runtimeTattooMaterial;
    private void Start()
    {
        if (player == null) player = Player.instance;
        if (playerRB == null) playerRB = player.GetComponent<Rigidbody>();
        minSpeedSquared = minSpeedForWind * minSpeedForWind;
        maxSpeedSquared = maxSpeedForWind * maxSpeedForWind;

        knife.knifeRetrieved.AddListener(OnKnifeRetrieved);

        wrapsEmission = windWraps.emission;
        wrapsMainModule = windWraps.main;

        runtimeTattooMaterial = new(tattooMaterial);
        leftArm.material = runtimeTattooMaterial;

    }
    void OnKnifeRetrieved(KnifeRetrievalInfo info)
    {
        if (info.pickupType != KnifeRetrievalType.Pickup)
        {
            CurrentWind = 0;
            UpdateWindDisplays();
        }
    }
    private void FixedUpdate()
    {
        if (!pauseWindGeneration)
        {
            AddWindFromVelocity();
        }
        UpdateWindDisplays();
    }
    void AddWindFromVelocity()
    {
        float speed = new Vector2(playerRB.linearVelocity.x, playerRB.linearVelocity.z).sqrMagnitude;

        if (speed < minSpeedSquared) return;

        var speedAsProgress = speed / maxSpeedSquared;
        if (speedAsProgress > 1.0f) speedAsProgress = 1.0f;
        float windToAdd = speedToWind.Evaluate(speedAsProgress) * speedToWindRatio;

        CurrentWind += windToAdd;
    }
    void UpdateWindDisplays()
    {
        if (windDisplay != null) windDisplay.text = "Wind: " + Mathf.RoundToInt(CurrentWind);
        if (speedDisplay != null) speedDisplay.text = "Speed: " + Mathf.RoundToInt(new Vector2(playerRB.linearVelocity.x, playerRB.linearVelocity.z).magnitude) + "u/s";
        float windAsPercent = GetWindAsPercent();
        if (windWraps.gameObject.activeSelf)
        {
            wrapsEmission.rateOverTime = Mathf.Lerp(0, maxWindWraps, windAsPercent);
            if (windAsPercent > 0.99f)
            {
                wrapsMainModule.startColor = maxWindColor;
            }
            else
            {
                wrapsMainModule.startColor = baseWindColor;
            }

        }

        if (tattooMaterial != null)
        {
            runtimeTattooMaterial.SetFloat("_windPercentage", windAsPercent);
        }
    }

    public bool HasEnoughWindForBlink()
    {
        return currentWind > 99;
    }

    public bool HasEnoughWindForRicochet()
    {
        return currentWind > 99;
    }

    public float GetWindAsPercent()
    {
        return currentWind / 100.0f;
    }

    public void RestoreWindOnKill()
    {
        currentWind = 100.0f;
        UpdateWindDisplays();
    }
}
