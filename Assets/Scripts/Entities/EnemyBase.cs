using NaughtyAttributes;
using UnityEngine;

public class EnemyBase : EntityBase
{
    public Vector3 targetVector => transform.position - Player.instance.transform.position;
    public Vector3 directionToTarget => targetVector.normalized;
    public float distanceFromTarget => targetVector.magnitude;
    public float baseMovementSpeed = 5;
    public int contactDamage = 0;

    public Collider collider;
    [SerializeField, ShowIf(nameof(IsBulwark))] ShieldEntity enemyShield;


    public EnemyType enemyType { private set; get; } = EnemyType.Dummy;
    [SerializeField] EnemyType type = EnemyType.Grunt;

     public bool IsBulwark() => type == EnemyType.Bulwark;

    /* ---------------------------Template Inherited function documentation------------------------- */
    // Feel free to copy paste these into any new enemy you implement so you can have documentation comments at hand


    // PLEASE call this instead of using unity's Awake() or else things will break 
    protected override void Init()
    {
        base.Init();
        if (collider == null) collider = GetComponent<Collider>();
        enemyType = type;
        // Put your code here

    }
    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    protected override void OnDeath()
    {
        // Put all your death event code here BEFORE base.Death()



        // Will Handle the destruction of the object. 
        // If you dont want the object to dissappear or want to "delay" its true death for an animation or smth, consider Invoke() or coroutines to delay it
        // Or call it somewhere else and delete this reference.
        base.OnDeath();
    }

    // You can choose to process your code before or after health is updated. Be advised that processing code AFTER may not run if the object dies through this damage event
    public override void Damage(int damage = 1)
    {
        // Before damage event is internally processed
        if (enemyShield != null) damage = 0;
        base.Damage(damage);
        // after damage event is internally processed 
    }

    // Handles contact damage against the player
    public override void CollisionEnterEvent(Collision collision)
    {
        if(collision.gameObject == Player.instance.gameObject)
        {
            Player.instance.Damage(contactDamage);
        }
    }
}

[System.Serializable]
public enum EnemyType
{
    Grunt,
    Duelist,
    Bulwark,
    Banshee,
    Dummy
}
