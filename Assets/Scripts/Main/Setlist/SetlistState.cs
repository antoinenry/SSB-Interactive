using System.Text.Json.Serialization;
using System;

[Serializable]
public struct SetlistState
{
    public SongInfo song;
    public int position;

    public static SetlistState None => new SetlistState()
    {
        song = SongInfo.None,
        position = -1
    };

    //public static implicit operator SongInfo(SetlistState s) => s.song;

    [JsonPropertyName("song")] public SongInfo? Song { get => song; set => song = value.HasValue ? value.Value : SongInfo.None; }
    [JsonPropertyName("position")] public int? Position { get => position; set => position = value.HasValue ? value.Value : -1; }
}