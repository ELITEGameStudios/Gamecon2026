using UnityEngine;
using UnityEngine.Events;

public class ToggleTutorial : MonoBehaviour
{
    public UnityEvent openEvent, closeEvent;
    bool open = true;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (open)
            {
                closeEvent.Invoke();
            }
            else
            {
                openEvent.Invoke();
            }
            open = !open;
        }
    }
}
