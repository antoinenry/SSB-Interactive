using System;
using System.Text.Json.Serialization;

[Serializable]
public struct ConcertInfo
{
    [JsonPropertyName("date")] public string Date { get => date; set => date = value; }
    [JsonPropertyName("location")] public string Location { get => location; set => location = value; }
    [JsonPropertyName("name")] public string Name { get => name; set => name = value; }
    [JsonPropertyName("setlist")] public SetlistInfo Setlist { get => setlist; set => setlist = value; }

    public string date;
    public string location;
    public string name;
    public SetlistInfo setlist;

    public string GetLog()
    {
        return "date : " + date + " / location : " + location + " / name : " + name;
    }
}