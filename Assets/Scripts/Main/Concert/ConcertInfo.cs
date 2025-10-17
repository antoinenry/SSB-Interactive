using System;
using System.Text.Json.Serialization;
using UnityEngine;

[Serializable]
public struct ConcertInfo
{
    public string name;
    public string location;
    public string date;
    public SetlistInfo setlist;

    public static ConcertInfo None => new ConcertInfo()
    {
        date = null,
        location = null,
        name = null,
    };

    public string GetLog()
    {
        return "name : " + name + " / date : " + date + " / location : " + location;
    }

    [JsonPropertyName("name")] public string Name { get => name; set => name = value; }
    [JsonPropertyName("location")] public string Location { get => location; set => location = value; }
    [JsonPropertyName("date")] public string Date { get => date; set => date = value; }
    [JsonPropertyName("setlist")] public SetlistInfo? Setlist { get => setlist; set => setlist = value.HasValue ? value.Value : SetlistInfo.None; }
}