using UnityEngine;

public class PlayButtonEventHelper : MonoBehaviour
{
    public void PlayFunction()
    {
        SceneSystem.Instance.GameInitializationFunction();
    }
}
