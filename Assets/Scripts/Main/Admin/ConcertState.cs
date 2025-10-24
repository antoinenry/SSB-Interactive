using System;
using System.Text.Json.Serialization;

[Serializable]
public struct ConcertState
{
    public SetlistInfo setlist;
    public SongInfo song;
    public int songPosition;
    public int moment;
    public int momentIndex;
    public bool paused;
    public int databaseID;

    public static ConcertState None => new ConcertState()
    {
        setlist = SetlistInfo.None,
        song = SongInfo.None,
        songPosition = -1,
        moment = -1,
        momentIndex = -1,
        paused = false
    };

    public override bool Equals(object obj)
    {
        return obj is ConcertState && this == (ConcertState)obj;
    }

    public override int GetHashCode()
    {
        return databaseID;
    }

    public static bool operator ==(ConcertState left, ConcertState right)
        => left.setlist == right.setlist && left.song == right.song && left.songPosition == right.songPosition && left.moment == right.moment && left.momentIndex == right.momentIndex && left.paused == right.paused;

    public static bool operator !=(ConcertState left, ConcertState right)
        => left.setlist != right.setlist || left.song != right.song || left.songPosition != right.songPosition || left.moment != right.moment || left.momentIndex == right.momentIndex || left.paused == right.paused;

    public bool PositionMatchesSong => setlist.GetSongByIndex(songPosition) == song;

    public bool Paused { get => paused; set => paused = value; }

    public string GetLog()
    {
        return "setlist : " + setlist.Name + " / song : " + song.Title + " / position : " + songPosition + " / stage : " + song.linkedStage.Name + " / moment " + moment + " (" + momentIndex + ")";
    }

    public StageInfo Stage => song.linkedStage;

    [JsonPropertyName("setlist")] public SetlistInfo? Setlist { get => setlist; set => setlist = value.HasValue ? value.Value : SetlistInfo.None; }
    [JsonPropertyName("setlistSong")] public SetlistState? SetlistSong
    {
        get => new SetlistState()
        {
            Song = song,
            Position = PositionMatchesSong ? songPosition : -1
        };
        set
        {
            SetlistState setlistSong = value.HasValue ? value.Value : SetlistState.None;
            song = setlistSong;
            songPosition = setlistSong.position;
        }
    }
    [JsonPropertyName("moment")] public int? Moment { get => moment; set => moment = value.HasValue ? value.Value : -1; }
    [JsonPropertyName("momentIndex")] public int? MomentIndex { get => momentIndex; set => momentIndex = value.HasValue ? value.Value : -1; }

    [Serializable]
    public struct PauseState
    {
        public bool paused;

        public static implicit operator bool(PauseState state) => state.paused;

        [JsonPropertyName("pause")] public bool? Paused { get => paused; set => paused = value.HasValue ? value.Value : false; }
    }
    [JsonPropertyName("id")] public int? DatabaseID { get => databaseID; set => databaseID = value.HasValue ? value.Value : -1; }
}