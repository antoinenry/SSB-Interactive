using System;
using System.Text.Json.Serialization;

[Serializable]
public struct ScoreData
{
    [JsonPropertyName("score")] public float StageScore { get => stageScore; set => stageScore = value; }
    [JsonPropertyName("stageTitle")] public string Stage { get => stage; set => stage = value; }

    public float stageScore;
    public string stage;
}