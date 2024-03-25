using System;
using UnityEngine;

[Serializable]
public struct StageLoaderConfigData
{
    [Serializable]
    public struct StageSceneInfo
    {
        public string stageName;
        public string sceneName;
    }

    public StageSceneInfo[] stageScenes;

    public string GetScene(string stageName) => stageScenes != null ?
        Array.Find(stageScenes, s => s.stageName == stageName).sceneName : null;
}

[CreateAssetMenu(fileName = "StageLoaderConfig", menuName = "Config/Stage Loader")]
public class StageLoaderConfig : JsonAsset<StageLoaderConfigData>
{
    public override StageLoaderConfigData Data { get => data; set => data = value; }
    [SerializeField] private StageLoaderConfigData data;

    public string GetScene(string stageName) => data.GetScene(stageName);
}