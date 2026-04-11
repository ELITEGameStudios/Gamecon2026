using UnityEngine;

public class ParryTutorialEvent : MonoBehaviour
{
    public bool active, canBeActivated;
    public float Kp = 0.01f;
    public Transform[] targetTransforms;

    [SerializeField] Animator anim;
    [SerializeField] GameUIPopup gameUIPopup;
    [SerializeField] AnimationCurve timeScaleByDistance;
    


    public void SetCanBeActivated(bool canBeActivated)
    {
        this.canBeActivated = canBeActivated;
    }

    public Transform GetTargetTransform()
    {
        for (int i = 0; i < targetTransforms.Length; i++)
        {
            try
            {
                if(targetTransforms[i] != null)
                {
                    return targetTransforms[i];
                }
            }
            catch
            {
                continue;
            }
        }

        return null;
    }

    public void TryActivate()
    {
        if(canBeActivated){Begin();}
    }

    public float GetTimeScale(float dist){
        return timeScaleByDistance.Evaluate(dist);
    }

    public void Begin()
    {
        if(GetTargetTransform() == null) {active = false; return;}
        active = true;
        gameUIPopup.Activate();
    }

    public void End()
    {
        if(!active) return;
        active = false;
        Time.timeScale = 1;
        gameUIPopup.Deactivate();
        anim.SetTrigger("PostParry");
    }
}
