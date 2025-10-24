using UnityEngine;

public class MessengerAdmin : MonoBehaviourSingleton<MessengerAdmin>
{
    public string eventName = "text";
    public string text;
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
        SendText(text);
    }

    public void SendText(string t)
    {
        text = t;
        if (!HasAllComponents()) return;
        client.Emit(eventName, text);
    }

    static public void Send(string t)
    {
        Current?.SendText(t);
    }
}
