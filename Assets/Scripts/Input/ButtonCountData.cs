using System;
using System.Text.Json;
using System.Text.Json.Serialization;

[Serializable]
public struct ButtonCountData
{
    public string buttonID;
    public int totalInput;

    public int DeltaInput { get; private set; }

    [JsonPropertyName("name")] public string ButtonID { get => buttonID; set => buttonID = value; }
    [JsonPropertyName("count")] public int InputCount { get => totalInput; set => totalInput = value; }

    public static ButtonCountData[] Deserialize(string json)
    {
        return JsonSerializer.Deserialize<ButtonCountData[]>(json);
    }

    public static ButtonCountData[] UpdateFromJSON(ButtonCountData[] counts, string json)
    {
        ButtonCountData[] newCounts = Deserialize(json);
        int newLength = newCounts != null ? newCounts.Length : 0;
        if (newLength == 0 || counts == null) return newCounts;
        for (int i = 0; i < newLength; i++)
        {
            ButtonCountData newCount = newCounts[i];
            int oldTotal = Array.Find(counts, c => c.buttonID == newCount.buttonID).totalInput;
            newCounts[i] = new ButtonCountData()
            {
                buttonID = newCount.buttonID,
                totalInput = newCount.totalInput,
                DeltaInput = newCount.totalInput - oldTotal
            };
        }
        return newCounts;
    }
}