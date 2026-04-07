using UnityEngine;

public class Player : EntityBase
{
    public static Player instance {get; private set;}

    public Rigidbody mainRb;
    public EntityDetectionSystem entityDetectionSystem {get; private set;}


    //lets the property be seralized
    [SerializeField] EntityDetectionSystem feds;

    [SerializeField] float invincibilityDuration = 0.5f;
    public int deathsCount;

    float invincibilityTimer = 0f;
    protected override void Init()
    {
        if(instance == null){ instance = this; }
        else if(instance != this){ Destroy(this); }
        entityDetectionSystem = feds;
        if (entityDetectionSystem == null) entityDetectionSystem = GetComponent<EntityDetectionSystem>();
        
        entityName = "The Yellow Runner";
        base.Init();

    }

    protected override void OnDeath()
    {
        health = maxHealth;
        // Put all your death event code here BEFORE base.Death()
        entityDeath.start();
        deathsCount++;

        // Will Handle the destruction of the object. 
        // If you dont want the object to dissappear or want to "delay" its true death for an animation or smth, consider Invoke() or coroutines to delay it
        // Or call it somewhere else and delete this reference.

        // base.OnDeath(); ( Commented out for now, i dont think we intend to delete the player yet... )
        entityKilled.Invoke(this);
        Debug.Log("Player " + name + " died");
    }

    // You can choose to process your code before or after health is updated. Be advised that processing code AFTER may not run if the object dies through this damage event
    public override void Damage(int damage = 1)
    {
        // Before damage event is internally processed 
        if (invincibilityTimer > 0) return;
        base.Damage(damage);
        // after damage event is internally processed 


    }

    public void OnRespawn()
    {
        invincibilityTimer = invincibilityDuration;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
    }

}

