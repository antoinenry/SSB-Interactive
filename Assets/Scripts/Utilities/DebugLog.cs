using SocketIOClient;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
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
    private InputSystem inputSystem;

    private void Awake()
    {
        client = CurrentAssetsManager.GetCurrent<SocketIOClientScriptable>();
        socketIOEventQueue = new Queue<string>(socketIOEventQueueLength);
        inputSystem = FindObjectOfType<InputSystem>();
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
            List<ButtonTimeSpawnData> window = inputSystem?.GetWindow();
            if (window != null)
            {
                logText += "\nTime window: " + inputSystem.timeWindow + "s";
                logText += "\nRequest duration: " + inputSystem.RequestDuration + "s";
                logText += "\nTime between requests: " + inputSystem.TimeBetweenRequests + "s";
                logText += "\nButton counts:";
                foreach (ButtonTimeSpawnData b in window)
                    logText += "\n- " + b.buttonID + ": " + b.maxCount + " (+ " + b.DeltaCount + ")";
            }
        }
    }
}
