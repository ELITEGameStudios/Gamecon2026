using UnityEngine;

public class StartupScreenHelper : MonoBehaviour
{
    public void OnAnimEnd()
    {
        SceneSystem.Instance.MenusInitializationFunction();
    }
}
