using UnityEngine;

public class KnifeParticleManager : MonoBehaviour
{
    [SerializeField] ParticleSystem terrainCollision;
    [SerializeField] ParticleSystem enemyCollision;
    [SerializeField] ParticleSystem windExplosion;
    [SerializeField] ParticleSystem parryWindSpriral;
    public void InitParticleManager(Projectile knife)
    {
        knife.enemyStruck.AddListener(OnEnemyCollision);
        knife.terrainStruck.AddListener(OnTerrainCollision);
        knife.projectileAbilities.knifeParried.AddListener(() => OnKnifeParried(knife));

        terrainCollision.Stop();
        enemyCollision.Stop();


        foreach (var particle in GetComponentsInChildren<ParticleSystem>())
        {
            var main = particle.main;
            main.loop = false;
            particle.Stop();
        }
    }
     
    void OnKnifeParried(Projectile knife)
    {
        parryWindSpriral.Play();
        parryWindSpriral.transform.position = knife.transform.position;
        Debug.Log("Knife parried LETS GOOOOOOOOOOOOOOOOOOOOOO");
    }

     void OnEnemyCollision(Collision collision)
    {
        var contact = collision.GetContact(0);
        PlayParticleAtCollisionPoint(contact, enemyCollision);
        PlayParticleAtCollisionPoint(contact, windExplosion);
    }
     void OnTerrainCollision(Collision collision)
    {
        var contact = collision.GetContact(0);
        PlayParticleAtCollisionPoint(contact, terrainCollision);
    }
    void PlayParticleAtCollisionPoint(ContactPoint contactPoint, ParticleSystem particle)
    {
        particle.transform.position = contactPoint.point;
        particle.transform.LookAt(contactPoint.normal);
        particle.Play();
    }
}
