using UnityEngine;

public class RecallRing : MonoBehaviour
{
    [SerializeField] MeshRenderer recallVFX;
    [SerializeField] float maxExpansion = 2.5f;
    [SerializeField] Player player;
    [SerializeField] Projectile knife;
    [SerializeField, ColorUsage(true, hdr: true)] Color baseColor;
    [SerializeField, ColorUsage(true, hdr: true)] Color parryableColor;

    Vector3 startingScale = Vector3.zero;
    Vector3 maxScale = Vector3.zero;

    float maxKnifeRecallDistance = 1200;

    private void Awake()
    {
        startingScale = recallVFX.transform.localScale;
        maxScale = startingScale * maxExpansion;
        recallVFX.gameObject.SetActive(false);
        knife.projectileAbilities.recallStarted.AddListener(OnRecallStarted);
        knife.knifeRetrieved.AddListener(OnKnifeRetrieved);
    }

    void OnRecallStarted()
    {
        recallVFX.material.SetColor("_EffectColor", baseColor);
        maxKnifeRecallDistance = Vector3.Distance(player.transform.position, knife.transform.position);
        recallVFX.gameObject.SetActive(true);
    }

    void OnKnifeRetrieved(KnifeRetrievalInfo info)
    {
        recallVFX.gameObject.SetActive(false);
    }

    void OnKnifeParried(bool success)
    {
        if (!success) return;
    }

    private void Update()
    {
        if (knife.currentState == Projectile.ProjectileState.Recalling)
        {
            float distance = Vector3.Distance(player.transform.position, knife.transform.position);
            float distanceAsPercent = Mathf.Clamp01(distance / maxKnifeRecallDistance);

            Vector3 newScale = Vector3.Lerp(startingScale, maxScale, distanceAsPercent);
            recallVFX.transform.localScale = newScale;

            if (knife.projectileAbilities.IsParryable()) recallVFX.material.SetColor("_EffectColor", parryableColor);
        }
        else
        {
            if (recallVFX.gameObject.activeSelf) recallVFX.gameObject.SetActive(false);
        }
    }




}
