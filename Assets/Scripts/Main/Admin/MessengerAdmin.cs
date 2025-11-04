using System;
using System.Text.Json;
using UnityEngine;

public class AdminText
{
    public string Message { get; set; }
    public float Duration { get; set; }

    public AdminText(string text, float duration)
    {
        this.Message = text;
        this.Duration = duration;
    }
}

public class MessengerAdmin : MonoBehaviourSingleton<MessengerAdmin>
{
    public string eventName = "text";
    public string text;

    public float duration = 4;
    public ObjectMethodCaller methodCaller = new ObjectMethodCaller("Send");

    private SocketIOClientScriptable client;

    private void Awake()
    {
        if (!HasAllComponents()) Debug.LogWarning("Missing components");
    }

    private bool HasAllComponents()
    {
        if (client) return true;
        client = CurrentAssetsManager.GetCurrent<SocketIOClientScriptable>();
        return client;
    }

    public void Send()
    {
        SendText(text, duration);
    }

    public void SendText(string t, float duration)
    {
        text = t;
        if (!HasAllComponents()) return;
        AdminText message = new(t, duration);
        JsonSerializerOptions options = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        String jsonMessage = JsonSerializer.Serialize(message, options);
        client.Emit(eventName, jsonMessage);
    }

    static public void Send(string t)
    {
        if (Current != null)
        {
            Current.SendText(t, Current.duration);
        }
    }

    static public void Send(string t, float customDuration)
    {
        if (Current != null)
        {
            Current.SendText(t, customDuration);
        }
    }
}
