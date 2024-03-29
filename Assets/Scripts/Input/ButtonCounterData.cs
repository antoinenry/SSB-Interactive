using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

[Serializable]
public struct ButtonCountData
{
    private string buttonID;
    private int totalPresses;

    [JsonPropertyName("name")] public string ButtonID { get => buttonID; set => buttonID = value; }
    [JsonPropertyName("count")] public int InputCount { get => totalPresses; set => totalPresses = value; }

    public static ButtonCountData[] Deserialize(string json) => JsonSerializer.Deserialize<ButtonCountData[]>(json);
}

[Serializable]
public struct ButtonCountFrame
{
    public float time;
    public Dictionary<string, int> totalPresses;

    public ButtonCountFrame(float time, ButtonCountData[] data)
    {
        this.time = time;
        int dataCount = data != null ? data.Length : 0;
        totalPresses = new Dictionary<string, int>(dataCount);
        for (int i = 0; i < dataCount; i++)
        {
            ButtonCountData d = data[i];
            if (totalPresses.ContainsKey(d.ButtonID)) totalPresses[d.ButtonID] += d.InputCount;
            else totalPresses.Add(d.ButtonID, d.InputCount);
        }
    }

    static public int CompareByAge(ButtonCountFrame a, ButtonCountFrame b) => b.time.CompareTo(a.time);

    public void AddPresses(Dictionary<string, int> presses)
    {
        if (presses == null) return;
        if (totalPresses == null)
        {
            totalPresses = new(presses);
            return;
        }
        foreach (string key in presses.Keys)
        {
            if (totalPresses.ContainsKey(key)) totalPresses[key] += presses[key];
            else totalPresses.Add(key, presses[key]);
        }
    }
}

[Serializable]
public struct ButtonCountDelta
{
    public string buttonID;
    public float startTime;
    public float endTime;
    public int minCount;
    public int maxCount;

    public float DeltaTime => endTime - startTime;
    public int DeltaCount => maxCount - minCount;

    public ButtonCountDelta(string id, float timeStart, float timeEnd, int countStart, int countEnd)
    {
        buttonID = id;
        minCount = countStart;
        maxCount = countEnd;
        startTime = timeStart;
        endTime = timeEnd;
    }

    public ButtonCountDelta(string id, float time, int count)
    {
        buttonID = id;
        minCount = count;
        maxCount = minCount;
        startTime = time;
        endTime = startTime;
    }

    public void AddFrame(ButtonCountFrame frame)
    {
        if (frame.totalPresses == null || frame.totalPresses.ContainsKey(buttonID) == false) return;
        float time = frame.time;
        int count = frame.totalPresses[buttonID];
        startTime = Mathf.Min(time, startTime);
        endTime = Mathf.Max(time, endTime);
        minCount = Mathf.Min(count, minCount);
        maxCount = Mathf.Max(count, maxCount);
    }
}
