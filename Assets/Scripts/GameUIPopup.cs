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

    public void Toggle()
    {
        // Main operation
        if(!dontDisable) gameObject.SetActive(!active);  
        if(animator != null){animator.SetBool(activePropertyName, !active);}
        active = !active;



        // Additional operations
        if (sendAlphaCommandOnActive){
            HUDManager.Instance.screenDimmer.DimScreen(active ? targetAlpha : 0, dimPriority: dimPriority);
        }

        if (sendTimeCommandOnActive){
            Time.timeScale = active ? targetTimescale : 1;
        }
    }
    
}
