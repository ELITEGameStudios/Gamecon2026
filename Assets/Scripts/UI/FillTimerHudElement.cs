using UnityEngine;
using UnityEngine.UI;

public class FillTimerHudElement : HUDElement
{
    [SerializeField] protected Image fillImage;
    [SerializeField] protected float fillFactor;
    [SerializeField] protected float padding;

    public virtual void SetFillFactor(float fillFactor)
    {
        
        this.fillFactor = (padding/2) + fillFactor * (1-padding);
        fillImage.fillAmount = this.fillFactor;
    }
    
}
