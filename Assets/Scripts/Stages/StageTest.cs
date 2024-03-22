using SocketIOClient;
using UnityEngine;

public class StageTest : MonoBehaviour
{
    public string stageEvent = "stage";
    public string currentStage;

    private SocketIOClientScriptable client;

    private void Awake()
    {
        client = CurrentAssetsManager.GetCurrent<SocketIOClientScriptable>();
    }

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
