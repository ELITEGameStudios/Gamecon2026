using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    public Player player;
    public GameObject cinematics, ui;
    public Canvas uiCanvas;
    public ProjectileGlowManager cinematicGlowManager, externalGlowManager;

    public bool inCinematic { get; private set; } = false;

    public void PreCutscene()
    {
        inCinematic = true;
    }
    public void PostCutscene()
    {
        uiCanvas.enabled = true;
        player.gameObject.SetActive(true);
        cinematics.SetActive(false);
        inCinematic = false;
    }
}
