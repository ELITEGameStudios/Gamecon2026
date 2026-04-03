using System.Collections.Generic;
using UnityEngine;

public class ParryWingsManager : MonoBehaviour
{
    [SerializeField] Animator leftWingsAnimator;
    [SerializeField] Animator rightWingsAnimator;
    [SerializeField] Projectile knife;

    [SerializeField] Transform leftWingsTransform;
    [SerializeField] Transform rightWingsTransform;

    List<SkinnedMeshRenderer> skinnedMeshes = new();
    List<MeshRenderer> regularMeshes = new();
    private void Start()
    {
        knife.projectileAbilities.attemptedParry.AddListener(OnParryAttempted);
        knife.stateChanged.AddListener(OnParryEnded);
        var leftWingRenderers = leftWingsTransform.GetComponentsInChildren<SkinnedMeshRenderer>();
        var rightWingRenderers = rightWingsTransform.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var renderer in leftWingRenderers) skinnedMeshes.Add(renderer);
        foreach (var renderer in rightWingRenderers) skinnedMeshes.Add(renderer);

        var leftWingRegularRenderers = leftWingsTransform.GetComponentsInChildren<MeshRenderer>();
        var rightWingRegularRenderers = rightWingsTransform.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in leftWingRegularRenderers) regularMeshes.Add(renderer);
        foreach (var renderer in rightWingRegularRenderers) regularMeshes.Add(renderer);
        OnParryEnded(Projectile.ProjectileState.Idle);
    }

    void OnParryAttempted(bool successful)
    {
        if (!successful) return;

        leftWingsAnimator.SetTrigger("ParryPerformed");
        rightWingsAnimator.SetTrigger("ParryPerformed");
        // point wings in the direction the knife is flying
        foreach (var renderer in skinnedMeshes) renderer.enabled = true;
        foreach (var renderer in regularMeshes) renderer.enabled = true;

    }

    void OnParryEnded(Projectile.ProjectileState state)
    {
        if (state == Projectile.ProjectileState.Flying) return; // means it was just fired by the parry, ignore it
        DisableRenderers();
    }

    void DisableRenderers()
    {
        foreach (var renderer in skinnedMeshes) renderer.enabled = false;
        foreach (var renderer in regularMeshes) renderer.enabled = false;
    }





}
