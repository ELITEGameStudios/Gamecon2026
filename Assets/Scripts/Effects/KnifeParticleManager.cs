using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class KnifeParticleManager : MonoBehaviour
{
    [SerializeField] ParticleSystem terrainCollision;
    [SerializeField] ParticleSystem enemyCollision;
    [SerializeField] ParticleSystem windExplosion;
    [SerializeField] ParticleSystem parryWindSpiral;
    [SerializeField] ParticleSystem shockwaveExplosion;
    [SerializeField] VisualEffect throwTrailEffect;

    [Header("Impact Decal")]
    [SerializeField] DecalProjector impactDecal;
    [SerializeField] AnimationCurve impactDecalTransparencyOverTime;
    [SerializeField] int numberOfDecals = 8;


    float impactDecalLifetimeTracker = 0.0f;
    Color impactDecalColor;
    public void InitParticleManager(Projectile knife, Transform player)
    {
        knife.enemyStruck.AddListener(OnEnemyCollision);
        knife.terrainStruck.AddListener(OnTerrainCollision);
        knife.projectileAbilities.knifeThrown.AddListener(OnKnifeFired);
        knife.knifeRetrieved.AddListener((retrieveType)=> OnKnifeRetrieved());
        terrainCollision.Stop();
        enemyCollision.Stop();


        foreach (var particle in GetComponentsInChildren<ParticleSystem>())
        {
            var main = particle.main;
            main.loop = false;
            particle.Stop();
        }

        impactDecalColor = impactDecal.material.GetColor("_ImpactColor");
        impactDecal.enabled = false;
    }
     
    void OnKnifeRetrieved()
    {
        impactDecal.enabled = false;
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

        impactDecal.enabled = true;
        int rand = Random.Range(0, numberOfDecals);
        impactDecal.material.SetFloat("_TextureIndex", rand);
        //impactDecal.transform.position = collision.point;
        impactDecalLifetimeTracker = 0.0f;

        Debug.Log("Setting decal to index " + rand);

    }
    void PlayParticleAtCollisionPoint(KnifeCollisionInfo collision, ParticleSystem particle, bool flipAlongNormal = false)
    {
        int flip = flipAlongNormal ? 1 : -1;
        particle.transform.position = collision.point;
        particle.transform.LookAt(collision.normal * flip);
        particle.Play();
    }

    private void Update()
    {
        if (impactDecalLifetimeTracker < 1.0f)
        {
            impactDecalLifetimeTracker += Time.deltaTime;
            float val = impactDecalTransparencyOverTime.Evaluate(impactDecalLifetimeTracker);
            impactDecalColor.a = val;
            impactDecal.material.SetColor("_ImpactColor", impactDecalColor);
        }
    }


}
