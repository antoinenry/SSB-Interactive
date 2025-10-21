using System;
using System.Text.Json.Serialization;

[Serializable]
public struct ConcertInfo
{
    public string name;
    public string location;
    public string date;
    public SetlistInfo setlist;
    public int databaseID;

    public static ConcertInfo None => new ConcertInfo()
    {
        date = null,
        location = null,
        name = null,
    };

    public override bool Equals(object obj)
    {
        return obj is ConcertInfo && this == (ConcertInfo)obj;
    }

    public override int GetHashCode()
    {
        return databaseID;
    }

    public static bool operator ==(ConcertInfo left, ConcertInfo right)
        => left.name == right.name && left.location == right.location && left.date == right.date && left.setlist == right.setlist;

    public static bool operator !=(ConcertInfo left, ConcertInfo right)
        => left.name != right.name || left.location != right.location || left.date != right.date || left.setlist != right.setlist;

    public ConcertInfo(ConcertInfo model)
    {
        name = model.name;
        date = model.date;
        location = model.location;
        setlist = new(model.setlist);
        databaseID = model.databaseID;
    }

    public void Copy(ConcertInfo model)
    {
        name = model.name;
        date = model.date;
        location = model.location;
        setlist = new(model.setlist);
        databaseID = model.databaseID;
    }

    public string GetLog()
    {
        return "name : " + name + " / date : " + date + " / location : " + location;
    }

    [JsonPropertyName("name")] public string Name { get => name; set => name = value; }
    [JsonPropertyName("location")] public string Location { get => location; set => location = value; }
    [JsonPropertyName("date")] public string Date { get => date; set => date = value; }
    [JsonPropertyName("setlist")] public SetlistInfo? Setlist { get => setlist; set => setlist = value.HasValue ? value.Value : SetlistInfo.None; }
    [JsonPropertyName("id")] public int? DatabaseID { get => databaseID; set => databaseID = value.HasValue ? value.Value : -1; }
}