using UnityEngine;

public class ShadowMainCamScript : MonoBehaviour
{
    public Camera mainCam, shadowmapCam;
    public RenderTexture shadowmapRenderTex;
    void OnEnable()
    {
        
    }
    void OnDisable()
    {
        
    }

    void OnDestroy()
    {
        
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // shadowmapCam = ShadowmapCamScript.instance.cam;
        // RenderTexture targetDepthTex = new RenderTexture(shadowmapRenderTex.width, shadowmapRenderTex.height, 16, RenderTextureFormat.Depth);
        // shadowmapRenderTex.descriptor = new RenderTextureDescriptor(shadowmapRenderTex.width, shadowmapRenderTex.height, RenderTextureFormat.Depth, 16);
        // shadowmapRenderTex.Create();
        // shadowmapCam.SetTargetBuffers(null, shadowmapRenderTex.colorBuffer);
        // shadowmapCam.Render();

        // ShadowmapCamScript.instance.SetTexture(targetDepthTex);
    }


}
