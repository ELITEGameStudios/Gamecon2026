using UnityEngine;
using UnityEngine.VFX;

public class RicochetShockwave : MonoBehaviour
{
    [SerializeField] Projectile knife;
    [SerializeField] VisualEffect sparksVFX;
    void Start()
    {
        knife.knifeRicocheted.AddListener(OnKnifeBounced);
        sparksVFX.transform.SetParent(null);
    }
    void OnKnifeBounced()
    {
        sparksVFX.transform.position = knife.transform.position;
        sparksVFX.Play();
    }
}
