using UnityEngine;

//public class KnifeParticleManager : MonoBehaviour
//{
//    [SerializeField] ParticleSystem terrainCollision;
//    [SerializeField] ParticleSystem enemyCollision;
//    public void InitParticleManager(Projectile knife)
//    {
//        knife.enemyCollision.AddListener(OnEnemyCollision);
//        knife.terrainCollision.AddListener(OnTerrainCollision);


//        terrainCollision.Stop();
//        enemyCollision.Stop();

//        var main = terrainCollision.main;
//        main.loop = false;

//        main = enemyCollision.main;
//        main.loop = false;
//    }
//    public void OnEnemyCollision(Collision collision)
//    {
//        PlayParticleAtCollisionPoint(collision.GetContact(0), enemyCollision);
//    }
//    public void OnTerrainCollision(Collision collision)
//    {
//        PlayParticleAtCollisionPoint(collision.GetContact(0), terrainCollision);
//    }
//    void PlayParticleAtCollisionPoint(ContactPoint contactPoint, ParticleSystem particle)
//    {
//        particle.transform.position = contactPoint.point;
//        particle.transform.LookAt(contactPoint.normal);
//        particle.Play();
//    }
//}
