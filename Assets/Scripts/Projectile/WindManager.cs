using TMPro;
using UnityEngine;

public class WindManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Rigidbody playerRB;
    [SerializeField] Projectile knife;
    [SerializeField] AnimationCurve speedToWind;
    [SerializeField] TMP_Text windDisplay;
    [SerializeField] float minSpeedForWind;
    [SerializeField] float maxSpeedForWind;
    [SerializeField] float speedToWindRatio = 4.0f;
    float currentWind = 0;
    public float CurrentWind
    {
        get => currentWind;
        set => currentWind = Mathf.Clamp (value, 0, 100);
    }

    [HideInInspector] public bool pauseWindGeneration = false;


    //Square it because getting the magnitude every frame is expensive
    //we're looking for relative values (i.e. 40% max speed) instead of absolute (i.e. moving at 40 m/s)
    float minSpeedSquared;
    float maxSpeedSquared;

    private void Start()
    {
        if (player == null) player = Player.instance;
        if (playerRB == null) playerRB = player.GetComponent<Rigidbody>();
        minSpeedSquared = minSpeedForWind * minSpeedForWind;
        maxSpeedSquared = maxSpeedForWind * maxSpeedForWind;

        knife.knifeRetrieved.AddListener(OnKnifeRetrieved);

    }
    void OnKnifeRetrieved(KnifeRetrievalInfo info)
    {
        if (info.pickupType != KnifeRetrievalType.Pickup)
        {
            CurrentWind = 0;
        }
    }
    private void FixedUpdate()
    {
        if (windDisplay != null) windDisplay.text = "Wind: " + Mathf.RoundToInt(CurrentWind);
        if (pauseWindGeneration) return;
        float speed = new Vector2(playerRB.linearVelocity.x, playerRB.linearVelocity.z).sqrMagnitude;

        if (speed < minSpeedSquared) return;

        var speedAsProgress = speed / maxSpeedSquared;
        if (speedAsProgress > 1.0f) speedAsProgress = 1.0f;
        float windToAdd = speedToWind.Evaluate(speedAsProgress) * speedToWindRatio;

        CurrentWind += windToAdd;

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
    }


}
