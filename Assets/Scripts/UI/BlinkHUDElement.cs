using TMPro;
using UnityEngine;

public class BlinkHUDElement : HUDElement
{
    [SerializeField] protected TMP_Text countdownText;
    public virtual void SetTimer(float time)
    {
        
        animator.SetFloat("Timer", time);
        countdownText.text = ((int)time+1).ToString();
    }
    
    public void SetRecharged(bool recharged)
    {
        animator.SetBool("Recharged", recharged);
    }
}
