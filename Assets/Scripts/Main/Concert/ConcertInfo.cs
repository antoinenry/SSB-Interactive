using System;
using System.Text.Json.Serialization;
using UnityEngine;

[Serializable]
public struct ConcertInfo
{
    [SerializeField] private string date;
    [SerializeField] private string location;
    [SerializeField] private string name;
    [SerializeField] private SetlistInfo setlist;

    [JsonPropertyName("date")] public string Date { get => date;  set => date = value; }
    [JsonPropertyName("location")] public string Location { get => location; set => location = value; }
    [JsonPropertyName("name")] public string Name { get => name; set => name = value; }
    [JsonPropertyName("setlist")] public SetlistInfo? Setlist { get => setlist; set => setlist = value.HasValue ? value.Value : SetlistInfo.None; }

    public string GetLog()
    {
        return "date : " + date + " / location : " + location + " / name : " + name;
    }
}