using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CinematicManager : MonoBehaviour
{
    public Player player;
    public GameObject cinematics, ui;
    public Canvas uiCanvas;
    public ProjectileGlowManager cinematicGlowManager, externalGlowManager;


    [SerializeField] PlayableDirector bansheeCutscene;
    [SerializeField] Collider cutsceneTrigger;
    [SerializeField] Camera cinemaCamera;
    public bool inCinematic { get; private set; } = false;

    bool startedBansheeCutscene = false;

    public void PreCutscene()
    {
        player.gameObject.SetActive(false);
        inCinematic = true;
    }
    public void PostCutscene()
    {
        uiCanvas.enabled = true;
        player.gameObject.SetActive(true);
        inCinematic = false;
        cinemaCamera.enabled = false;
        cinematics.SetActive(false);
    }
}
