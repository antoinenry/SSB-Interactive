using UnityEngine;
using UnityEngine.SceneManagement;

public class StageLoader : MonoBehaviour
{
    private StageLoaderConfig config;
    private StageLoaderConfigData.StageSceneInfo loaded;

    private void Awake()
    {
        CurrentAssetsManager.GetCurrent(ref config);
    }

    public string Stage
    {
        get => loaded.stageName;
        set => LoadStage(value);
    }

    public string Scene => loaded.sceneName;

    private void LoadStage(string stageName)
    {
        string sceneName = config?.GetScene(stageName);
        LoadScene(sceneName);
        loaded.stageName = stageName;
    }

    private void LoadScene(string sceneName)
    {
        if (loaded.sceneName == sceneName) return;
        if (SceneManager.GetSceneByName(loaded.sceneName).isLoaded)
            SceneManager.UnloadSceneAsync(loaded.sceneName);
        if (sceneName != null)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            loaded.sceneName = sceneName;
        }
        else
            loaded.sceneName = null;
    }
}
