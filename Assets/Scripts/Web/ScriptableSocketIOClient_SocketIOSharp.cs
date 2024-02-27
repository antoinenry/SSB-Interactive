// STATUS: Echec de connexion. Déconnexion immédiate, pas d'erreur.

using UnityEngine;
using EngineIOSharp.Common.Enum;
using SocketIOSharp.Client;
using SocketIOSharp.Common;
using Newtonsoft.Json.Linq;


[CreateAssetMenu(fileName = "SocketIOClient2", menuName = "Client/SocketIOClient (socketiosharp)")]
public class ScriptableSocketIOClient_SocketIOSharp : ScriptableObject
{
    public string host = "wss://socketio-chat-h9jt.herokuapp.com";
    public EngineIOScheme scheme = EngineIOScheme.https;
    public ushort port = 443;

    private SocketIOSharp.Client.SocketIOClient socketIOSharpClient;

    public void Connect()
    {
        Dispose();
        SocketIOClientOption options = new SocketIOClientOption
            (Scheme:scheme, Host:host, Port:port);
        socketIOSharpClient = new SocketIOSharp.Client.SocketIOClient(options);
        

        socketIOSharpClient.On(SocketIOEvent.CONNECTION, () =>
        {
            Debug.Log("Connected!");
        });

        socketIOSharpClient.On(SocketIOEvent.DISCONNECT, () =>
        {
            Debug.Log("Disconnected!");
        });

        socketIOSharpClient.On(SocketIOEvent.ERROR, (JToken[] Data) => // Type of argument is JToken[].
        {
            if (Data != null && Data.Length > 0 && Data[0] != null)
            {
                Debug.Log("Error : " + Data[0]);
            }
            else
            {
                Debug.Log("Unkown Error");
            }
        });

        socketIOSharpClient.Connect();
        Debug.Log("Connecting...");
    }

    public void Dispose()
    {
        if (socketIOSharpClient != null) socketIOSharpClient.Dispose();
    }
}
