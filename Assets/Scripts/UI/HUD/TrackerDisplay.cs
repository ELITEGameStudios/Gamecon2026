using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrackerDisplay : MonoBehaviour
{
    const int DISPLAY_REFRESH_RATE = 10;

    [SerializeField] PlayerTracker tracker;
    [SerializeField] InputActionReference toggleDisplay;
    [SerializeField] GameObject display;

    [SerializeField] TMP_Text accuracyDisplay;
    [SerializeField] TMP_Text avgSpeedDisplay;
    [SerializeField] TMP_Text parryDisplay;
    [SerializeField] TMP_Text reclaimDisplay;
    [SerializeField] TMP_Text blinkDisplay;
    [SerializeField] TMP_Text windDisplay;


    [SerializeField] TMP_Text blinksCounter;
    [SerializeField] TMP_Text recallCounter;
    [SerializeField] TMP_Text dashCounter;


    int refreshTracker = DISPLAY_REFRESH_RATE;
    private void Start()
    {
        if (tracker == null)
        {
            //no use if there's no tracker
            Destroy(gameObject);
            return;
        }
        display.SetActive(false);
        toggleDisplay.action.performed += OnTogglePressed;
        refreshTracker = DISPLAY_REFRESH_RATE;
    }

    void OnTogglePressed(InputAction.CallbackContext context)
    {
        display.SetActive(!display.activeSelf);
    }

    private void FixedUpdate()
    {
        refreshTracker--;
        if (refreshTracker <= 0)
        {

            accuracyDisplay.text = tracker.GetHitAccuracy().ToString("F2") + "%";
            avgSpeedDisplay.text = tracker.GetAverageSpeedWhileFiring().ToString("F2") + " m/s";
            parryDisplay.text = tracker.GetParryAccuracy().ToString("F2") + "%";
            reclaimDisplay.text = tracker.GetAverageKnifeReclaimTime().ToString("F2") + " secs";
            blinkDisplay.text = tracker.GetAverageBlinkDistance().ToString("F2") + "m";
            windDisplay.text = tracker.GetAverageWind().ToString("F2") + "%";

            var tracked = tracker.GetTrackerData();
            blinksCounter.text = tracked.blinksTracker.ToString();
            recallCounter.text = tracked.recallTracker.ToString();
            dashCounter.text = tracked.dashTracker.ToString();

            refreshTracker = DISPLAY_REFRESH_RATE;

        }

    }
}
