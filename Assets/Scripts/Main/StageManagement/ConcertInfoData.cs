using System;
using System.Text.Json.Serialization;

[Serializable]
public struct ConcertInfoData
{
    [JsonPropertyName("date")] public string Date { get => date; set => date = value; }
    [JsonPropertyName("location")] public string Location { get => location; set => location = value; }
    [JsonPropertyName("name")] public string Name { get => name; set => name = value; }

    public string date;
    public string location;
    public string name;
}