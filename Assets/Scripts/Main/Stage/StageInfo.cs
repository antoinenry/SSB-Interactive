using System;
using System.Text.Json.Serialization;

[Serializable]
public struct StageInfo
{
    [Serializable] public struct MomentInfo
    {
        public string title;
        public int moment;
        public int index;

        [JsonPropertyName("title")] public string Title { get => title; set => title = value; }
        [JsonPropertyName("moment")] public int? Moment { get => moment; set => moment = value.HasValue ? value.Value : -1; }
        [JsonPropertyName("index")] public int? Index { get => index; set => index = value.HasValue ? value.Value : -1; }
    }

    public string title;
    public bool hasScore;
    public MomentInfo[] moments;

    public static StageInfo None => new StageInfo()
    {
        title = null,
        hasScore = false,
        moments = null
    };

    [JsonPropertyName("title")] public string Title { get => title; set => title = value; }
    [JsonPropertyName("hasScore")] public bool? HasScore { get => hasScore; set => hasScore = value.HasValue ? value.Value : false; }
    [JsonPropertyName("stageMoments")] public MomentInfo[] Moments { get => moments; set => moments = value; }
}