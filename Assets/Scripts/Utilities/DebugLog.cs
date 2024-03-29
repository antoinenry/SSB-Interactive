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
            ButtonTimeSpawnData[] window = inputSystem?.ButtonCounts;
            if (window != null)
            {
                logText += "\nTime window: " + inputSystem.timeWindow + "s (smooth " + inputSystem.smoothRates + "s)";
                logText += "\nRequest duration: " + inputSystem.RequestDuration + "s (every " + inputSystem.TimeBetweenRequests + "s)";
                logText += "\nButton counts:";
                foreach (ButtonTimeSpawnData b in window)
                    logText += "\n- " + b.buttonID + ": " + b.maxCount + " (+ " + b.DeltaCount + ") ; " + inputSystem.GetButtonRateSmooth(b.buttonID).ToString("0.0") + "/s";
            }
        }
    }
}
