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
    public bool showStageLoaderDebug;

    private string logText;
    private SocketIOClientScriptable client;
    private Queue<string> socketIOEventQueue;
    private int socketIOEventCount;
    private StageLoader stageLoader;

    private void OnEnable()
    {
        client = CurrentAssetsManager.GetCurrent<SocketIOClientScriptable>();
        socketIOEventQueue = new Queue<string>(socketIOEventQueueLength);
        stageLoader = FindObjectOfType<StageLoader>();
    }

    private void Start()
    {
        foreach (string sub in client.Subscriptions)
            client.Subscribe(sub, OnClientReceives);
    }

    private void OnClientReceives(string eventName, SocketIOResponse response)
    {
        socketIOEventCount++;
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
            {
                int i = socketIOEventCount;
                foreach (string s in socketIOEventQueue)
                {
                    logText += "\n" + i + ". " + s;
                    i--;
                }
            }
            logText += "\n\n";
        }
        if (showInputSystemDebug)
        {
            logText += "Input System status: ";
            logText += InputSource.GetLog();
            logText += "\n\n";
        }
        if (showStageLoaderDebug)
        {
            logText += "Stage Loader status:\n";
            logText += "stages: " + stageLoader.Config?.StageCount + "\n";
            logText += "missing scenes: " + stageLoader.Config?.Data.missingStages?.Length;
        }
    }
}
