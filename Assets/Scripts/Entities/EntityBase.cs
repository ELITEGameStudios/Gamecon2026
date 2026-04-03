using UnityEngine;
using UnityEngine.Events;
using static Unity.Collections.Unicode;
public abstract class EntityBase : MonoBehaviour
{

    public int maxHealth = 1;
    [HideInInspector] public int health;
    public float normalizedHealth => health / maxHealth;
    public string entityName;

   [HideInInspector] public UnityEvent<EntityBase> entityKilled = new();
   [HideInInspector] public UnityEvent<EntityBase> entitySpawned = new();

    [Header("FMOD events")]
    public string FMODDeathEvent = "";
    public FMOD.Studio.EventInstance entityDeath;
    public bool hostedByParent; // Used optionally only for instantiating some select entities

    void Awake()
    {
        Init();
    }

    void Update()
    {
        OnUpdate();
    }
    protected virtual void OnUpdate()
    {
        if(FMODDeathEvent != "" && transform.parent != null)
        {
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(entityDeath, transform.parent);
        }
    }


    protected virtual void Init()
    {
        health = maxHealth;
        if(FMODDeathEvent != "")
        {
            entityDeath = FMODUnity.RuntimeManager.CreateInstance(FMODDeathEvent);
        }
    }

    protected virtual void OnDeath()
    {

        health = 0; //prevent negative numbers
        entityDeath.start();

        Debug.Log(entityName + " Has been slain.");

        entityKilled.Invoke(this);

        Destroy(gameObject);


    }

    public void Die()
    {
        OnDeath();
    }

    public virtual void Damage(int damage = 1)
    {
        if (damage <= 0) { return; }
        health -= damage;
        if (health <= 0) { Die(); }
        Debug.Log(entityName + " took damage");
    }

    public virtual void SpawnAtPosition(Vector3 position)
    {
        transform.position = position;
        if (!transform.TryGetComponent(out Rigidbody rb)) return;
        rb.linearVelocity = Vector3.zero; //get rid of gravity built up during falls
        entitySpawned.Invoke(this);
    }

    public virtual void CollisionEnterEvent(Collision collision){}
    
    void OnCollisionEnter(Collision collision){
        CollisionEnterEvent(collision);
    }
}
