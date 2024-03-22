using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StageLoaderConfig", menuName = "Config/Stage Loader")]
public class StageLoaderConfig : JsonAsset<StageLoaderConfigData>
{
    public override StageLoaderConfigData Data { get => data; set => data = value; }
    [SerializeField] private StageLoaderConfigData data;
}

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
}