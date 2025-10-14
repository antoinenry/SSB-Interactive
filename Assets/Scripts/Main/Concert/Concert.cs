using System;
using System.Text.Json.Serialization;
using UnityEngine;

[Serializable]
public class ConcertWebInstance
{
    private int id;
    private ConcertInfo info;
    private SetlistWebInstance setlistInstance;

    [JsonPropertyName("id")] public int ID {  get => id; set => id = value; }
    [JsonPropertyName("date")] public string Date { get => info.Date; set => info.Date = value; }
    [JsonPropertyName("location")] public string Location { get => info.Location; set => info.Location = value; }
    [JsonPropertyName("name")] public string Name { get => info.Name; set => info.Name = value; }
    [JsonPropertyName("setlist")] public SetlistWebInstance SetlistInstance
    { 
        get => setlistInstance;
        set
        { 
            setlistInstance = value;
            info.Setlist = value;
        }
    }

    public static implicit operator ConcertInfo(ConcertWebInstance instance) => instance != null ? instance.info : new ConcertInfo();
}

[Serializable]
public struct ConcertInfo
{
    [SerializeField] private string date;
    [SerializeField] private string location;
    [SerializeField] private string name;
    [SerializeField] private SetlistInfo setlist;

    public string Date { get => date;  set => date = value; }
    public string Location { get => location; set => location = value; }
    public string Name { get => name; set => name = value; }
    public SetlistInfo Setlist { get => setlist; set => setlist = value; }

    public string GetLog()
    {
        return "date : " + date + " / location : " + location + " / name : " + name;
    }
}