using UnityEngine;
using UnityEngine.VFX;

public class RicochetVFX : MonoBehaviour
{
    [SerializeField] Projectile knife;
    [SerializeField] VisualEffect sparksVFX;
    void Start()
    {
        knife.knifeRicocheted.AddListener(OnKnifeBounced);
        sparksVFX.transform.SetParent(null);
    }
    void OnKnifeBounced(Vector3 dir)
    {
        sparksVFX.transform.SetPositionAndRotation(knife.transform.position, Quaternion.LookRotation(knife.transform.forward));
        sparksVFX.Play();
    }
}
