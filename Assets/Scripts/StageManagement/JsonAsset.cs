using System.IO;
using System.Text.Json;
using UnityEngine;

public abstract class JsonAsset<T> : ScriptableObject where T : struct
{
    [CurrentToggle] public bool isCurrent;
    public string savePath;

    [JsonAssetButtons("Save", "Load")] public string cmd;

    public abstract T Data { get; set; }

    private JsonSerializerOptions JsonOptions => new JsonSerializerOptions()
    {
        WriteIndented = true,
        IncludeFields = true
    };

    public void Save()
    {
        string dataString = JsonSerializer.Serialize(Data, JsonOptions);
        File.WriteAllText(Application.dataPath + "/" + savePath, dataString);
    }

    public void Load()
    {
        string dataString = File.ReadAllText(Application.dataPath + "/" + savePath);
        object dataObject = JsonSerializer.Deserialize(dataString, typeof(T), JsonOptions);
        if (dataObject != null && dataObject is T) Data = (T)dataObject;
    }
}

public class JsonAssetButtonsAttribute : PropertyAttribute
{
    public string[] methods;
    public JsonAssetButtonsAttribute(params string[] methodNames)
    {
        methods = methodNames;
    }
}