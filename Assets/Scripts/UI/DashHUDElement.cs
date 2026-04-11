using TMPro;
using UnityEngine;

public class DashHUDElement : HUDElement
{
    [SerializeField] private string soarString;
    public void SetSoarTime(float newFloat)
    {
        animator.SetFloat(soarString, newFloat);
    }
    public override void Activate()
    {
        isReady = false;
        animator.SetBool("Active", true);
        animator.SetBool(ReadyStateString, isReady);
    }

    public void Deactivate()
    {
        animator.SetBool("Active", false);
    }
}
