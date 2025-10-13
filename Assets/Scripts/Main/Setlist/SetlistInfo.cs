using System;
using System.Text.Json.Serialization;

[Serializable]
public struct SetlistInfo
{
    public int id;
    public string name;
    public bool isTemplate;
    public SongInfo[] songs;

    [JsonPropertyName("id")] public int ID { get => id; set => id = value; }
    [JsonPropertyName("name")] public string Name { get => name; set => name = value; }
    [JsonPropertyName("isTemplate")] public bool IsTemplate {  get => isTemplate; set => isTemplate = value; }
    [JsonPropertyName("setlistSongs")] public SongInfo[] Songs { get => songs; set => songs = value; }
  }