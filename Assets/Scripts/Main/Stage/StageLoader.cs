using System;
using UnityEngine;

[ExecuteAlways]
public class StageLoader : MonoBehaviour
{
    public string defaultStage = "Logo";
    public Stage[] stages;
    public GameObject pauseGUI;
    public MainScore mainScore;

    public Stage LoadedStage { get; private set; }
    public StageNamingConfig Config => CurrentAssetsManager.GetCurrent<StageNamingConfig>();

    public string[] StageNames => stages != null ? Array.ConvertAll(stages, s => s?.name) : new string[0];
    public string LoadedStageLocalName => LoadedStage?.name;
    public string LoadedStageServerName => Config?.GetServerName(LoadedStageLocalName);

    private void OnEnable()
    {
        if (ConcertAdmin.HasInstance)
        {
            ConcertAdmin currentConcert = ConcertAdmin.Current;
            if (currentConcert != null)
            {
                LoadStage(serverStageName: currentConcert.state.Stage.name);
                ConcertAdmin.Current.onStateUpdate.AddListener(OnConcertStateUpdate);
            }
        }        
    }

    private void OnDisable()
    {
        if (ConcertAdmin.HasInstance) ConcertAdmin.Current.onStateUpdate.RemoveListener(OnConcertStateUpdate);
        LoadStage(null);
    }

    private void OnConcertStateUpdate(ConcertState state)
    {
        StageInfo stageUpdate = state.Stage;
        if (LoadedStage == null || LoadedStage.name != stageUpdate.name) LoadStage(serverStageName: stageUpdate.name);
    }

    public void LoadStage(string localStageName = null, string serverStageName = null, int moment = 0)
    {
        // Set stage
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
        // Set moment
        if (LoadedStage) LoadedStage.Moment = moment;
    }

    public bool Pause
    {
        get => LoadedStage ? LoadedStage.Pause : false;
        set
        {
            if (LoadedStage)
            {
                LoadedStage.Pause = value;
                pauseGUI?.SetActive(value);
            }
        }
    }

    private void LoadStageFromLocalName(string localStageName)
    {
        mainScore.DisplayScore = false;
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
        if (stagePrefab != null)
        {
            LoadedStage = Instantiate(stagePrefab, transform);
            LoadedStage.onStageEnd.AddListener(OnStageEnd);
        }
        else
        {
            LoadedStage = null;
        }
    }

    private void OnStageEnd()
    {
        if (LoadedStage.HasScore)
        {
            mainScore.DisplayScore = true;
            mainScore.RegisterStageScore();
        }
        else
            mainScore.DisplayScore = false;
    }
}
