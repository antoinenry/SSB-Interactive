using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StageNamingConfig", menuName = "Config/Stage Naming")]
public class StageNamingConfig : JsonAsset<StageNamingConfigData>
{
    public override StageNamingConfigData Data { get => data; set => data = value; }
    [SerializeField] private StageNamingConfigData data;

    [CurrentToggle] public bool isCurrent;

    private void OnEnable() => FindMissingStages();
    private void OnValidate() => FindMissingStages();

    public string GetLocalName(string nameOnServer)
        => data.stages != null ? Array.Find(data.stages, s => s.serverName == nameOnServer).localName : null;
    public string GetServerName(string localName)
        => data.stages != null ? Array.Find(data.stages, s => s.localName == localName).serverName : null;

    public int StageCount => data.stages != null ? data.stages.Length : 0;

    public string[] GetAllLocalNames() => data.stages != null ? Array.ConvertAll(data.stages, s => s.localName) : new string[0];
    public string[] GetAllServerNames() => data.stages != null ? Array.ConvertAll(data.stages, s => s.serverName) : new string[0];

    public void FindMissingStages()
    {
        StageLoader loader = FindObjectOfType<StageLoader>(true);
        if (loader == null) return;
        string[] stagesInConfig = GetAllLocalNames();
        string[] stagesInLoader = loader.StageNames;
        data.missingStages = Array.FindAll(stagesInConfig, configStageName => Array.IndexOf(stagesInLoader, configStageName) == -1);
        data.otherStages = Array.FindAll(stagesInLoader, loaderStageName => Array.IndexOf(stagesInConfig, loaderStageName) == -1);
    }

    public void AddStage(string nameOnServer = null, string localName = null) => data.AddStage(nameOnServer, localName);
}

[Serializable]
public struct StageNamingConfigData
{
    [Serializable]
    public struct StageInfo
    {
        public string serverName;
        public string localName;
    }

    public StageInfo[] stages;
    [HideInInspector] public string[] missingStages;
    [HideInInspector] public string[] otherStages;

    public void AutoComplete()
    {
        int count = stages != null ? stages.Length : 0;
        for (int i = 0; i < count; i++) if (stages[i].localName == "") stages[i].localName = stages[i].serverName;
    }

    public void AddStage(string nameOnServer = null, string localName = null)
    {
        int count = stages != null ? stages.Length : 0;
        if (count == 0) stages = new StageInfo[1];
        else Array.Resize(ref stages, count + 1);
        stages[count] = new()
        {
            serverName = nameOnServer,
            localName = localName
        };
    }
}