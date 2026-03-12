using System.Collections.Generic;
using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Projectile knife;
    [SerializeField] GameManager gameManager;
    [HideInInspector] public TrackerData trackerData;
    Rigidbody playerRb;

    bool checkedAvgVel = false;

    SaveSystem saveSystem = new();

    List<Vector3> velocitiesWhileShooting = new();
    List<float> elapsedUntilKnifeRetrieved = new();


    float elapsedMissingKnifeTime = 0;

    float totalBlinkDistance = 0;
    public void Start()
    {
        trackerData = new();
        playerRb = player.GetComponent<Rigidbody>();
        knife.enemyStruck.AddListener((data) => OnKnifeCollision(true));
        knife.terrainStruck.AddListener((data) => OnKnifeCollision(false));
        knife.projectileAbilities.attemptedParry.AddListener(OnKnifeParryAttempt);
        knife.knifeRetrieved.AddListener(OnKnifeRetrieved);
        knife.projectileAbilities.dashPerformed.AddListener(OnDashPerformed);

        gameManager.gameEnding += OnGameOver;
    }

    void OnDashPerformed()
    {
        trackerData.dashTracker++;
    }

    void OnKnifeRetrieved(KnifeRetrievalInfo info)
    {
        switch (info.pickupType)
        {
            case KnifeRetrievalType.Recall:
                trackerData.recallTracker++;
                break;
            case KnifeRetrievalType.Pickup:
                trackerData.pickupTracker++;
                break;
            case KnifeRetrievalType.Blink:
                trackerData.blinksTracker++; 
                totalBlinkDistance += info.blinkDistance;
                trackerData.avgBlinkDistance = totalBlinkDistance / trackerData.blinksTracker;
                break;
        }
        elapsedUntilKnifeRetrieved.Add(elapsedMissingKnifeTime);
        elapsedMissingKnifeTime = 0;

    }


    public void OnKnifeParryAttempt(bool parry)
    {
        trackerData.parryAttempts++;
        if (parry)
        {
            trackerData.successfulParries++;
        }
    }

    public void OnKnifeCollision(bool hitEnemy)
    {
        trackerData.knifeCollisionCounts++;
        if (hitEnemy)
        {
            trackerData.knifeHitCount++;
            velocitiesWhileShooting.Add(playerRb.linearVelocity);
        }
    }

    private void Update()
    {
        float delta = Time.deltaTime;
        trackerData.timeElapsed += delta;
        elapsedMissingKnifeTime += delta;
    }

    public float GetAverageSpeedWhileFiring()
    {
        Vector3 sum = Vector3.zero;
        foreach (var velocity in velocitiesWhileShooting)
        {
            sum += velocity;
        }
        var avg = (sum / velocitiesWhileShooting.Count).magnitude;
        if (float.IsNaN(avg)) return 0;
        return avg;
    }

    public float GetHitAccuracy()
    {
        var accuracy = (float)trackerData.knifeHitCount / trackerData.knifeCollisionCounts;
        if (float.IsNaN(accuracy)) accuracy = 0;
        return accuracy * 100;
    }

    public float GetParryAccuracy()
    {
        var accuracy = (float)trackerData.successfulParries / trackerData.parryAttempts;
        if (float.IsNaN (accuracy)) accuracy = 0;
        return accuracy * 100;
    }

    public float GetAverageKnifeReclaimTime()
    {
        float sum = 0;
        foreach (var time in elapsedUntilKnifeRetrieved)
        {
            sum += time;
        }
        var avg = sum / elapsedUntilKnifeRetrieved.Count;
        if (float.IsNaN(avg)) return 0;
        return avg;
    }

    public float GetAverageBlinkDistance()
    {
        if (float.IsNaN(trackerData.avgBlinkDistance)) return 0;
        return trackerData.avgBlinkDistance;
    }
    public TrackerData GetTrackerData()
    {
        return trackerData;
    }
    private void OnApplicationQuit()
    {
        OnGameOver(-1);
    }

    void OnGameOver(float gameDuration)
    {
        int numberOfSaves = saveSystem.GetNumberOfFilesInDirectory(TrackerService.GetDataFolderPathForLevel(gameManager.CurrentLevel));
        //number of files returns -1 as a fallback in case there's no directory present
        //but that's fine because we're not accessing data in the directory, we're just adding some
        //if there's no directory we'll make one
        //this helps to make sure that the file names start at one
        Debug.Log("Number of files in directory is " + numberOfSaves);
        if (numberOfSaves < 0) numberOfSaves = 0;
        saveSystem.EnsureSave(TrackerService.GetDataFolderPathForLevel(gameManager.CurrentLevel), (numberOfSaves + 1).ToString(), trackerData);
        Debug.Log("Saving data to " + TrackerService.GetDataFolderPathForLevel(gameManager.CurrentLevel));
    }
}

public struct TrackerData
{
    //Pick-up
    public int recallTracker;
    public int blinksTracker;
    public int pickupTracker;
    //Movement
    public int dashTracker;
    public float avgBlinkDistance;
    //Accuracy 
    public int knifeCollisionCounts;
    public int knifeHitCount;
    //Parrying
    public int parryAttempts;
    public int successfulParries;
    //Misc
    public float timeElapsed;
}


