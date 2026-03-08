using UnityEngine;
using UnityEngine.VFX;

public class KnifeParticleManager : MonoBehaviour
{
    [SerializeField] ParticleSystem terrainCollision;
    [SerializeField] ParticleSystem enemyCollision;
    [SerializeField] ParticleSystem windExplosion;
    [SerializeField] ParticleSystem parryWindSpiral;
    [SerializeField] ParticleSystem shockwaveExplosion;
    [SerializeField] VisualEffect throwTrailEffect;
    public void InitParticleManager(Projectile knife, Transform player)
    {
        knife.enemyStruck.AddListener(OnEnemyCollision);
        knife.terrainStruck.AddListener(OnTerrainCollision);
        knife.projectileAbilities.firedKnife.AddListener(OnKnifeFired);
        terrainCollision.Stop();
        enemyCollision.Stop();


        foreach (var particle in GetComponentsInChildren<ParticleSystem>())
        {
            var main = particle.main;
            main.loop = false;
            particle.Stop();
        }
    }
     
    void OnKnifeFired(KnifeThrowInfo throwInfo)
    {
        if (throwInfo.parried) PlayParryFireEffects();
        else PlayStandardFireEffects(); 
    }

    void PlayParryFireEffects()
    {
        parryWindSpiral.transform.position = transform.position;
        parryWindSpiral.Play();
    }

    void PlayStandardFireEffects()
    {
       if (throwTrailEffect != null) throwTrailEffect.Play();
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
