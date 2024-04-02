using SocketIOClient;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class DebugLog : MonoBehaviour
{
    public TMP_Text field;
    public bool showSocketIODebug;
    public int socketIOEventQueueLength;
    public bool showInputSystemDebug;

    private string logText;
    private SocketIOClientScriptable client;
    private Queue<string> socketIOEventQueue;
    private InputSource inputSource;

    private void Awake()
    {
        client = CurrentAssetsManager.GetCurrent<SocketIOClientScriptable>();
        socketIOEventQueue = new Queue<string>(socketIOEventQueueLength);
        inputSource = FindObjectOfType<InputSource>();
    }

    private void Start()
    {
        foreach (string sub in client.Subscriptions)
            client.Subscribe(sub, OnClientReceives);

    }

    private void OnClientReceives(string eventName, SocketIOResponse response)
    {
        socketIOEventQueue.Enqueue (eventName);
        if (socketIOEventQueue.Count > socketIOEventQueueLength) socketIOEventQueue.Dequeue();
    }

    [ExecuteAlways]
    private void Update()
    {
        SetLog();
        if (field) field.text = logText;
    }

    private void SetLog()
    {
        logText = "";
        if (showSocketIODebug)
        {
            logText += "SocketIO status: ";
            if (client) logText += client.Connection;
            else logText += "NULL";
            if (socketIOEventQueue != null)
                foreach (string s in socketIOEventQueue)
                    logText += "\n " + s;
            logText += "\n\n";
        }
        if (showInputSystemDebug)
        {
            logText += "Input System status: ";
            logText += InputSource.GetLog();
        }
    }
}
