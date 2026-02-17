using System;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    public enum ProjectileState {Idle, Flying, Embedded, Recalling}//, PickedUp}
    public ProjectileState currentState { get; private set; } = ProjectileState.Idle;
    
    [Header("Flying Settings")]
    public float speed = 30f;
    public float lifetime = 5f;
    
    [Header("Embedded Settings")]
    public float pickUpRadius = 2f;
    public float embedDepth = 0.5f;
    public GameObject hitEffect;
    
    [Header("Idle Settings")]
    public Transform initProjectilePosition;
    
    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform;
    public ProjectileAbilities projectileAbilities;
    
    [Header("RecallData")]
    public Vector3 embeddedPos {get; private set;}
    public Vector3 throwDirection {get; private set;}
    [SerializeField] private AnimationCurve recallAnimationCurve;
    public float baseRecallSpeed = 50f; // Overall speed multiplier
    public float maxBoostDistance = 180f; // Distance threshold for speed boost
    public float maxSpeedBoost = 3f;      // How much to speed up (3x = 300% speed)
    public float recallCurrentTime { get; private set; }
    public float totalRecallTime { get; private set; }

    public float totalRecallDistance;
    private float recallProgress;
    
    private Rigidbody rb;
    private Collider col;
    
    [SerializeField] private Animator animator;
    private Transform embedParent;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        ReturnToIdle();
    }
    
    void Update()
    {
        if (playerTransform != null && currentState == ProjectileState.Embedded)
        {   
            float distance = Vector3.Distance(playerTransform.position, transform.position);
            if (distance <= pickUpRadius){ 
                Pickup(); 
            }
        }

        if(currentState == ProjectileState.Recalling)
        {
            RecallUpdate();
        }
        else
        {
            if(HUDManager.Instance.recallElement != null)
            {
                if( projectileAbilities.currentRecallCooldown <= 0 && currentState != ProjectileState.Recalling && currentState != ProjectileState.Idle) {
                    HUDManager.Instance.recallElement.SetFillFactor(1);
                    HUDManager.Instance.recallElement.SetReady(true);
                }
                else
                {
                    HUDManager.Instance.recallElement.SetFillFactor(( projectileAbilities.recallCooldown - projectileAbilities.currentRecallCooldown) / projectileAbilities.recallCooldown);
                    HUDManager.Instance.recallElement.SetReady(false);
                }
            }
        }
    }

    public void SetState(ProjectileState newState)
    {
        ProjectileState oldState = currentState;
        currentState = newState;
        
        switch (currentState)
        {
            case ProjectileState.Idle:
                rb.isKinematic = true;
                col.enabled = true;
                col.isTrigger = false;
                animator.SetTrigger("Idle");
                break;
            case ProjectileState.Flying:
                rb.isKinematic = false;
                col.isTrigger = false;
                
                if(oldState != ProjectileState.Recalling){
                    animator.SetTrigger("Flying");
                }
                break;
            case ProjectileState.Embedded:
                rb.isKinematic = true;
                col.isTrigger = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                animator.SetTrigger("Idle");

                SetEmbeddedPos();
                break;
            case ProjectileState.Recalling:
                // Stop physics immediately
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                col.isTrigger = true;
                animator.SetTrigger("Recalling");
            
                CancelInvoke(nameof(ReturnToIdle));
                Invoke(nameof(ReturnToIdle), lifetime);
            
                // Calculate recall parameters
                totalRecallDistance = Vector3.Distance(transform.position, initProjectilePosition.position);
                recallProgress = 0f;
            
                // Estimate total time
                totalRecallTime = CalculateEstimatedRecallTime(totalRecallDistance);
                recallCurrentTime = totalRecallTime;
                break;
            
            // case ProjectileState.PickedUp:
            //     rb.isKinematic = true;
            //     col.enabled = false;
            //     gameObject.SetActive(false);
            //     break;
        }
    }
    
    void RecallUpdate()
    {
        if (currentState != ProjectileState.Recalling)
            return;

        recallCurrentTime -= Time.deltaTime;

        float currentDistance = Vector3.Distance(transform.position, initProjectilePosition.position);
        recallProgress = 1f - (currentDistance / totalRecallDistance);
        HUDManager.Instance.recallElement.SetSpeed(recallProgress);
    
        // Calculate distance-based speed boost
        float distanceBoost = 1f;
        if (currentDistance > maxBoostDistance)
        {
            // Apply maximum boost when way beyond threshold
            distanceBoost = maxSpeedBoost;
        }
        else if (currentDistance > maxBoostDistance * 0.7f) // Start slowing down at 70% of threshold
        {
            // Gradually reduce boost as it approaches normal range
            float boostProgress = (currentDistance - (maxBoostDistance * 0.7f)) / (maxBoostDistance * 0.3f);
            distanceBoost = Mathf.Lerp(1f, maxSpeedBoost, boostProgress);
        }
    
        // Get curve-based speed multiplier
        float speedMultiplier = recallAnimationCurve.Evaluate(recallProgress);
    
        // Combine both multipliers
        float currentSpeed = baseRecallSpeed * speedMultiplier * distanceBoost;
    
        transform.position = Vector3.MoveTowards(transform.position, initProjectilePosition.position, currentSpeed * Time.deltaTime);
    
        // Rotate towards hand
        Vector3 directionToHand = (initProjectilePosition.position - transform.position).normalized;
        if (directionToHand != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToHand) * Quaternion.Euler(-90, 180, 0);
        }
    
        if (currentDistance < 0.1f)
        {
            ReturnToIdle();
            
        }
    }

    private float CalculateEstimatedRecallTime(float distance)
    {
        // Sample the curve at several points to estimate total time
        float estimatedTime = 0f;
        int samples = 10;

        for (int i = 0; i < samples; i++)
        {
            float progress = (float)i / samples;
            float speedMultiplier = recallAnimationCurve.Evaluate(progress);
        
            // Calculate distance boost for this segment too!
            float segmentDistance = distance * (1f - progress);
            float distanceBoost = CalculateDistanceBoost(segmentDistance);
        
            float segmentTime = (distance / samples) / (baseRecallSpeed * speedMultiplier * distanceBoost);
            estimatedTime += segmentTime;
        }

        return estimatedTime;
    }
    
    private float CalculateDistanceBoost(float currentDistance)
    {
        if (currentDistance > maxBoostDistance)
        {
            return maxSpeedBoost;
        }
        else if (currentDistance > maxBoostDistance * 0.7f)
        {
            float boostProgress = (currentDistance - (maxBoostDistance * 0.7f)) / (maxBoostDistance * 0.3f);
            return Mathf.Lerp(1f, maxSpeedBoost, boostProgress);
        }
        return 1f;
    }

    public void SetEmbeddedPos(){
        embeddedPos = transform.position;
    }

    void OnCollisionEnter(Collision collision) //needs fixing. embedding doesn't work properly
    {
        if (currentState != ProjectileState.Flying || currentState == ProjectileState.Recalling)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (projectileAbilities != null)
            {
                projectileAbilities.ResetRecallCooldown();
            }

        }

        if(collision.transform.GetComponent<EnemyBase>() == null)
        {
            if(collision.transform.parent != null)
            {
                if(collision.transform.parent.GetComponent<EnemyBase>() == null)
                {
                    Debug.Log("Null");
                }
                else
                {
                    collision.transform.parent.GetComponent<EnemyBase>().Damage();
                }
            }
        }
        else
        {
            collision.transform.GetComponent<EnemyBase>().Damage();
        }
        
        if (hitEffect)
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        
        Quaternion incomingRotation = transform.rotation;
        Vector3 travelDirection = rb.linearVelocity.normalized; 

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position += travelDirection * (embedDepth * 0.1f);

        // Restore the original rotation to maintain impact angle
        transform.rotation = incomingRotation;

        SetState(ProjectileState.Embedded);

        CancelInvoke(nameof(ReturnToIdle));
    }
    
    public void CastProjectile(Vector3 direction)
    {
        transform.SetParent(null);
        transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(-90, 180, 0);
        throwDirection = direction;
        SetState(ProjectileState.Flying);
        
        rb.linearVelocity = direction * speed;
        
        CancelInvoke(nameof(ReturnToIdle));
        Invoke(nameof(ReturnToIdle), lifetime);
    }

    private void ReturnToIdle()
    {
        if(HUDManager.Instance != null) HUDManager.Instance.recallElement.Deactivate();
        
        if (currentState != ProjectileState.Flying && currentState != ProjectileState.Recalling)
            return;
        
        SetState(ProjectileState.Idle);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        transform.SetParent(initProjectilePosition);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        gameObject.SetActive(true);
    }
    
    public void OnParry()
    {
        animator.SetTrigger("Parried");
    }

    public void Pickup()
    {   
        rb.isKinematic = true;
        col.enabled = false;
        gameObject.SetActive(false);

        
        // Instantly return to hand + idle state
        transform.SetParent(initProjectilePosition);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    
        gameObject.SetActive(true);
        SetState(ProjectileState.Idle);
        if(HUDManager.Instance != null) HUDManager.Instance.recallElement.Deactivate();
    }
}
