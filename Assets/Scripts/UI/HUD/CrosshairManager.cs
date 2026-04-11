using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    const int UPDATE_RATE = 10;


    [SerializeField] int raycastRange = 3000;
    [SerializeField] Color highlightColor = Color.red;
    [SerializeField] Color baseColor = Color.white;
    [SerializeField] Image crosshairImage;

    int updateTracker = UPDATE_RATE;

    LayerMask raycastMask;

    private void Start()
    {
        raycastMask = LayerMask.GetMask("Default", "Wall", "Ground", "Shield", "EnemyHurtbox"); //cast for hurtbox because that determines the hit, not collision box
    }
    private void Update()
    {
        updateTracker--;

        if (updateTracker <= 0 && crosshairImage.enabled)
        {
            updateTracker = UPDATE_RATE;
            if (Camera.main == null) return;
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            var Raycast = Physics.Raycast(ray, out RaycastHit hit, raycastRange, raycastMask, QueryTriggerInteraction.Collide);
            if (hit.collider == null) crosshairImage.color = baseColor;
            else
            {
                crosshairImage.color = hit.collider.CompareTag("Enemy") ? highlightColor : baseColor;
            }
        }
    }

    public void ToggleCrosshair(bool status)
    {
        crosshairImage.enabled = status;
    }
}
