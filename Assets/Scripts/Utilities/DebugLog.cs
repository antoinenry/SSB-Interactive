using SocketIOClient;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class DebugLog : MonoBehaviour
{
    public TMP_Text field;
    public string logText = "coucou";
    public int eventQueueLength;

    private SocketIOClientScriptable client;
    private Queue<string> eventQueue;

    private void Awake()
    {
        logText = "";
        eventQueue = new Queue<string>(eventQueueLength);
    }

    private void Start()
    {
        client = CurrentAssetsManager.GetCurrent<SocketIOClientScriptable>();
        foreach (string sub in client.Subscriptions)
            client.Subscribe(sub, OnClientReceives);

    }

    private void OnClientReceives(string eventName, SocketIOResponse response)
    {
        eventQueue.Enqueue (eventName);
        if (eventQueue.Count > eventQueueLength) eventQueue.Dequeue();
    }

    [ExecuteAlways]
    private void Update()
    {
        SetLog();
        if (field) field.text = logText;
    }

    private void SetLog()
    {
        logText = "Client status: ";
        if (client) logText += client.Connection;
        else logText += "NULL";
        if (eventQueue != null)
            foreach(string s in eventQueue)
                logText += "\n " + s;
    }
}
