using SocketIOClient;
using UnityEngine;

public class Concert : MonoBehaviour
{
    public ConcertInfo concertInfo;
    public ConcertState concertState;
    [Header("Rest requests")]
    public HttpRequestLoop concertInfoRequest;
    public HttpRequestLoop concertStateRequest;
    public HttpRequestLoop pauseStateRequest;
    [Header("Socket events")]
    //public string stateChangeEvent = "concert_state";
    public string pauseEvent = "pause";
    public string resumeEvent = "resume";
    [Header("Editor Tools")]
    public ObjectMethodCaller editorButtons = new ObjectMethodCaller("RefreshConcertInfo", "RefreshConcertState");

    private StageLoader stageLoader;

    private bool pendingConcertState;
    private bool pendingPauseState;
    private bool paused;

    public HttpClientScriptable HttpClient { get; private set; }
    public SocketIOClientScriptable SocketClient { get; private set; }

    private void Awake()
    {
        stageLoader = FindObjectOfType<StageLoader>(true);
        HttpClient = CurrentAssetsManager.GetCurrent<HttpClientScriptable>();
        SocketClient = CurrentAssetsManager.GetCurrent<SocketIOClientScriptable>();
        SocketClient.Connect();
    }

    private void OnEnable()
    {
        AddClientListenners();
        RefreshConcertInfo();
        RefreshConcertState();
    }

    private void OnDisable()
    {
        RemoveClientListeners();
    }

    private void Update()
    {
        //if (stageLoader)
        //{
        //    if (pendingConcertState) ValidateConcertState();
        //    if (pendingPauseState) stageLoader.Pause = paused;
        //}
        //pendingConcertState = false;
        //pendingPauseState = false;
    }

    private void AddClientListenners()
    {
        // Socket
        //SocketClient?.Subscribe(stateChangeEvent, OnConcertStateSocketEvent);
        //SocketClient?.Subscribe(pauseEvent, OnClientPauseSocketEvent);
        //SocketClient?.Subscribe(resumeEvent, OnClientResumeSocketEvent);

    }

    private void RemoveClientListeners()
    {        
        // Socket
        //SocketClient.Unsubscribe(stateChangeEvent, OnConcertStateSocketEvent);
        //SocketClient.Unsubscribe(pauseEvent, OnClientPauseSocketEvent);
        //SocketClient.Unsubscribe(resumeEvent, OnClientResumeSocketEvent);
    }

    public void RefreshConcertInfo()
    {
        if (concertInfoRequest != null)
        {
            concertInfoRequest.onRequestEnd.AddListener(OnConcertInfoRequestEnd);
            concertInfoRequest.StartRequestCoroutine(this);
        }
    }

    private void OnConcertInfoRequestEnd(HttpRequest request)
    {
        concertInfo = ConcertInfo.None;
        if (concertInfoRequest != null)
        {
            concertInfoRequest.onRequestEnd.RemoveListener(OnConcertInfoRequestEnd);
            if (concertInfoRequest.RequestStatus != HttpRequest.RequestStatus.Success)
                concertInfo = concertInfoRequest.DeserializeResponse<ConcertInfo>();
        }
    }

    public void RefreshConcertState()
    {
        if (concertStateRequest != null)
        {
            concertStateRequest.onRequestEnd.AddListener(OnConcertStateRequestEnd);
            concertStateRequest.StartRequestCoroutine(this);
        }
        if (pauseStateRequest != null)
        {
            pauseStateRequest.onRequestEnd.AddListener(OnPauseStateRequestEnd);
            pauseStateRequest.StartRequestCoroutine(this);
        }
    }

    private void OnConcertStateRequestEnd(HttpRequest request)
    {
        // Preserve pause state (refreshed on a different request for some reason)
        bool paused = concertState.paused;
        concertState = ConcertState.None;
        concertState.Paused = paused;
        // Update rest or the concert state with response
        if (concertStateRequest != null)
        {
            concertStateRequest.onRequestEnd.RemoveListener(OnConcertStateRequestEnd);
            if (concertStateRequest.RequestStatus != HttpRequest.RequestStatus.Success)
                concertState = concertStateRequest.DeserializeResponse<ConcertState>();
        }
    }

    private void OnPauseStateRequestEnd(HttpRequest request)
    {
        if (pauseStateRequest != null)
        {
            pauseStateRequest.onRequestEnd.RemoveListener(OnPauseStateRequestEnd);
            if (pauseStateRequest.RequestStatus != HttpRequest.RequestStatus.Success)
                concertState.Paused = pauseStateRequest.DeserializeResponse<ConcertState.PauseState>().paused;
        }
    }

    private void OnConcertStateSocketEvent(string eventName, SocketIOResponse response)
    {
        //string dataString = response.GetValue<string>();
        //concertState = ConcertStateData.Deserialize(dataString);
        //pendingConcertState = true;
    }

    private void OnClientPauseSocketEvent(string eventName, SocketIOResponse response)
    {
        paused = true;
        pendingPauseState = true;
    }

    private void OnClientResumeSocketEvent(string eventName, SocketIOResponse response)
    {
        paused = false;
        pendingPauseState = true;
    }    

    public void ValidateConcertState()
    {
        //if (stageLoader)
        //{
        //    stageLoader.LoadStage(serverStageName: concertState.stage, moment: concertState.Moment);
        //    if (paused) stageLoader.Pause = true;
        //}
    }
}