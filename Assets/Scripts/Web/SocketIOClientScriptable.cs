using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SocketIOClient;

[CreateAssetMenu(fileName = "SocketIOClient", menuName = "Client/SocketIOClient")]
public class SocketIOClientScriptable : ScriptableObject
{
    public string serverUrl = "https://smash3000.ovh";
    public SocketIOClientOptions options = SocketIOClientOptions.Default;

    private SocketIOClient.SocketIO client;
    private Task connectionTask;
    private string exceptionMessage;

    public UnityEvent onConnected;
    public UnityEvent onDisconnected;
    public Dictionary<string, SocketIOResponseEvent> onReceived;

    private void Reset() => Dispose();

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
        onConnected.Invoke();
        if (onReceived != null)
            foreach (string eventName in onReceived.Keys)
                client.On(eventName, r => onReceived[eventName].Invoke(r));
    }

    public void Disconnect()
    {
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
        onDisconnected.Invoke();
        if (onReceived != null)
            foreach (string eventName in onReceived.Keys)
                client.Off(eventName);
        Dispose();
    }
    #endregion

    #region Subscription / Reception
    public string[] Subscriptions
    {
        get
        {
            if (onReceived == null) return new string[0];
            string[] keys = new string[onReceived.Count];
            onReceived.Keys.CopyTo(keys, 0);
            return keys;
        }
    }

    public void Subscribe(string eventName, UnityAction<string, SocketIOResponse> onReception)
    {
        if (onReceived == null) onReceived = new Dictionary<string, SocketIOResponseEvent>();
        SocketIOResponseEvent e;
        if (onReceived.ContainsKey(eventName))
        {
            e = onReceived[eventName];
        }
        else
        {
            e = new SocketIOResponseEvent(eventName);
            onReceived.Add(eventName, e);
            if (Connection == ConnectionState.Connected) client.On(eventName, r => e.Invoke(r));
        }
        e.AddListener(onReception);
    }

    public void Unsubscribe(string eventName, UnityAction<string, SocketIOResponse> onReception)
    {
        if (onReceived == null || onReceived.ContainsKey(eventName) == false) return;
        SocketIOResponseEvent e = onReceived[eventName];
        e.RemoveListener(onReception);
        onReceived.Remove(eventName);
        if (Connection == ConnectionState.Connected) client.Off(eventName);
    }
    #endregion
}
