// STATUS: Connection ok
// Abandon car Smash3000 utilise SocketIO

using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;

[CreateAssetMenu(fileName = "WebSocketClient", menuName = "Client/WebSocketClient")]
public class WebSocketClientScriptable : ScriptableObject
{
    public string serverUrl = "smash3000.ovh/socket.io";

    private ClientWebSocket clientWebSocket;
    private Task connectionPendingTask;
    private CancellationTokenSource connectionCancelSource;
    private Task emissionPendingTask;
    private CancellationTokenSource emissionCancelSource;

    public WebSocketState State => clientWebSocket != null ? clientWebSocket.State : 0;

    public void Connect()
    {
        if (clientWebSocket == null) clientWebSocket = new ClientWebSocket();
        connectionCancelSource = new CancellationTokenSource();
        connectionPendingTask = ConnectAsync(connectionCancelSource.Token);
    }

    public void Disconnect()
    {
        if (clientWebSocket == null || connectionCancelSource == null) return;
        connectionCancelSource.Cancel();
        clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
    }

    public void Send(string message)
    {
        throw new Exception("Send not implemented");
    }

    public void CancelSend()
    {
        throw new Exception("Cancel not implemented");
    }

    private async Task ConnectAsync(CancellationToken cancelToken)
    {
        try
        {            
            Debug.Log("Connexion...");
            await clientWebSocket.ConnectAsync(new(serverUrl), cancelToken);
        }
        catch (Exception e)
        {
            Debug.LogWarning("..exception: " + e);
        }
        finally
        {
            if (State == WebSocketState.Open) Debug.Log("...connected");
            else Debug.Log("...end");
        }
    }
}
