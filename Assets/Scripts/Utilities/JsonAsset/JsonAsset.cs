using System.IO;
using System.Text.Json;
using UnityEngine;

public abstract class JsonAsset : ScriptableObject
{
    [CurrentToggle] public bool isCurrent;
    public string savePath;

    static public JsonSerializerOptions JsonOptions => new JsonSerializerOptions()
    {
        WriteIndented = true,
        IncludeFields = true
    };

    public abstract void Save();
    public abstract void Load();
}

public abstract class JsonAsset<T> : JsonAsset where T : struct
{
    public abstract T Data { get; set; }

    override public void Save()
    {
        string dataString = JsonSerializer.Serialize(Data, JsonOptions);
        File.WriteAllText(Application.dataPath + "/" + savePath, dataString);
    }

    override public void Load()
    {
        string dataString = File.ReadAllText(Application.dataPath + "/" + savePath);
        object dataObject = JsonSerializer.Deserialize(dataString, typeof(T), JsonOptions);
        if (dataObject != null && dataObject is T) Data = (T)dataObject;
    }
}