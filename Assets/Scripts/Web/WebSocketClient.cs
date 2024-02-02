using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.WebSockets;
using static HttpRequest;
using System.Text;
using UnityEditor.Media;
using UnityEditor.PackageManager;
using System.Net.Sockets;

[CreateAssetMenu(fileName = "WebSocketClient", menuName = "Web/WebSocketClient")]
public class WebSocketClient : SingleScriptableObject
{
    protected override SingleScriptableObject CurrentObject { get => Current; set => Current = value as WebSocketClient; }
    static public WebSocketClient Current;

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
            Debug.Log("..exception: " + e);
        }
        finally
        {
            Debug.Log("...connected");
        }
    }

    private async Task<WebSocketReceiveResult> ReceiveAsync(CancellationToken cancelToken)
    {
        WebSocketReceiveResult response = null;
        try
        {
            byte[] recvBuffer = new byte[64 * 1024];
            int i = 0;
            while (State == WebSocketState.Open)
            {
                Debug.Log("Receive..." + i++);
                response = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(recvBuffer), CancellationToken.None);
            }
        }
        catch (TaskCanceledException)
        {

        }
        finally
        {

        }
        connectionPendingTask = null;
        return response;
    }
}
