using System;
using System.Text.Json;
using System.Text.Json.Serialization;

[Serializable]
public struct ButtonCountData
{
    public string buttonID;
    public int totalPresses;
    public int deltaPresses;
    public float time;
    public float deltaTime;

    [JsonPropertyName("name")] public string ButtonID { get => buttonID; set => buttonID = value; }
    [JsonPropertyName("count")] public int InputCount { get => totalPresses; set => totalPresses = value; }

    public static ButtonCountData[] Deserialize(string json) => JsonSerializer.Deserialize<ButtonCountData[]>(json);
    public void Update(int presses, float atTime)
    {
        deltaPresses = presses - totalPresses;
        deltaTime = atTime - time;
        totalPresses = presses;
        time = atTime;
    }
}

[Serializable]
public class ButtonCounter
{
    public ButtonCountData[] data;

    public int Buttons => data != null ? data.Length : 0;

    public void UpdateFromJSON(string json, float time)
    {
        ButtonCountData[] jsonData = ButtonCountData.Deserialize(json);
        if (data != null)
        {
            int newLength = jsonData != null ? jsonData.Length : 0;
            for (int i = 0; i < newLength; i++)
            {
                ButtonCountData newData = jsonData[i];
                ButtonCountData oldData = Array.Find(data, oldData => oldData.buttonID == newData.buttonID);
                if (oldData.buttonID == newData.buttonID)
                {
                    oldData.Update(newData.totalPresses, time);
                    jsonData[i] = oldData;
                }
                else
                {
                    jsonData[i] = newData;
                }
            }
        }
        data = jsonData;
    }
}