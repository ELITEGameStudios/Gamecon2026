using UnityEngine;
using UnityEngine.UI;

public class RecallHUD : FillTimerHudElement
{

    public bool isActive;
    public string isActiveString = "Active";
    public float speed;

    public override void Activate()
    {
        base.Activate();
        isActive = true;
        animator.SetBool(isActiveString, isActive);
    }

    public override void SetReady(bool isReady)
    {
        base.SetReady(isReady);
        animator.SetBool(ReadyStateString, isReady);
    }
    public void Parry()
    {
        animator.SetTrigger("Parry");
        Deactivate();
    }

    public void SetSpeed(float speed)
    {
        this.speed = 1+(speed*2);
        animator.SetFloat("Speed", this.speed);
    }

    protected override void OnUpdate(){}

    public void Deactivate()
    {
        isActive = false;
        animator.SetBool(isActiveString, isActive);
    }
    
}
