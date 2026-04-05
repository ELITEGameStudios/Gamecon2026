using UnityEngine;

[ExecuteAlways]
public class ShadowmapCamScript : MonoBehaviour
{
    public Camera cam;
    [SerializeField] private Matrix4x4 latestMatrix;
    [SerializeField] private Material[] materials;
    [SerializeField] private RenderTexture texture;

    public static ShadowmapCamScript instance {get; private set;}
    void Awake()
    {
        if(instance == null){instance = this;}
        else if(instance != this){Destroy(this);}
    }

    public void SetTexture(RenderTexture texture)
    {
        this.texture = texture;
        foreach (Material material in materials)
        {
            if(material != null)
            {
                material.SetTexture("_ShadowmapTex", texture);
                material.SetFloat("_OrthoSize", cam.orthographicSize);
            }
        }
    }
    
    public void SetPos()
    {
        foreach (Material material in materials)
        {
            if(material != null)
            {
                material.SetVector("_ShadowmapCamPos", cam.transform.position);
            }
        }
    }

    void Update()
    {

        // if(latestMatrix != transform.worldToLocalMatrix)
        // {
            latestMatrix = transform.worldToLocalMatrix;
            
            foreach (Material material in materials)
            {
                if(material != null)
                {
                    // material.SetFloat("", transform.worldToLocalMatrix);
                    material.SetMatrix("_ShadowmapMatrix", Matrix4x4.Rotate(transform.rotation));
                    material.SetVector("_ShadowmapCamPos", cam.transform.position);
                }
            }
            
        // }
    }
}
