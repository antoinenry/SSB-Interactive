using System;
using System.Text.Json.Serialization;
using UnityEngine;

[Serializable]
public struct SetlistInfo
{
    [Serializable]
    public struct SetlistSongInfo
    {
        public SongInfo song;
        [JsonPropertyName("song")] public SongInfo? Song { get => song; set => song = value.HasValue ? value.Value : SongInfo.None; }

        public SetlistSongInfo(SongInfo song) { this.song = song; }
    }

    [SerializeField] private string name;
    [SerializeField] private bool isTemplate;
    [SerializeField] private SongInfo[] songs;

    public static SetlistInfo None => new SetlistInfo() { name = null, isTemplate = false, songs = null };

    [JsonPropertyName("name")] public string Name { get => name; set => name = value; }
    [JsonPropertyName("isTemplate")] public bool? IsTemplate { get => isTemplate; set => isTemplate = value.HasValue ? value.Value : false; }

    [JsonPropertyName("setlistSongs")]
    public SetlistSongInfo[] Songs
    {
        get => songs != null ? Array.ConvertAll(songs, s => new SetlistSongInfo(s)) : null;
        set => SetSongs(value != null ? Array.ConvertAll(value, s => s.song) : null);
    }

    public void SetSongs(SongInfo[] songs)
    {
        if (songs == null) this.songs = null;
        else
        {
            int songCount = songs.Length;
            if (this.songs == null) this.songs = new SongInfo[songCount];
            else Array.Resize(ref this.songs, songCount);
            Array.Copy(songs, this.songs, songCount);
        }
    }
}