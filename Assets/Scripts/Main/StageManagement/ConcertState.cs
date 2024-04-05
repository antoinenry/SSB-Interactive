using SocketIOClient;
using UnityEngine;

public class ConcertState : MonoBehaviour
{
    public string changeEvent = "concert_state";
    public StageLoader stageLoader;
    public ConcertStateData concertState;
    public ObjectMethodCaller editorButtons = new ObjectMethodCaller("ValidateConcertState");

    public SocketIOClientScriptable Client { get; private set; }
    private bool pendingClientResponse;

    private void Awake()
    {
        Client = CurrentAssetsManager.GetCurrent<SocketIOClientScriptable>();
    }

    private void OnEnable()
    {
        Client.Subscribe(changeEvent, OnClientReponse);
    }

    private void OnDisable()
    {
        Client.Unsubscribe(changeEvent, OnClientReponse);
    }

    private void Update()
    {
        if (pendingClientResponse)
        {
            ValidateConcertState();
            pendingClientResponse = false;
        }
    }

    private void OnClientReponse(string eventName, SocketIOResponse response)
    {
        string dataString = response.GetValue<string>();
        concertState = ConcertStateData.Deserialize(dataString);
        pendingClientResponse = true;
    }

    public void ValidateConcertState()
    {
        stageLoader?.LoadStage(serverStageName: concertState.StageName);
    }
}
