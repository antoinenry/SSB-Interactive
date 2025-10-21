using System;
using System.Text.Json.Serialization;
using System.Linq;

[Serializable]
public struct StageInfo
{
    [Serializable] public struct MomentInfo
    {
        public string name;
        public int moment;
        public int index;

        [JsonPropertyName("title")] public string Title { get => name; set => name = value; }
        [JsonPropertyName("moment")] public int? Moment { get => moment; set => moment = value.HasValue ? value.Value : -1; }
        [JsonPropertyName("index")] public int? Index { get => index; set => index = value.HasValue ? value.Value : -1; }
    }

    public string name;
    public bool hasScore;
    public MomentInfo[] moments;
    public int databaseID;

    public static StageInfo None => new StageInfo()
    {
        name = null,
        hasScore = false,
        moments = null
    };

    public override bool Equals(object obj)
    {
        return obj is StageInfo && this == (StageInfo)obj;
    }

    public override int GetHashCode()
    {
        return databaseID;
    }

    public static bool operator ==(StageInfo left, StageInfo right)
        => left.name == right.name && left.hasScore == right.hasScore && ((left.moments == null && right.moments == null) || left.moments == right.moments || Enumerable.SequenceEqual(left.moments, right.moments));

    public static bool operator !=(StageInfo left, StageInfo right)
        => left.name != right.name || left.hasScore != right.hasScore || ((left.moments != null || right.moments != null) && left.moments != right.moments && !Enumerable.SequenceEqual(left.moments, right.moments));

    [JsonPropertyName("title")] public string Name { get => name; set => name = value; }
    [JsonPropertyName("hasScore")] public bool? HasScore { get => hasScore; set => hasScore = value.HasValue ? value.Value : false; }
    [JsonPropertyName("stageMoments")] public MomentInfo[] Moments { get => moments; set => moments = value; }
    [JsonPropertyName("id")] public int? DatabaseID { get => databaseID; set => databaseID = value.HasValue ? value.Value : -1; }
}