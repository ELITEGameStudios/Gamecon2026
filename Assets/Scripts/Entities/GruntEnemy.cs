using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GruntEnemy : RangedEnemy
{
    [SerializeField] Animator animator;

    protected override void Init()
    {
        base.Init(); 
        startedFiring.AddListener(OnStartFiring);

    }
    void OnStartFiring()
    {
        animator.SetTrigger("Throw");
    }


}