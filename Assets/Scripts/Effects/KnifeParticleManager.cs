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
   //     parryWindSpiral.transform.position = knife.transform.position;
    }

     void OnEnemyCollision(KnifeCollisionInfo collision)
    {
        PlayParticleAtCollisionPoint(collision, enemyCollision);
        PlayParticleAtCollisionPoint(collision, windExplosion);
        PlayParticleAtCollisionPoint(collision, shockwaveExplosion);
    }
     void OnTerrainCollision(KnifeCollisionInfo collision)
    {
        PlayParticleAtCollisionPoint(collision, terrainCollision);
        PlayParticleAtCollisionPoint(collision, shockwaveExplosion, true);
    }
    void PlayParticleAtCollisionPoint(KnifeCollisionInfo collision, ParticleSystem particle, bool flipAlongNormal = false)
    {
        int flip = flipAlongNormal ? 1 : -1;
        particle.transform.position = collision.point;
        particle.transform.LookAt(collision.normal * flip);
        particle.Play();
    }
}
