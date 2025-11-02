using System.IO;
using System.Text.Json;
using UnityEngine;

public abstract class JsonAsset : ScriptableObject
{
    public string savePath;
    public bool isUserData = false;

    static public JsonSerializerOptions JsonOptions => new JsonSerializerOptions()
    {
        WriteIndented = true,
        IncludeFields = true,
        AllowTrailingCommas = true
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
        File.WriteAllText(GetCompletePath(), dataString);
    }

    override public void Load()
    {
        string dataString = File.ReadAllText(GetCompletePath());
        object dataObject = JsonSerializer.Deserialize(dataString, typeof(T), JsonOptions);
        if (dataObject != null && dataObject is T) Data = (T)dataObject;
    }

    public string GetCompletePath()
    {
        string completePath;
        if (isUserData)
        {
            completePath = Application.persistentDataPath + "/" + savePath;
        }
        else
        {
            completePath = Application.dataPath + "/" + savePath;
        }
        if (!Directory.Exists(Path.GetDirectoryName(completePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(completePath));   
        }
        Debug.Log("Save Path: " + completePath);
        return completePath;
    }
}