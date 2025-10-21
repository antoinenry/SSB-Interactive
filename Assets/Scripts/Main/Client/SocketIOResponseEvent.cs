using UnityEngine;
using UnityEngine.Events;
using SocketIOClient;
using System;
using System.Text.Json;

[Serializable]
public class SocketIOResponseEvent : UnityEvent<string, SocketIOResponse>
{
    readonly public string Name;

    public SocketIOResponseEvent(string name)
    {
        Name = name;
    }

    public void Invoke(SocketIOResponse r) => Invoke(Name, r);

    public static T DeserializeResponse<T>(string response)
    {
        if (response == null || response.Length == 0) return default;
        try
        {
            return JsonSerializer.Deserialize<T>(response);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Deserialize exception: " + e);
            Debug.Log("...when deserializing " + response);
            return default;
        }
    }
}