using TMPro;
using UnityEngine;

public class BlinkHUDElement : HUDElement
{

    public void SetRecharged(bool recharged)
    {
        animator.SetBool("Recharged", recharged);
    }
}
