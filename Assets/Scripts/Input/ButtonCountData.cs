using System;
using System.Text.Json;
using System.Text.Json.Serialization;

[Serializable]
public struct ButtonCountData
{
    public string buttonID;
    public int totalInput;

    [JsonPropertyName("name")] public string ButtonID { get => buttonID; set => buttonID = value; }
    [JsonPropertyName("count")] public int InputCount { get => totalInput; set => totalInput = value; }

    public static ButtonCountData[] Deserialize(string json)
    {
        return JsonSerializer.Deserialize<ButtonCountData[]>(json);
    }
}