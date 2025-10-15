using System;
using System.Text.Json.Serialization;
using UnityEngine;

[Serializable]
public struct SongInfo
{
    [SerializeField] private string title;
    [SerializeField] private float duration;
    [SerializeField] private float bpm;
    [SerializeField] private float partyLevel;
    [SerializeField] private bool hasMinigame;
    [SerializeField] private StageInfo linkedStage;

    public static SongInfo None => new SongInfo()
    {
        title = null,
        bpm = 0f,
        partyLevel = 0f,
        hasMinigame = false,
        linkedStage = StageInfo.None
    };

    [JsonPropertyName("title")] public string Title { get => title; set => title = value; }
    [JsonPropertyName("duration")] public float? Duration { get => duration; set => duration = value.HasValue ? value.Value : float.NaN; }
    [JsonPropertyName("bpm")] public float? BPM { get => bpm; set => bpm = value.HasValue ? value.Value : float.NaN; }
    [JsonPropertyName("zgg")] public float? PartyLevel { get => partyLevel; set => partyLevel = value.HasValue ? value.Value : float.NaN; }
    [JsonPropertyName("minigame")] public bool? HasMinigame { get => hasMinigame; set => hasMinigame = value.HasValue ? value.Value : false; }
    [JsonPropertyName("stage")] public StageInfo? LinkedStage { get => linkedStage; set => linkedStage = value.HasValue ? value.Value : StageInfo.None; }
}