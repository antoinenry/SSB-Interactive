using System;
using System.Text.Json.Serialization;

[Serializable]
public struct SongInfo
{
    public int id;
    public string title;
    public float duration;
    public float bpm;
    public float partyLevel;
    public bool hasMinigame;
    public StageInfo linkedStage;
    public int setlistPosition;

    [JsonPropertyName("id")] public int ID { get => id; set => id = value; }
    [JsonPropertyName("title")] public string Title { get => title; set => title = value; }
    [JsonPropertyName("duration")] public float Duration { get => duration; set => duration = value; }
    [JsonPropertyName("bpm")] public float BPM { get => bpm; set => bpm = value; }
    [JsonPropertyName("zgg")] public float PartyLevel { get => partyLevel; set => partyLevel = value; }
    [JsonPropertyName("minigame")] public bool HasMinigame { get => hasMinigame; set => hasMinigame = value; }
    [JsonPropertyName("stage")] public StageInfo LinkedStage { get => linkedStage; set => linkedStage = value; }
    [JsonPropertyName("position")] public int SetlistPosition { get => setlistPosition; set => setlistPosition = value; }
}