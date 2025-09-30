using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using static UnityEditor.FilePathAttribute;

[Serializable] 
public struct ConcertStateData
{
    //[JsonPropertyName("setlistSong")] public SetlistSongData SetlistSong { get => setlistSong; set => setlistSong = value; }
    [JsonPropertyName("stageTitle")] public string Stage { get => stage; set => stage = value; }
    [JsonPropertyName("moment")] public int Moment { get => moment; set => moment = value; }

    static public ConcertStateData Deserialize(string dataString)
    {
        try
        {
            object dataObject = JsonSerializer.Deserialize(dataString, typeof(ConcertStateData));
            if (dataObject != null && dataObject is ConcertStateData) return (ConcertStateData)dataObject;
            else return new();
        }
        catch
        {
            return new();
        }
    }

    //public SetlistSongData setlistSong;
    public string stage;
    public int moment;

    //public string StageName => setlistSong.song.stage.title;

    [Serializable]
    public struct SetlistSongData
    {
        [JsonPropertyName("song")] public SongData Song { get => song; set => song = value; }

        public SongData song;    
    }

    [Serializable]
    public struct SongData
    {
        [JsonPropertyName("title")] public string Title { get => title; set => title = value; }
        [JsonPropertyName("stage")] public StageData Stage { get => stage; set => stage = value; }

        public string title;
        public StageData stage;
    }

    [Serializable]
    public struct StageData
    {
        [JsonPropertyName("title")] public string Title { get => title; set => title = value; }

        public string title;
    }
    public string GetLog()
    {
        return "stage : " + stage + " / moment : " + moment;
    }
}


/*
{
    "current": 
    {
        "id": 0,
        "title": "string",
        "setlistSong": 
        {
            "id": 0,
            "song": 
            {
                "id": 0,
                "title": "string",
                "duration": 0,
                "bpm": 0,
                "zgg": 0,
                "stage": 
                {
                    "id": 0,
                    "title": "string",
                    "hasScore": true
                }
            },
            "position": 0
        },
        "setlist":
        {
            "id": 0,
            "name": "string",
            "setlistSongs": [
            {
                "id": 0,
                "song": 
                {
                    "id": 0,
                    "title": "string",
                    "duration": 0,
                    "bpm": 0,
                    "zgg": 0,
                    "stage":
                    {
                        "id": 0,
                        "title": "string",
                        "hasScore": true
                    }
                },
                "position": 0
            }
            ]
        },
        "moment": 0
    },
    "pause":
    {
        "id": 0,
        "pause": true
    }
}
*/