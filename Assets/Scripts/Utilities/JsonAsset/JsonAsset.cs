using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using UnityEngine;

public abstract class JsonAsset : ScriptableObject
{
    public abstract void Save();
    public abstract void Load();
}

public abstract class JsonAsset<T> : JsonAsset where T : struct
{
    public string savePath;

    public string CompletePath
    {
        get
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
            return completePath;
        }
    }
    public bool isUserData = false;
    public T Data;

    private void OnEnable() => Load();
    private void OnDisable() => Save();

    static public JsonSerializerOptions JsonOptions => new JsonSerializerOptions()
    {
        WriteIndented = true,
        IncludeFields = true,
        AllowTrailingCommas = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    public override void Save()
    {
        string dataString = JsonSerializer.Serialize(Data, JsonOptions);
        File.WriteAllText(CompletePath, dataString);
    }

    public override void Load()
    {
        if (File.Exists(CompletePath))
        {
            string dataString = File.ReadAllText(CompletePath);
            object dataObject = JsonSerializer.Deserialize(dataString, typeof(T), JsonOptions);
            if (dataObject != null && dataObject is T) Data = (T)dataObject;
        }
    }
}