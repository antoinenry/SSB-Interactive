using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


[CreateAssetMenu(fileName = "SocketIOClient", menuName = "Client/SocketIOClient")]
public class SocketIOClientScriptable : ScriptableObject
{
    public string serverUrl = "htpps://smash3000.ovh";
    public SocketIOClientOptions options = SocketIOClientOptions.Default;

    private SocketIOClient.SocketIO client;
    private Task connectionTask;
    private string exceptionMessage;
    private List<string> subscriptions;
    private List<string> responses;

    public UnityEvent<string> onReceive;

    private void OnDisable() => Disconnect();

    #region Start / End client
    public void Init()
    {
        client = new SocketIOClient.SocketIO(serverUrl, options);
        client.OnConnected += OnConnected;
        client.OnDisconnected += OnDisconnected;
        client.OnError += OnError;
    }

    public void Dispose()
    {
        if (client != null)
        {
            client.OnConnected -= OnConnected;
            client.OnDisconnected -= OnDisconnected;
            client.OnError -= OnError;
            client.Dispose();
            client = null;
        }
    }

    private void OnError(object sender, string e)
    {
        Debug.LogWarning("SocketIO client error: " + e);
    }
    #endregion

    #region Start / End connexion
    public ConnectionState Connection
    {
        get
        {
            if (exceptionMessage != null && exceptionMessage.Length > 0) return ConnectionState.Error;
            if (client != null)
            {
                if (connectionTask == null || connectionTask.IsCompleted) return client.Connected ? ConnectionState.Connected : ConnectionState.Disconnected;
                else return client.Connected ? ConnectionState.Disconnecting : ConnectionState.Connecting;
            }
            else
                return ConnectionState.Disconnected;
        }
    }

    public void Connect()
    {
        connectionTask = ConnectAsync();
        if (subscriptions != null) foreach (string s in subscriptions) AddListenner(s);
    }

    async private Task ConnectAsync()
    {
        try
        {
            Init();
            exceptionMessage = null;
            await client.ConnectAsync();
        }
        catch (Exception e)
        { 
            exceptionMessage = e.Message;
        }
    }

    private void OnConnected(object sender, EventArgs e)
    {

    }

    public void Disconnect()
    {
        if (subscriptions != null) foreach (string s in subscriptions) RemoveListenner(s);
        connectionTask = DisconnectAsync();
    }

    async private Task DisconnectAsync()
    {
        try
        {
            exceptionMessage = null;
            if (client != null) await client.DisconnectAsync();
        }
        catch (Exception e)
        {
            exceptionMessage = e.Message;
        }
    }

    private void OnDisconnected(object sender, string e)
    {
        Dispose();
    }
    #endregion

    #region Subscription / Reception
    public string[] Subscriptions => subscriptions != null ? subscriptions.ToArray() : new string[0];
    public string[] Responses => responses != null ? responses.ToArray() : new string[0];

    public void Subscribe(string eventName)
    {
        if (subscriptions == null) subscriptions = new List<string>();
        if (!subscriptions.Contains(eventName))
        {
            subscriptions.Add(eventName);
            if (Connection == ConnectionState.Connected) AddListenner(eventName);
        }
    }

    public void Unsubscribe(string eventName)
    {
        if (subscriptions == null) return;
        if (subscriptions.Contains(eventName))
        {
            subscriptions.Remove(eventName);
            if (Connection == ConnectionState.Connected) RemoveListenner(eventName);
        }
    }

    private void AddListenner(string eventName)
    {
        if (eventName != null && client != null) client.On(eventName, socketResponse => OnReceive(eventName));
    }

    private void RemoveListenner(string eventName)
    {
        if (eventName != null && client != null) client.Off(eventName);
    }

    private void OnReceive(string eventName)
    {
        if (responses == null) responses = new List<string>();
        onReceive.Invoke(eventName);
    }
    #endregion
}
