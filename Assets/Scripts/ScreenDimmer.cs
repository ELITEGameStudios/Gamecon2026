using UnityEngine;
using UnityEngine.UI;

public class ScreenDimmer : MonoBehaviour
{
    public float alpha, timer;
    public Image image;
    public DimOperation currentOperation;
    public static ScreenDimmer main {get; private set;}
    public bool intendedAsMain;

    public struct DimOperation{
        public float time, target, startDim;
        public int dimPriority;
    }

    public bool IsDim(){return alpha > 0;}

    public void DimScreen(float targetAlpha = 0.5f, float time = 0.5f, int dimPriority = 0)
    {
        if(dimPriority < currentOperation.dimPriority && currentOperation.target != 0){return;}

        currentOperation.startDim = alpha;
        currentOperation.target = targetAlpha;
        currentOperation.time = time;
        currentOperation.dimPriority = dimPriority;

        if(time <= 0){
            alpha = targetAlpha;
            image.color = Color.Lerp(Color.clear, Color.black, alpha);
        }

        timer = time;
    }

    void Awake()
    {
        if (intendedAsMain)
        {
            if(main == null){
                main = this;
                DontDestroyOnLoad(gameObject);
            }
            else if(main != this){
                Destroy(this);
            }
        }
    }

    void Update()
    {
        if(alpha != currentOperation.target)
        {
            timer -= Time.unscaledDeltaTime;
            alpha = Mathf.Lerp(currentOperation.target, currentOperation.startDim, timer/currentOperation.time);
        }

        image.color = Color.Lerp(Color.clear, Color.black, alpha);
    }
}
