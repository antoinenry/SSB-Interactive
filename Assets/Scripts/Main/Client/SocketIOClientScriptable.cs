using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SocketIOClient;

[CreateAssetMenu(fileName = "SocketIOClient", menuName = "Client/SocketIOClient")]
public class SocketIOClientScriptable : ScriptableObject
{
    [CurrentToggle] public bool isCurrent;
    public string serverUrl = "https://smash3000.ovh";
    public SocketIOClientOptions options = SocketIOClientOptions.Default;

    private SocketIOClient.SocketIO client;
    private Task connectionTask;
    private List<Task> emissionTasks;
    private string connectionError;
    private string emissionError;

    public UnityEvent onConnected;
    public UnityEvent onDisconnected;
    public Dictionary<string, SocketIOResponseEvent> onReceived;

    private void Reset() => Dispose();

    #region Start / End client
    public void Init()
    {
        connectionError = null;
        emissionError = null;
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
            connectionError = null;
            emissionError = null;
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
            if (connectionError != null && connectionError.Length > 0) return ConnectionState.Error;
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
            connectionError = null;
            await client.ConnectAsync();
        }
        catch (Exception e)
        { 
            connectionError = e.Message;
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
            connectionError = null;
            if (client != null) await client.DisconnectAsync();
        }
        catch (Exception e)
        {
            connectionError = e.Message;
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

    public void ResetConnection()
    {
        connectionTask = ResetConnectionAsync();
    }

    async private Task ResetConnectionAsync()
    {
        try
        {
            connectionError = null;
            await DisconnectAsync();
            await ConnectAsync();
        }
        catch (Exception e)
        {
            connectionError = e.Message;
        }
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
            client.On(eventName, r => e.Invoke(r));
        }
        e.AddListener(onReception);
    }

    public void Unsubscribe(string eventName, UnityAction<string, SocketIOResponse> onReception)
    {
        if (onReceived == null || onReceived.ContainsKey(eventName) == false) return;
        SocketIOResponseEvent e = onReceived[eventName];
        e.RemoveListener(onReception);
        onReceived.Remove(eventName);
        //if (Connection == ConnectionState.Connected) client.Off(eventName);
    }
    #endregion

    #region Emission
    public void Emit(string eventName, params object[] parameters)
    {
        if (emissionTasks == null) emissionTasks = new();
        Task newTask = EmitAsync(emissionTasks.Count, eventName, parameters);
        emissionTasks.Add(newTask);
    }

    async private Task EmitAsync(int taskIndex, string eventName, params object[] parameters)
    {
        try
        {
            await client.EmitAsync(eventName, parameters);
        }
        catch (Exception e)
        {
            emissionError = e.Message;
            emissionTasks.RemoveAt(taskIndex);
        }
        finally
        {
            emissionTasks.RemoveAt(taskIndex);
        }
    }
    #endregion
}
