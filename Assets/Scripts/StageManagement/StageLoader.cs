using UnityEngine;
using UnityEngine.SceneManagement;

public class StageLoader : MonoBehaviour
{   
    private StageLoaderConfig config;

    private void Awake()
    {
        CurrentAssetsManager.GetCurrent(ref config);
    }
}
