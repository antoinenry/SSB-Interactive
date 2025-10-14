using System;
using System.Text.Json.Serialization;

[Serializable]
public struct StageInfo
{
    [Serializable] public struct MomentInfo
    {
        public int id;
        public string title;
        public int moment;
        public int index;

        [JsonPropertyName("id")] public int ID { get => id; set => id = value; }
        [JsonPropertyName("title")] public string Title { get => title; set => title = value; }
        [JsonPropertyName("moment")] public int Moment { get => moment; set => moment = value; }
        [JsonPropertyName("index")] public int Index { get => index; set => index = value; }
    }

    public int id;
    public string title;
    public bool hasScore;
    public MomentInfo[] moments;

    [JsonPropertyName("id")] public int ID { get => id; set => id = value; }
    [JsonPropertyName("title")] public string Title { get => title; set => title = value; }
    [JsonPropertyName("hasScore")] public string HasScore { get => hasScore.ToString(); set => hasScore = value == true.ToString(); }
    [JsonPropertyName("stageMoments")] public MomentInfo[] Moments { get => moments; set => moments = value; }
}