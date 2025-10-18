using SocketIOClient;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class DebugLog : MonoBehaviour
{
    public TMP_Text field;
    public KeyCode toggleKey = KeyCode.L;
    public bool visible = false;
    public int socketIOEventQueueLength;

    private string logText;
    private Concert concertClient;
    private SocketIOClientScriptable socketClient;
    private Queue<string> socketIOEventQueue;
    private int socketIOEventCount;
    private StageLoader stageLoader;
    private ClientButtonTracker buttonCounter;

    private void OnEnable()
    {
        concertClient = FindObjectOfType<Concert>();
        socketClient = CurrentAssetsManager.GetCurrent<SocketIOClientScriptable>();
        socketIOEventQueue = new Queue<string>(socketIOEventQueueLength);
        stageLoader = FindObjectOfType<StageLoader>();
        buttonCounter = FindObjectOfType<ClientButtonTracker>();
    }

    private void Start()
    {
        foreach (string sub in socketClient.Subscriptions)
            socketClient.Subscribe(sub, OnClientReceives);
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
        if (Input.GetKeyDown(toggleKey)) ToggleVisibility();
        SetLog();
        if (field) field.text = logText;
    }

    private void ToggleVisibility()
    {
        visible = !visible;
    }

    private void SetLog()
    {
        logText = "";
        if (visible)
        {
            logText += "Framerate : " + (1f / Time.deltaTime).ToString("0") + "\n";

            logText += "SocketIO status : ";
            if (socketClient) logText += socketClient.Connection;
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

            logText += "Concert status : ";
            if (concertClient)
            {
                logText += "\n- concert : " + concertClient.info.GetLog();
                logText += "\n- state : " + concertClient.state.GetLog();
            }
            else logText += "NULL";

            logText += "\n\n";

            logText += "Input System status : ";
            logText += buttonCounter ? "\n" + buttonCounter.GetLog() : "missing button counter";

            logText += "\n\n";

            logText += "Stage Loader status :";
            logText += "\n- stages : " + stageLoader.Config?.StageCount;
            logText += "\n- missing scenes : " + stageLoader.Config?.Data.missingStages?.Length;
        }
    }
}
