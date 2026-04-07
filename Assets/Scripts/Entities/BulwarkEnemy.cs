using UnityEngine;

public class BulwarkEnemy : RangedEnemy
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

    private void FixedUpdate()
    {
        SearchForPlayer();
        animator.SetBool("PlayerNearby", playerNearby);
        animator.SetBool("CanFire", cooldownTracker <= 0.0f);
    }
}
