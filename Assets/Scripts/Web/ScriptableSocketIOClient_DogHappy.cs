// STATUS: Connexion ok sur herokuapp.com, échec sur smash3000. La demande de connexion ne se termine jamais.

using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;


[CreateAssetMenu(fileName = "SocketIOClient", menuName = "Client/SocketIOClient (doghappy)")]
public class ScriptableSocketIOClient_DogHappy : ScriptableObject
{
    public enum State { Disconneted, Connecting, Connected, Disconnecting, Error }

    public string serverUrl = "wss://socketio-chat-h9jt.herokuapp.com";
    public SocketIOClientOptions options = SocketIOClientOptions.Default;

    private SocketIOClient.SocketIO client;
    private Task connectionTask;

    public State CurrentState { get; private set; }

    private void OnDisable()
    {
        Disconnect();
    }

    public void Connect()
    {
        Debug.Log("Connecting...");
        CurrentState = State.Connecting;
        client = new SocketIOClient.SocketIO(serverUrl, options);
        client.OnConnected += OnConnected;
        client.OnReconnectAttempt += OnReconnect;
        client.OnDisconnected += OnDisconnected;
        client.OnError += OnError;
        connectionTask = ConnectAsync(CancellationToken.None);
    }

    private async Task ConnectAsync(CancellationToken cancelToken)
    {
        try
        {
            Debug.Log("Connexion...");
            await client.ConnectAsync();
        }
        catch (Exception e)
        {
            Debug.LogWarning("ConnctAsync exception: " + e);
        }
    }

    private void OnConnected(object sender, EventArgs e)
    {
        Debug.Log("..connected.");
        CurrentState = State.Connected;
    }

    private void OnReconnect(object sender, int e)
    {
        Debug.Log("..connecting...");
        CurrentState = State.Connecting;
    }

    private void OnDisconnected(object sender, string e)
    {
        Debug.Log("..disconnected.");
        CurrentState = State.Disconneted;
        if (client != null) client.Dispose();
        client = null;
    }

    private void OnError(object sender, string e)
    {
        Debug.LogWarning("..error: " + e);
        CurrentState = State.Error;
    }

    public void Disconnect()
    {
        if (client != null && client.Connected)
        {
            Debug.Log("Disconnecting...");
            CurrentState = State.Disconnecting;
            client.DisconnectAsync();
        }
        else OnDisconnected(null, null);
    }
}
