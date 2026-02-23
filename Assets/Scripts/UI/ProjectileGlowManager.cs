using UnityEngine;

[ExecuteAlways]
public class ProjectileGlowManager : MonoBehaviour
{
    [SerializeField] private string emissionString;
    [SerializeField] private Material primaryGlow, secondaryGlow;
    [ColorUsage(true, true)] public Color emissionPrimary, emissionSecondary;



    void Update()
    {
        if(primaryGlow != null) primaryGlow.SetColor(emissionString, emissionPrimary);
        if(primaryGlow != null) secondaryGlow.SetColor(emissionString, emissionSecondary);
    }
}
