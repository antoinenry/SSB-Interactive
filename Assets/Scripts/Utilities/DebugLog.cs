using SocketIOClient;
using System;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class DebugLog : MonoBehaviour
{
    public TMP_Text field;
    public string logText = "coucou";

    private SocketIOClientScriptable client;
    private string latestEvent;

    private void Awake()
    {
        logText = "";
    }

    private void Start()
    {
        client = CurrentAssetsManager.GetCurrent<SocketIOClientScriptable>();
        foreach (string sub in client.Subscriptions)
            client.Subscribe(sub, OnClientReceives);

    }

    private void OnClientReceives(string eventName, SocketIOResponse response)
    {
        latestEvent = eventName;
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
        logText += "\nLatest event: " + latestEvent;
    }
}
