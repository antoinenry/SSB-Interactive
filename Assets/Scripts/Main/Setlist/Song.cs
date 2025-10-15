using System;
using System.Text.Json.Serialization;
using UnityEngine;

[Serializable]
public class SongWebInstance
{
    private int id;
    private SongInfo info;

    [JsonPropertyName("id")] public int? ID { get => id; set => id = value.HasValue ? value.Value : -1; }
    [JsonPropertyName("title")] public string Title { get => info.Title; set => info.Title = value; }
    [JsonPropertyName("duration")] public float? Duration { get => info.Duration; set => info.Duration = value.HasValue ? value.Value : float.NaN; }
    [JsonPropertyName("bpm")] public float? BPM { get => info.BPM; set => info.BPM = value.HasValue ? value.Value : float.NaN; }
    [JsonPropertyName("zgg")] public float? PartyLevel { get => info.PartyLevel; set => info.PartyLevel = value.HasValue ? value.Value : float.NaN; }
    [JsonPropertyName("minigame")] public bool? Minigame { get => info.HasMinigame; set => info.HasMinigame = value == true; }

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