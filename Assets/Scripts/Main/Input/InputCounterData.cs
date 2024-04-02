using UnityEngine;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

[Serializable]
public struct SingleButtonCount
{
    private string buttonID;
    private int count;

    [JsonPropertyName("name")] public string ButtonID { get => buttonID; set => buttonID = value; }
    [JsonPropertyName("count")] public int InputCount { get => count; set => count = value; }

    public static SingleButtonCount[] Deserialize(string json) => JsonSerializer.Deserialize<SingleButtonCount[]>(json);
}

[Serializable]
public struct SingleButtonCountOverTime
{
    public string buttonID;
    public float startTime;
    public float endTime;
    public int minCount;
    public int maxCount;

    public float DeltaTime => endTime - startTime;
    public int DeltaCount => maxCount - minCount;
    public float Rate => DeltaTime != 0f ? DeltaCount / DeltaTime : 0f;

    public SingleButtonCountOverTime(string id, float timeStart, float timeEnd, int countStart, int countEnd)
    {
        buttonID = id;
        minCount = countStart;
        maxCount = countEnd;
        startTime = timeStart;
        endTime = timeEnd;
    }

    public SingleButtonCountOverTime(string id, float time, int count)
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

public struct MultipleButtonsCount
{
    public float time;
    public Dictionary<string, int> count;

    public MultipleButtonsCount(float time, SingleButtonCount[] data)
    {
        this.time = time;
        int dataCount = data != null ? data.Length : 0;
        count = new Dictionary<string, int>(dataCount);
        for (int i = 0; i < dataCount; i++)
        {
            SingleButtonCount d = data[i];
            if (count.ContainsKey(d.ButtonID)) count[d.ButtonID] += d.InputCount;
            else count.Add(d.ButtonID, d.InputCount);
        }
    }

    static public int CompareByAge(MultipleButtonsCount a, MultipleButtonsCount b) => b.time.CompareTo(a.time);

    public void AddPresses(Dictionary<string, int> presses)
    {
        if (presses == null) return;
        if (count == null)
        {
            count = new(presses);
            return;
        }
        foreach (string key in presses.Keys)
        {
            if (count.ContainsKey(key)) count[key] += presses[key];
            else count.Add(key, presses[key]);
        }
    }
}
