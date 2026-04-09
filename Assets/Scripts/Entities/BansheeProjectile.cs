using System.ComponentModel;
using UnityEngine;

public class BansheeProjectile : EnemyProjectile
{
    [SerializeField] Animator animator;
    [SerializeField] ProjectileClampModifier clampModifier;
    [SerializeField] Collider groundChecker;
    [SerializeField] int timeBetweenGroundChecks = 8;
    [SerializeField] float distanceToBeConsideredCloseToPlayer = 5f;

    int groundCheckCounter = 0;

    LayerMask terrainMask;

    Projectile knife;
    public void InitProjectile(Transform e, Projectile knife)
    {
        base.InitProjectile(e);
        clampModifier.teleportPerformed.AddListener(OnTeleport);
        groundCheckCounter = timeBetweenGroundChecks;
        terrainMask = LayerMask.GetMask("Default");

        this.knife = knife;
    }

    void OnTeleport()
    {
        animator.SetTrigger("TeleportPerformed");
    }
    private void FixedUpdate()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Armature_Banshee Spawn")) return; //no processing during spawn animation
        UpdateModifiers();
        groundCheckCounter--;
        if (groundCheckCounter <= 0)
        {
            groundCheckCounter = timeBetweenGroundChecks;
            animator.SetBool("IsGrounded", Physics.Raycast(groundChecker.bounds.center, Vector3.down, groundChecker.bounds.extents.y + 0.1f, terrainMask));
        }
        if (Vector3.Distance(transform.position, enemy.transform.position) < distanceToBeConsideredCloseToPlayer) animator.SetTrigger("PlayerNear");
        
        if (Vector3.Distance(transform.position, knife.transform.position) < distanceToBeConsideredCloseToPlayer && knife.currentState == Projectile.ProjectileState.Flying)
        {
            knife.CastProjectile(enemy.transform.position - transform.position);
            animator.SetTrigger("StruckByKnife");
        }
    }
}