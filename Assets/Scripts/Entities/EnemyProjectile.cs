using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static LevelData;

public class EnemyProjectile : MonoBehaviour
{
    public struct ProjectileTarget
    {
        public Transform targetTransform;
    }

    [HideInInspector] public UnityEvent<EnemyProjectile> projectileFired;
    [HideInInspector] public UnityEvent<EnemyProjectile> projectileDestroyed;
    [HideInInspector] public UnityEvent<ProjectileTarget> projectileActivated; //transform is target


    [HideInInspector]  public List<ProjectileModifier> projectileModifiers = new();


    public Collider projectileCollider;
    public GameObject meshObjects;
    public Rigidbody rb;
    public Transform enemy;

   bool active = false;

    public bool Active { private set => active = value; get => active; }

    [HideInInspector] public Vector3 projectileSpeed;

    private void Awake()
    {
       if (rb == null)  rb = GetComponent<Rigidbody>();
       if (projectileCollider == null) projectileCollider = rb.GetComponentInChildren<Collider>();
       if (projectileCollider == null)
       {
           Debug.LogError("Could not find projectile collider");
           Destroy(gameObject);
       }
    } 
    public void InitProjectile(Transform e)
    {
        enemy = e;
        var mods = GetComponents<ProjectileModifier>();
        foreach (var mod in mods)
        {
            projectileModifiers.Add(mod);
            mod.InitModifier(this);
        }
        projectileModifiers = projectileModifiers.OrderByDescending(x => x.priority).ToList();
    }

    public virtual void DestroyProjectile()
    {
        projectileCollider.enabled = false;
        meshObjects.SetActive(false);
        projectileDestroyed.Invoke(this);
        active = false;
    }

    private void FixedUpdate()
    {
        foreach (var mod in projectileModifiers)
        {
            mod.UpdateModifier();
        }
        rb.linearVelocity = projectileSpeed;
        if (projectileCollider.enabled) Debug.Log("Active = " + active);
    }

    public void Activate(Transform target, Vector3 spawnPos)
    {
        ProjectileTarget newTarget = new() { targetTransform = target };

        rb.MovePosition(spawnPos);
        projectileActivated.Invoke(newTarget);
        meshObjects.SetActive(true);
        active = true;
    }

    public T GetProjectileModifier<T>() where T : ProjectileModifier
    {
        foreach (var mod in projectileModifiers)
        {
            if (mod is T) return mod as T;
        }
        return null;
    }
}
[System.Serializable]
public class ProjectileFireInformation
{
    public EnemyProjectile projectilePrefab;
    public float delayAfterShot = 0.0f;
    /// <summary>
    /// if true, use the offsetTransform's position as the offset from the enemy's position.
    /// </summary>
    public bool useTransformForOffset = false;

    [ShowIf(nameof(RequiresTransformOffset))] public Transform offsetTransform;
    [HideIf(nameof(RequiresTransformOffset))] public Vector3 offset;
    bool RequiresTransformOffset() => useTransformForOffset == true;
}


