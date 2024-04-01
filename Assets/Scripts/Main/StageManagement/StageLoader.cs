using SocketIOClient;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageLoader : MonoBehaviour
{
    public string stageEvent = "stage";

    private SocketIOClientScriptable client;
    private StageLoaderConfig config;
    private StageLoaderConfigData.StageSceneInfo loaded;
    [SerializeField] private string loadingStage;

    private void Awake()
    {
        CurrentAssetsManager.GetCurrent(ref client);
        CurrentAssetsManager.GetCurrent(ref config);
    }

    private void OnEnable()
    {
        client.Subscribe(stageEvent, OnClientRequestsStage);
    }

    private void OnDisable()
    {
        client.Unsubscribe(stageEvent, OnClientRequestsStage);
    }

    private void Update()
    {
        LoadStage(loadingStage);
    }

    private void OnClientRequestsStage(string eventName, SocketIOResponse response) => loadingStage = response.GetValue<string>();

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
