using UnityEngine;

public class GameUIPopup : MonoBehaviour
{
    public Animator animator;
    public bool dontDisable;
    public string activePropertyName = "Active";
    public bool active;
    
    public bool sendAlphaCommandOnActive;
    public float targetAlpha = 0.65f;
    public int dimPriority;
    
    public bool sendTimeCommandOnActive;
    public float targetTimescale = 0;
    public float toTimescaleKp = 1;

    public bool locked;



    public void Activate()
    {
        if(active){return;}
        Toggle();
    }
    public void Deactivate()
    {
        if(!active){return;}
        Toggle();
    }

    void Update()
    {
        if(active && sendTimeCommandOnActive)
        {
            if(Time.timeScale != targetTimescale && toTimescaleKp < 1)
            {
                PerformTimescaleIteration();
            }
        }
    }

    void PerformTimescaleIteration()
    {
        Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimescale, toTimescaleKp);
    }

    public void Toggle()
    {
        if(!GameManager.Instance.GetSettingsMenu().currentSettings.showTutorialPrompts && !active) return;
        if(locked) return;


        // Main operation
        if(!dontDisable) gameObject.SetActive(!active);  
        if(animator != null){animator.SetBool(activePropertyName, !active);}
        active = !active;



        // Additional operations
        if (sendAlphaCommandOnActive){
            if(HUDManager.Instance.screenDimmer != null) HUDManager.Instance.screenDimmer.DimScreen(active ? targetAlpha : 0, dimPriority: dimPriority);
        }

        if (sendTimeCommandOnActive){
            if(toTimescaleKp <= 0 || !active)
            {
                Time.timeScale = active ? targetTimescale : 1;
                return;
            }
            PerformTimescaleIteration();
        }
    }
    
}
