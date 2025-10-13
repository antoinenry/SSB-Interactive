using System;
using System.Text.Json.Serialization;

[Serializable]
public struct ConcertInfo
{
    [JsonPropertyName("id")] public int ID { get => id; set => id = value; }
    [JsonPropertyName("date")] public string Date { get => date; set => date = value; }
    [JsonPropertyName("location")] public string Location { get => location; set => location = value; }
    [JsonPropertyName("name")] public string Name { get => name; set => name = value; }

    public int id;
    public string date;
    public string location;
    public string name;

    public string GetLog()
    {
        return "date : " + date + " / location : " + location + " / name : " + name;
    }
}