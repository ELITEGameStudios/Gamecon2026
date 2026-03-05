using UnityEngine;

public class KnifeParticleManager : MonoBehaviour
{
    [SerializeField] ParticleSystem terrainCollision;
    [SerializeField] ParticleSystem enemyCollision;
    [SerializeField] ParticleSystem windExplosion;
    [SerializeField] ParticleSystem parryWindSpiral;
    [SerializeField] ParticleSystem shockwaveExplosion;
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
        parryWindSpiral.Play();
        parryWindSpiral.transform.position = knife.transform.position;
    }

     void OnEnemyCollision(Collision collision)
    {
        var contact = collision.GetContact(0);
        PlayParticleAtCollisionPoint(contact, enemyCollision);
        PlayParticleAtCollisionPoint(contact, windExplosion);
        PlayParticleAtCollisionPoint(contact, shockwaveExplosion);
    }
     void OnTerrainCollision(Collision collision)
    {
        var contact = collision.GetContact(0);
        PlayParticleAtCollisionPoint(contact, terrainCollision);
        PlayParticleAtCollisionPoint(contact, shockwaveExplosion, true);
    }
    void PlayParticleAtCollisionPoint(ContactPoint contactPoint, ParticleSystem particle, bool flipAlongNormal = false)
    {
        int flip = flipAlongNormal ? 1 : -1;
        particle.transform.position = contactPoint.point;
        particle.transform.LookAt(contactPoint.normal * flip);
        particle.Play();
    }
}
