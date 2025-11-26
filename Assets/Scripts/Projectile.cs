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
    
    [Header("RecallData")]
    [SerializeField] private Vector3 embeddedPos;
    [SerializeField] private AnimationCurve recallAnimationCurve;
    public float baseRecallSpeed = 50f; // Overall speed multiplier
    public float maxBoostDistance = 180f; // Distance threshold for speed boost
    public float maxSpeedBoost = 3f;      // How much to speed up (3x = 300% speed)
    public float recallCurrentTime { get; private set; }
    public float totalRecallTime { get; private set; }

    private float totalRecallDistance;
    private float recallProgress;
    
    private Rigidbody rb;
    private Collider col;
    
    private Transform embedParent;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        ReturnToIdle();
    }
    
    void Update()
    {
        if (playerTransform != null)
        {
            TryPickUp();
        }

        if(currentState == ProjectileState.Recalling)
        {
            RecallUpdate();
        }
    }

    public void SetState(ProjectileState newState)
    {
        currentState = newState;
        
        switch (currentState)
        {
            case ProjectileState.Idle:
                rb.isKinematic = true;
                col.enabled = true;
                col.isTrigger = false;
                break;
            case ProjectileState.Flying:
                rb.isKinematic = false;
                col.isTrigger = false;
                break;
            case ProjectileState.Embedded:
                rb.isKinematic = true;
                col.isTrigger = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                SetEmbeddedPos();
                break;
            case ProjectileState.Recalling:
                rb.isKinematic = false;
                col.isTrigger = true;
            
                // Calculate total distance
                totalRecallDistance = Vector3.Distance(transform.position, initProjectilePosition.position);
                recallProgress = 0f;
            
                // Estimate total time by simulating the journey
                // This ensures parry timing works correctly
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
    
    private float CalculateEstimatedRecallTime(float distance)
    {
        // Sample the curve at several points to estimate total time
        float estimatedTime = 0f;
        int samples = 10;
    
        for (int i = 0; i < samples; i++)
        {
            float progress = (float)i / samples;
            float speedMultiplier = recallAnimationCurve.Evaluate(progress);
            float segmentTime = (distance / samples) / (baseRecallSpeed * speedMultiplier);
            estimatedTime += segmentTime;
        }
    
        return estimatedTime;
    }
    
    void RecallUpdate()
    {
        if (currentState != ProjectileState.Recalling)
            return;

        // Update timing for parry window
        recallCurrentTime -= Time.deltaTime;

        // Calculate current distance and progress (0 to 1)
        float currentDistance = Vector3.Distance(transform.position, initProjectilePosition.position);
        recallProgress = 1f - (currentDistance / totalRecallDistance);
    
        // Get speed multiplier from curve (this is where the speed variations happen)
        float speedMultiplier = recallAnimationCurve.Evaluate(recallProgress);
    
        // Calculate current speed and move
        float currentSpeed = baseRecallSpeed * speedMultiplier;
        transform.position = Vector3.MoveTowards(transform.position, initProjectilePosition.position, currentSpeed * Time.deltaTime);
    
        // Rotate towards hand
        Vector3 directionToHand = (initProjectilePosition.position - transform.position).normalized;
        if (directionToHand != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToHand) * Quaternion.Euler(-90, 180, 0);
        }
    
        // Check if we've reached the hand
        if (currentDistance < 0.1f)
        {
            ReturnToIdle();
        }
    }



    public void SetEmbeddedPos(){
        embeddedPos = transform.position;
    }

    void OnCollisionEnter(Collision collision) //needs fixing. embedding doesn't work properly
    {
        if (currentState != ProjectileState.Flying || currentState == ProjectileState.Recalling)
            return;

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
        SetState(ProjectileState.Flying);
        
        rb.linearVelocity = direction * speed;
        
        CancelInvoke(nameof(ReturnToIdle));
        Invoke(nameof(ReturnToIdle), lifetime);
    }

    private void ReturnToIdle()
    {
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
    
    public void TryPickUp()
    {
        if (currentState != ProjectileState.Embedded)
            return;
        
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        if (distance <= pickUpRadius)
        {
            // SetState(ProjectileState.PickedUp);
            
            rb.isKinematic = true;
            col.enabled = false;
            gameObject.SetActive(false);
            
            // Instantly return to hand + idle state
            transform.SetParent(initProjectilePosition);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        
            gameObject.SetActive(true);
            SetState(ProjectileState.Idle);
        }
    }
}
