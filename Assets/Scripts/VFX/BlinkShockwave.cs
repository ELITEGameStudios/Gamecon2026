using UnityEngine;

public class BlinkShockwave : MonoBehaviour
{

    [SerializeField] Projectile knife;
    [SerializeField] Transform playerTransform;
    [SerializeField] MeshRenderer shockwave;
    [SerializeField] Color shockwaveColor;
    [SerializeField] float timeToScale = 0.7f;
    [SerializeField] AnimationCurve scaleXZOverTime;
    [SerializeField] AnimationCurve scaleYOverTime;
    [SerializeField] AnimationCurve alphaOverTime;

    float elaspedTime = 0.0f;

    bool animate = false;


    private void Start()
    {
        knife.knifeRetrieved.AddListener(OnKnifeRetrieved);
        shockwave.gameObject.SetActive(false);
        shockwave.transform.SetParent(null);
    }

    void OnKnifeRetrieved(KnifeRetrievalInfo info)
    {
        if (info.pickupType != KnifeRetrievalType.Blink) return;

        elaspedTime = 0.0f;
        shockwave.gameObject.SetActive(true);
        animate = true;
        shockwave.transform.position = playerTransform.transform.position;
        var rotation = playerTransform.rotation.eulerAngles;
        rotation.y += 180;
        shockwave.transform.rotation = Quaternion.Euler(rotation);
    }

    void EndAnimation()
    {
        animate = false;
        shockwave.gameObject.SetActive(false);
    }

    private void Update()
    { 
        if (animate)
        {
            elaspedTime += Time.deltaTime;
            float animationProgress = elaspedTime / timeToScale;
            if (animationProgress > 1.0f)
            {
                animationProgress = timeToScale;
                EndAnimation();
            }
            float xz = scaleXZOverTime.Evaluate(animationProgress);
            float y = scaleYOverTime.Evaluate(animationProgress);
            shockwave.transform.localScale = new Vector3(xz, y, xz);

            shockwaveColor.a = alphaOverTime.Evaluate(animationProgress);

            shockwave.material.SetColor("_ShockwaveColor", shockwaveColor);


        }
    }
}
