using System;
using System.Text.Json.Serialization;
using UnityEngine;

[Serializable]
public struct SetlistInfo
{
    public string name;
    public SongInfo[] songs;

    public static SetlistInfo None => new SetlistInfo() { name = null, songs = null };

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

    public SongInfo[] GetSongs()
    {
        SongInfo[] getSongs = new SongInfo[Length];
        if (songs != null) Array.Copy(songs, getSongs, Length);
        return getSongs;
    }

    public SongInfo GetSong(int index)
    {
        if (index < 0 || index >= Length) return SongInfo.None;
        return songs[index];
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
            SetSongs(value != null ? Array.ConvertAll(value, s => s.Song.Value) : null);
        }
    }
}