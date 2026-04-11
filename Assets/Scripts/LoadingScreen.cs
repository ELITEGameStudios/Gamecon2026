using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance {get; private set;}
    public ScreenDimmer dimmer;
    public Animator loadingWidgetAnimator;
    public bool visible;


    void Awake()
    {
        if (Instance == null){Instance = this;}
        else if(Instance != this){Destroy(this);}
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        dimmer.DimScreen(1, 0, 100);
        dimmer.DimScreen(0, 1, 100);
    }

    public void TriggerScreen(bool visible, float time = 1.5f)
    {
        dimmer.DimScreen(visible ? 1 : 0, time, 100);
        
        // if(!visible){ Invoke(nameof(ToggleKnife), 1);}
        // else{ ToggleKnife(); }
        
        this.visible = visible;
    }

    public void ToggleKnife(bool knifeIsVisible)
    {
        loadingWidgetAnimator.SetBool("Active", knifeIsVisible);
    }


}
