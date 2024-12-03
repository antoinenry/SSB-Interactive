using SocketIOClient;
using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class ConcertClient : MonoBehaviour
{
    public string infoRequestUri = "concert/today";
    public ConcertInfoData info;
    public string stateChangeEvent = "concert_state";
    public ConcertStateData state;
    public string pauseEvent = "pause";
    public string resumeEvent = "resume";
    public ObjectMethodCaller editorButtons = new ObjectMethodCaller("GetConcertInfo", "ValidateConcertState");

    private StageLoader stageLoader;
    private HttpRequest infoRequest;
    private bool pendingConcertState;
    private bool pendingPauseState;
    private bool paused;

    public HttpClientScriptable HttpClient { get; private set; }
    public SocketIOClientScriptable SocketClient { get; private set; }

    private void Awake()
    {
        stageLoader = FindObjectOfType<StageLoader>(true);
        HttpClient = CurrentAssetsManager.GetCurrent<HttpClientScriptable>();
        infoRequest = new();
        SocketClient = CurrentAssetsManager.GetCurrent<SocketIOClientScriptable>();
        SocketClient.Connect();
    }

    private void OnEnable()
    {
        GetConcertInfo();
        SocketClient.Subscribe(stateChangeEvent, OnClientReponse);
        SocketClient.Subscribe(pauseEvent, OnClientPause);
        SocketClient.Subscribe(resumeEvent, OnClientResume);
    }

    private void OnDisable()
    {
        SocketClient.Unsubscribe(stateChangeEvent, OnClientReponse);
        SocketClient.Unsubscribe(pauseEvent, OnClientPause);
        SocketClient.Unsubscribe(resumeEvent, OnClientResume);
    }

    private void Update()
    {
        if (stageLoader)
        {
            if (pendingConcertState) ValidateConcertState();
            if (pendingPauseState) stageLoader.Pause = paused;
        }
        pendingConcertState = false;
        pendingPauseState = false;
    }

    private void OnClientReponse(string eventName, SocketIOResponse response)
    {
        string dataString = response.GetValue<string>();
        state = ConcertStateData.Deserialize(dataString);
        pendingConcertState = true;
    }

    private void OnClientPause(string eventName, SocketIOResponse response)
    {
        paused = true;
        pendingPauseState = true;
    }

    private void OnClientResume(string eventName, SocketIOResponse response)
    {
        paused = false;
        pendingPauseState = true;
    }

    private IEnumerator GetConcertInfoCoroutine()
    {
        infoRequest.requestUri = infoRequestUri;
        infoRequest.type = HttpRequest.RequestType.GET;
        if (HttpClient != null) HttpClient.SendRequest(infoRequest);
        yield return new WaitUntil(() => infoRequest.Status == HttpRequest.RequestStatus.Success);
        info = infoRequest.DeserializeResponse<ConcertInfoData>();
        stageLoader.mainScore.publicName = info.Location;
    }

    public void GetConcertInfo()
    {
        StartCoroutine(GetConcertInfoCoroutine());
    }

    public void ValidateConcertState()
    {
        if (stageLoader)
        {
            stageLoader.LoadStage(serverStageName: state.stage, moment: state.Moment);
            if (paused) stageLoader.Pause = true;
        }
    }
}
