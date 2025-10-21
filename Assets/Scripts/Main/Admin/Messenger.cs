using UnityEngine;

public class Messenger : MonoBehaviourSingleton<Messenger>
{
    public string eventName = "text";
    public string text;
    public ObjectMethodCaller methodCaller = new ObjectMethodCaller("SendText");

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

    public void SendText()
    {
        if (!HasAllComponents()) return;
        client.Emit(eventName, text);
    }

    public void Send(string t)
    {
        text = t;
        SendText();
    }

    static public void SendText(string t) => Current?.Send(t);
}
