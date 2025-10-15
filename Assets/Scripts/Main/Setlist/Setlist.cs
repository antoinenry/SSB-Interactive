using System;
using System.Text.Json.Serialization;
using UnityEngine;

[Serializable]
public class SetlistWebInstance
{
    [Serializable]
    public class SetlistSongWebInstance
    {
        private int id;
        private SongWebInstance song;

        [JsonPropertyName("id")] public int? ID { get => id; set => id = value.HasValue ? value.Value : -1; }
        [JsonPropertyName("song")] public SongWebInstance Song { get => song; set => song = value; }
    }

    private int id;
    private SetlistInfo info;
    private SetlistSongWebInstance[] songInstances;

    [JsonPropertyName("id")] public int? ID { get => id; set => id = value.HasValue ? value.Value : -1; }
    [JsonPropertyName("name")] public string Name { get => info.Name; set => info.Name = value; }
    [JsonPropertyName("isTemplate")] public bool? IsTemplate { get => info.IsTemplate; set => info.IsTemplate = value.HasValue ? value.Value : false; }
    [JsonPropertyName("setlistSongs")] public SetlistSongWebInstance[] Songs
    { 
        get => songInstances;
        set
        {
            songInstances = value;
            info.SetSongs(value != null ? Array.ConvertAll(value, s => s != null ? s.Song : new SongInfo()) : null);
        }
    }

    public static implicit operator SetlistInfo(SetlistWebInstance instance) => instance != null ? instance.info : new SetlistInfo();
}

[Serializable]
public struct SetlistInfo
{
    [SerializeField] private string name;
    [SerializeField] private bool isTemplate;
    [SerializeField] private SongInfo[] songs;

    public string Name { get => name; set => name = value; }
    public bool IsTemplate { get => isTemplate; set => isTemplate = value; }

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