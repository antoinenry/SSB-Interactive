using System;
using UnityEngine;

public class StageLoader : MonoBehaviour
{
    public string defaultStage = "Logo";
    public Stage[] stages;

    public Stage LoadedStage { get; private set; }
    public StageLoaderConfig Config => CurrentAssetsManager.GetCurrent<StageLoaderConfig>();

    public string[] StageNames => stages != null ? Array.ConvertAll(stages, s => s?.name) : new string[0];
       
    public void LoadStage(string localStageName = null, string serverStageName = null)
    {
        if (localStageName != null && serverStageName != null)
        {
            if (localStageName != Config.GetLocalName(serverStageName))
            {
                Debug.LogWarning("Stage name mismatch");
                LoadStageFromLocalName(defaultStage);
            }
        }
        else if (serverStageName != null) LoadStageFromLocalName(Config?.GetLocalName(serverStageName));
        else if (localStageName != null) LoadStageFromLocalName(localStageName);
        else LoadStageFromLocalName(defaultStage);
    }

    private void LoadStageFromLocalName(string localStageName)
    {
        Stage[] stageInstances = GetComponentsInChildren<Stage>();
        bool alreadyLoaded = false;
        if (stageInstances != null)
        {
            foreach (Stage instance in stageInstances)
            {
                if (!alreadyLoaded && instance.name == localStageName)
                {
                    alreadyLoaded = true;
                    continue;
                }
                DestroyImmediate(instance.gameObject);
            }
        }
        if (alreadyLoaded) return;
        Stage stagePrefab = stages != null ? Array.Find(stages, s => s?.name == localStageName) : null;
        if (stagePrefab == null) stagePrefab = stages != null ? Array.Find(stages, s => s?.name == defaultStage) : null;
        LoadedStage = stagePrefab != null ? Instantiate(stagePrefab, transform) : null;
    }
}
