using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSystem : MonoBehaviour
{
    [SerializeField] private List<Scene> scenes;
    [SerializeField] private Scene activeScene;  
    [SerializeField] private string gameSceneName, menusSceneName; 
    [SerializeField] private float flexibleTransitionTime = 1.5f; 
    [SerializeField] private AsyncOperation sceneLoadOperation;
    public static SceneSystem Instance { get; private set; }
    
    void Awake(){
        if(Instance == null) {Instance = this;}
        else if(Instance != this) {Destroy(this);}
        
        DontDestroyOnLoad(gameObject);

    }

    void Start(){
    }

    public void GameInitializationFunction(){
        StartCoroutine(GameInitializationCoroutine());
    }

    public void MenusInitializationFunction(){
        SceneManager.LoadScene(menusSceneName);
    }

    public void UpdateSceneData(){
        scenes.Clear();
        activeScene = SceneManager.GetActiveScene();
        for (int i = 0; i < SceneManager.sceneCount; i++) { scenes.Add(SceneManager.GetSceneAt(i)); }
    }

    public void LoadAndUnloadOperation(string loadSceneName, string unloadSceneName){
        StartCoroutine(LoadAdditiveCoroutine(loadSceneName));

        for (int i = 0; i < scenes.Count; i++)
        {
            if(scenes[i].name == unloadSceneName){
                StartCoroutine(UnloadSceneCoroutine(scenes[i]));
                scenes.RemoveAt(i);
            }
        }
    }

    public void AddScene(string sceneName){
        StartCoroutine(LoadAdditiveCoroutine(sceneName));
    }

    public void LoadMainMenu(){
        SceneManager.LoadScene(2);
        UpdateSceneData();
    }

    IEnumerator LoadAdditiveCoroutine(string sceneName, bool newActive = false){
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        float timer = 0;
        float time = Time.realtimeSinceStartup;
        while (!operation.isDone){
            timer = time - Time.realtimeSinceStartup;
            if(timer >= 5){
                Debug.Log("This code is ass. Session Terminated.");
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                yield break;
            }
            yield return null;
        }
        
        UpdateSceneData();
    }

    public void UnloadScene(string sceneName){
        
        for (int i = 0; i < scenes.Count; i++)
        {
            if(scenes[i].name == sceneName){
                StartCoroutine(UnloadSceneCoroutine(scenes[i]));
                scenes.RemoveAt(i);
            }
        }
    }
    IEnumerator UnloadSceneCoroutine(Scene scene){
        AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
        while (!operation.isDone){
            yield return null;
        }

        UpdateSceneData();
    }

    IEnumerator GameInitializationCoroutine(){
        // yield return StartCoroutine(LoadAdditiveCoroutine(flexibleLoadingSceneName));
        // loadingScene = SceneManager.GetSceneByName(flexibleLoadingSceneName);

        // Fade into loading screen
        LoadingScreen.Instance.TriggerScreen(true, flexibleTransitionTime);
        Scene menusScene = SceneManager.GetActiveScene();

        // Load and unload scenes
        yield return new WaitForSecondsRealtime(flexibleTransitionTime);
        // LoadingScreen.Instance.ToggleKnife(true);
        yield return StartCoroutine(LoadAdditiveCoroutine(gameSceneName)); 
        
        Scene gameScene = SceneManager.GetSceneByName(gameSceneName);
        SceneManager.SetActiveScene(gameScene);

        // yield return new WaitForSecondsRealtime(flexibleTransitionTime);
        yield return StartCoroutine(UnloadSceneCoroutine(menusScene)); 
        
        // Fade out of loading scene
        // LoadingScreen.Instance.ToggleKnife(false);
        // yield return new WaitForSecondsRealtime(flexibleTransitionTime);
        LoadingScreen.Instance.TriggerScreen(false, 0);

        
        yield return null;
    }
}
