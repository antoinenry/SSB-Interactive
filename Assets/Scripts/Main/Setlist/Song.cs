using System;
using System.Text.Json.Serialization;
using UnityEngine;

[Serializable]
public class SongWebInstance
{
    private int id;
    private SongInfo info;

    [JsonPropertyName("id")] public int ID { get => id; set => id = value; }
    [JsonPropertyName("title")] public string Title { get => info.Title; set => info.Title = value; }
    //[JsonPropertyName("duration")] public float Duration { get => info.Duration; set => info.Duration = value; }
    //[JsonPropertyName("bpm")] public float BPM { get => info.BPM; set => info.BPM = value; }
    //[JsonPropertyName("zgg")] public float PartyLevel { get => info.PartyLevel; set => info.PartyLevel = value; }
    //[JsonPropertyName("minigame")] public string HasMinigame { get => info.HasMinigame.ToString(); set => info.HasMinigame = value == true.ToString(); }
    // [JsonPropertyName("stage")] public StageInfo LinkedStage { get => info.LinkedStage; set => info.LinkedStage = value; }
    //[JsonPropertyName("position")] public int SetlistPosition { get => info.SetlistPosition; set => info.SetlistPosition = value; }

    public static implicit operator SongInfo(SongWebInstance instance) => instance != null ? instance.info : new SongInfo();
}

[Serializable]
public struct SongInfo
{
    [SerializeField] private string title;
    [SerializeField] private float duration;
    [SerializeField] private float bpm;
    [SerializeField] private float partyLevel;
    [SerializeField] private bool hasMinigame;
    [SerializeField] private StageInfo linkedStage;

    public string Title { get => title; set => title = value; }
    public float Duration { get => duration; set => duration = value; }
     public float BPM { get => bpm; set => bpm = value; }
    public float PartyLevel { get => partyLevel; set => partyLevel = value; }
    public bool HasMinigame { get => hasMinigame; set => hasMinigame = value; }
    public StageInfo LinkedStage { get => linkedStage; set => linkedStage = value; }
}