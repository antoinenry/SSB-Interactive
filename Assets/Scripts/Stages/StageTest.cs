using SocketIOClient;
using UnityEditor.PackageManager;
using UnityEngine;

public class StageTest : MonoBehaviour
{
    public SocketIOClientScriptable client;
    public string stageEvent = "stage";
    public string currentStage;

    private void OnEnable()
    {
        client.Subscribe(stageEvent, OnClientRequestsStage);
        if (client.Connection != ConnectionState.Connected) client.Connect();
    }

    private void OnDisable()
    {
        client.Unsubscribe(stageEvent, OnClientRequestsStage);
    }

    private void OnClientRequestsStage(string eventName, SocketIOResponse response) => SetStage(response.GetValue<string>());

    public void SetStage(string stageName) => currentStage = stageName;
}
