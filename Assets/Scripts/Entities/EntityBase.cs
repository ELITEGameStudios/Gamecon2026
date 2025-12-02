using UnityEngine;

public abstract class EntityBase : MonoBehaviour
{
    public int maxHealth = 1;
    public int health;
    public float normalizedHealth => health/maxHealth;
    public string entityName;
    

    [Header("FMOD events")]
    public string FMODDeathEvent = "";
    public FMOD.Studio.EventInstance entityDeath;

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
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(entityDeath, transform.parent);
    }


    protected virtual void Init()
    {
        health = maxHealth;
        entityDeath = FMODUnity.RuntimeManager.CreateInstance(FMODDeathEvent);
    }

    protected virtual void OnDeath()
    {
        
        entityDeath.start();

        Debug.Log(entityName + " Has been slain.");
        
        Destroy(gameObject);
    }

    public virtual void Damage(int damage = 1)
    {
        if(damage <= 0){ return; }

        health -= damage;
        if(health <= 0){OnDeath();}

        Debug.Log(entityName + " took damage");
    }
}
