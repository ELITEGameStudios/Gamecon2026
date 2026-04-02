using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GruntEnemy : RangedEnemy
{
    [SerializeField] Animator animator;
    [SerializeField] int targetIterations;


    protected override void Shoot(Transform target)
    {
        Debug.Log("Grunt shooting");
        StartCoroutine(ShootCoroutine(target));
    }

    public IEnumerator ShootCoroutine(Transform target)
    {
        animator.SetTrigger("Throw");

        int iterations = 0;

        if (projectileInfo == null) yield break;
        firing = true;
        yield return new WaitForSeconds(delayBeforeFiring);
        foreach (var info in projectileInfo)
        {
            EnemyProjectile projectile = GetProjectile(0);
            projectile.Activate(target, info.useTransformForOffset ? info.offsetTransform.position : transform.position);
            projectilePools[info].Enqueue(projectile);
            iterations++;
            
            if(iterations >= targetIterations){break;}
           if (info != projectileInfo[^1]) yield return new WaitForSeconds(info.delayAfterShot);
        }
        cooldownTracker = cooldown;
        firing = false;
    
    }
}