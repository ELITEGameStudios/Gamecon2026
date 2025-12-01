using UnityEngine;

public abstract class EntityBase : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public float normalizedHealth => health/maxHealth;
    public string entityName;
    
    void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        health = maxHealth;
    }

    protected virtual void OnDeath()
    {
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
