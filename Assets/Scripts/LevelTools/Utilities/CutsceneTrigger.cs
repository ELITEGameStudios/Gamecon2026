using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;
public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] Collider cutsceneTrigger;
    [SerializeField] PlayableDirector bansheeCutscene;
    bool startedBansheeCutscene = false;

    [SerializeField] LayerMask playerMask;
    double cutsceneDuration = 0.0f;

    void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
    }
    public void FixedUpdate()
    {
        var overlap = Physics.OverlapBox(cutsceneTrigger.bounds.center, cutsceneTrigger.bounds.extents, cutsceneTrigger.transform.rotation, playerMask);
        if (overlap.Length > 0 && !startedBansheeCutscene)
        {
            startedBansheeCutscene = true;
            PlayBansheeCutscene();
        }
        if (cutsceneDuration > 0.0f && startedBansheeCutscene)
        {
            cutsceneDuration -= Time.fixedDeltaTime;
            if (cutsceneDuration <= 0)
            {
                OnCutsceneOver();
            }
        }
    }
    public void OnCutsceneOver()
    {
        SceneManager.LoadScene("UpperLevelBlockout");
        GameManager.Instance.ManualStartWaveSystem();
    }

    void PlayBansheeCutscene()
    {
        Player.instance.gameObject.SetActive(false);
        bansheeCutscene.Play();
        cutsceneDuration = bansheeCutscene.duration;
    }



    

}
