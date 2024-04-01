using UnityEngine;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

[Serializable]
public struct ButtonCountData
{
    private string buttonID;
    private int count;

    [JsonPropertyName("name")] public string ButtonID { get => buttonID; set => buttonID = value; }
    [JsonPropertyName("count")] public int InputCount { get => count; set => count = value; }

    public static ButtonCountData[] Deserialize(string json) => JsonSerializer.Deserialize<ButtonCountData[]>(json);
}

[Serializable]
public struct ButtonTimeSpawnData
{
    public string buttonID;
    public float startTime;
    public float endTime;
    public int minCount;
    public int maxCount;

    public float DeltaTime => endTime - startTime;
    public int DeltaCount => maxCount - minCount;
    public float Rate => DeltaTime != 0f ? DeltaCount / DeltaTime : 0f;

    public ButtonTimeSpawnData(string id, float timeStart, float timeEnd, int countStart, int countEnd)
    {
        buttonID = id;
        minCount = countStart;
        maxCount = countEnd;
        startTime = timeStart;
        endTime = timeEnd;
    }

    public ButtonTimeSpawnData(string id, float time, int count)
    {
        buttonID = id;
        minCount = count;
        maxCount = minCount;
        startTime = time;
        endTime = startTime;
    }

    public void AddCountAtTime(int count, float time)
    {
        startTime = Mathf.Min(time, startTime);
        endTime = Mathf.Max(time, endTime);
        minCount = Mathf.Min(count, minCount);
        maxCount = Mathf.Max(count, maxCount);
    }
}
