using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

[Serializable]
public struct SetlistInfo
{
    public string name;
    public SongInfo[] songs;
    public int databaseID;

    public SetlistInfo(SetlistInfo model)
    {
        name = model.name;
        songs = null;
        databaseID = model.databaseID;
        SetSongs(model.songs);
    }

    public void Copy(SetlistInfo model)
    {
        name = model.name;
        songs = null;
        databaseID = model.databaseID;
        SetSongs(model.songs);
    }

    public static SetlistInfo None => new SetlistInfo() { name = null, songs = null };

    public override bool Equals(object obj)
    {
        return obj is SetlistInfo && this == (SetlistInfo)obj;
    }

    public override int GetHashCode()
    {
        return databaseID;
    }

    public static bool operator ==(SetlistInfo left, SetlistInfo right)
        => left.name == right.name && (left.songs == right.songs || (left.songs != null && Enumerable.SequenceEqual(left.songs, right.songs)));

    public static bool operator !=(SetlistInfo left, SetlistInfo right)
        => left.name != right.name || (left.songs != right.songs && (left.songs == null || !Enumerable.SequenceEqual(left.songs, right.songs)));

    public int Length
    {
        get => songs != null ? songs.Length : 0;
        set
        {
            int length = Mathf.Max(0, value);
            if (songs == null) songs = new SongInfo[length];
            else Array.Resize(ref songs, length);
        }
    }

    public void SetSongs(SongInfo[] setSongs)
    {
        Length = setSongs != null ? setSongs.Length : 0;
        Array.Copy(setSongs, songs, Length);
    }

    public void SetSongs(SetlistState[] setSongs)
    {
        int length = 0;
        if (setSongs == null || setSongs.Length == 0) return;
        foreach(SetlistState s in  setSongs)
        {
            length = Mathf.Max(length, s.position + 1);
        }
        songs = new SongInfo[length];
        foreach (SetlistState s in setSongs)
        {
            if (s.position < 0) continue;
            songs[s.position] = s;
        }
    }

    public SongInfo[] GetSongs()
    {
        SongInfo[] getSongs = new SongInfo[Length];
        if (songs != null) Array.Copy(songs, getSongs, Length);
        return getSongs;
    }

    public SongInfo GetSongByIndex(int index)
    {
        if (index < 0 || index >= Length) return SongInfo.None;
        return songs[index];
    }

    public bool ContainsSong(SongInfo song) => songs != null && songs.Contains(song);

    public SongInfo FindSong(Predicate<SongInfo> predicate)
    {
        if (Length == 0 || predicate == null) return SongInfo.None;
        return Array.Find(songs, predicate);
    }

    [JsonPropertyName("name")] public string Name { get => name; set => name = value; }
    [JsonPropertyName("setlistSongs")] public SetlistState[] SetlistSongs
    {
        get
        {
            SetlistState[] getSongs = new SetlistState[Length];
            for (int i = 0; i < Length; i++)
            {
                getSongs[i] = new SetlistState()
                {
                    Song = songs[i],
                    Position = i
                };
            }
            return getSongs;
        }
        set
        {
            SetSongs(value);
        }
    }
    [JsonPropertyName("id")] public int? DatabaseID { get => databaseID; set => databaseID = value.HasValue ? value.Value : -1; }
}