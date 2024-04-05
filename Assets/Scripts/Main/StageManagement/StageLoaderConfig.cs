using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StageLoaderConfig", menuName = "Config/Stage Loader")]
public class StageLoaderConfig : JsonAsset<StageLoaderConfigData>
{
    public override StageLoaderConfigData Data { get => data; set => data = value; }
    [SerializeField] private StageLoaderConfigData data;

    private void OnEnable() => FindMissingStages();
    private void OnValidate() => FindMissingStages();

    public string GetLocalName(string nameOnServer)
        => data.stages != null ? Array.Find(data.stages, s => s.serverName == nameOnServer).localName : null;

    public int StageCount => data.stages != null ? data.stages.Length : 0;

    public string[] GetAllLocalNames() => Array.ConvertAll(data.stages, s => s.localName);
    public string[] GetAllServerNames() => Array.ConvertAll(data.stages, s => s.serverName);

    public void FindMissingStages()
    {
        StageLoader loader = FindObjectOfType<StageLoader>(true);
        if (loader == null) return;
        string[] stagesInConfig = GetAllLocalNames();
        string[] stagesInLoader = loader.StageNames;
        data.missingStages = Array.FindAll(stagesInConfig, configStageName => Array.IndexOf(stagesInLoader, configStageName) == -1);
        data.otherStages = Array.FindAll(stagesInLoader, loaderStageName => Array.IndexOf(stagesInConfig, loaderStageName) == -1);
    }
}

[Serializable]
public struct StageLoaderConfigData
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
}