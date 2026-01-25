using System.Collections.Generic;
using UnityEngine;

public class EnemyList : EnemyBase
{
    [SerializeField] List<EnemyBase> enemyBases;
    protected override void Init()
    {
        maxHealth = 9999;
        base.Init();
    }

    protected override void OnUpdate()
    {
        health = maxHealth;
        for (int i = enemyBases.Count-1; i >= 0; i--)
        {
            try{
                if(enemyBases[i] != null)
                {
                    print("hmmm");
                }
                else
                {
                    enemyBases.RemoveAt(i);
                }
            }
            catch (MissingReferenceException){
                enemyBases.RemoveAt(i);
            }
        }

        if(enemyBases.Count == 0){OnDeath();}
    }
    
}
