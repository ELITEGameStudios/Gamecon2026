using UnityEngine;

public class Dummy : EnemyBase
{


    /* ---------------------------Template Inherited function documentation------------------------- */
    // Feel free to copy paste these into any new enemy you implement so you can have documentation comments at hand


    // PLEASE call this instead of using unity's Awake() or else things will break 
    protected override void Init()
    {
        base.Init();
        entityName = "Dummy";
        Debug.Log("Dummy init called");
    }

    protected override void OnDeath()
    {
        health = maxHealth;
        Debug.Log("Dummy death called");

        // I dont need dummys to die
        // base.OnDeath();
    }

    // You can choose to process your code before or after health is updated. Be advised that processing code AFTER may not run if the object dies through this damage event
    public override void Damage(int damage = 1)
    {
        Debug.Log("Dummy method called");
        base.Damage();
    }
}
