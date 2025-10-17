using System;
using System.Text.Json.Serialization;
using UnityEngine;

[Serializable]
public struct SongInfo
{
    public string title;
    public float duration;
    public float bpm;
    public float partyLevel;
    public bool hasMinigame;
    public StageInfo linkedStage;
    public int databaseID;

    public static SongInfo None => new SongInfo()
    {
        title = null,
        bpm = 0f,
        partyLevel = 0f,
        hasMinigame = false,
        linkedStage = StageInfo.None
    };

    public override bool Equals(object obj)
    {
        return obj is SongInfo && this == (SongInfo)obj;
    }

    public override int GetHashCode()
    {
        return databaseID;
    }

    public static bool operator ==(SongInfo left, SongInfo right)
        => left.title == right.title && left.duration == right.duration && left.bpm == right.bpm && left.partyLevel == right.partyLevel && left.hasMinigame == right.hasMinigame && left.linkedStage == right.linkedStage;

    public static bool operator !=(SongInfo left, SongInfo right)
        => left.title != right.title || left.duration != right.duration || left.bpm != right.bpm || left.partyLevel != right.partyLevel || left.hasMinigame != right.hasMinigame || left.linkedStage != right.linkedStage;

    [JsonPropertyName("title")] public string Title { get => title; set => title = value; }
    [JsonPropertyName("duration")] public float? Duration { get => duration; set => duration = value.HasValue ? value.Value : float.NaN; }
    [JsonPropertyName("bpm")] public float? BPM { get => bpm; set => bpm = value.HasValue ? value.Value : float.NaN; }
    [JsonPropertyName("zgg")] public float? PartyLevel { get => partyLevel; set => partyLevel = value.HasValue ? value.Value : float.NaN; }
    [JsonPropertyName("minigame")] public bool? HasMinigame { get => hasMinigame; set => hasMinigame = value.HasValue ? value.Value : false; }
    [JsonPropertyName("stage")] public StageInfo? LinkedStage { get => linkedStage; set => linkedStage = value.HasValue ? value.Value : StageInfo.None; }
    [JsonPropertyName("id")] public int? DatabaseID { get => databaseID; set => databaseID = value.HasValue ? value.Value : -1; }
}