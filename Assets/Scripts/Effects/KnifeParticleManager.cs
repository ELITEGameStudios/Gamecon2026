using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class KnifeParticleManager : MonoBehaviour
{
    const int UPDATE_RATE = 6;

    [SerializeField] ParticleSystem terrainCollision;
    [SerializeField] ParticleSystem enemyCollision;
    [SerializeField] ParticleSystem windExplosion;
    [SerializeField] ParticleSystem parryWindSpiral;
    [SerializeField] ParticleSystem shockwaveExplosion;
    [SerializeField] VisualEffect throwTrailEffect;

    [Header("Impact Decal")]
    [SerializeField] List<DecalData> impactDecals;
    [SerializeField] AnimationCurve impactDecalTransparencyOverTime;
    [SerializeField] int decalSpriteCount = 8;
    [SerializeField] float impactExtrusion;
    [SerializeField] Material impactMaterial;

    int currentDecal = 0;

    int updateTracker = 0;

    [System.Serializable]
    class DecalData
    {
        public DecalProjector projector;
       [HideInInspector] public float lifetime;
    }

    public void InitParticleManager(Projectile knife, Transform player)
    {
        knife.enemyStruck.AddListener(OnEnemyCollision);
        knife.terrainStruck.AddListener(OnTerrainCollision);
        knife.projectileAbilities.knifeThrown.AddListener(OnKnifeFired);
        knife.knifeRetrieved.AddListener((retrieveType) => OnKnifeRetrieved());
        terrainCollision.Stop();
        enemyCollision.Stop();


        foreach (var particle in GetComponentsInChildren<ParticleSystem>())
        {
            var main = particle.main;
            main.loop = false;
            particle.Stop();
        }

        for (int i = 0; i < impactDecals.Count; i++)
        {   
            var decal = impactDecals[i];
            decal.projector.enabled = false;
            decal.projector.transform.SetParent(null); // don't follow knife  
        }
    }

    void OnKnifeRetrieved()
    {

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

        int rand = Random.Range(0, decalSpriteCount);
        var decal = impactDecals[currentDecal];
        decal.lifetime = 0.0f;
        decal.projector.enabled = true;
        decal.projector.material.SetFloat("_TextureIndex", rand);
        currentDecal = (currentDecal + 1) % impactDecals.Count;
        decal.projector.transform.position = collision.point + (collision.normal * impactExtrusion);

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

        updateTracker++;
        for (int i = 0; i < impactDecals.Count; i++)
        {
            var decal = impactDecals[i];
            if (!decal.projector.enabled) continue;
            decal.lifetime += Time.deltaTime;
        }
        if (updateTracker >= UPDATE_RATE) 
        {
            updateTracker = 0;
            for (int i = 0; i < impactDecals.Count; i++)
            {
                var decal = impactDecals[i];
                if (!decal.projector.enabled) continue;
                float val = impactDecalTransparencyOverTime.Evaluate(decal.lifetime);
                decal.projector.fadeFactor = val;
            }
        }
    }


}
