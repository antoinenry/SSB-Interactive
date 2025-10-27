using SocketIOClient;
using UnityEngine;
using UnityEngine.Events;

public class ConcertAdmin : MonoBehaviourSingleton<ConcertAdmin>
{
    [Header("Current concert")]
    public ConcertInfo info;
    public ConcertState state;
    [Header("Rest requests")]
    public HttpRequestLoop concertInfoRequest;
    public HttpRequestLoop concertStateRequest;
    public HttpRequestLoop pauseStateRequest;
    [Header("Socket events")]
    public string stateChangeEvent = "concert_state";
    public string pauseEvent = "pause";
    public string resumeEvent = "resume";
    [Header("Events")]
    public UnityEvent<ConcertInfo> onInfoUpdate;
    public UnityEvent<ConcertState> onStateUpdate;
    [Header("Editor Tools")]
    public ObjectMethodCaller editorButtons = new ObjectMethodCaller("RefreshConcertInfo", "RefreshConcertState", "ClearConcert");

    private bool pendingStateEvent;

    public HttpClientScriptable HttpClient { get; private set; }
    public SocketIOClientScriptable SocketClient { get; private set; }

    private void Awake()
    {
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
        if (pendingStateEvent)
        {
            pendingStateEvent = false;
            onStateUpdate.Invoke(state);
        }
    }

    private void AddClientListenners()
    {
        // Socket
        SocketClient?.Subscribe(stateChangeEvent, OnConcertStateSocketEvent);
        SocketClient?.Subscribe(pauseEvent, OnClientPauseSocketEvent);
        SocketClient?.Subscribe(resumeEvent, OnClientResumeSocketEvent);

    }

    private void RemoveClientListeners()
    {
        // Socket
        SocketClient.Unsubscribe(stateChangeEvent, OnConcertStateSocketEvent);
        SocketClient.Unsubscribe(pauseEvent, OnClientPauseSocketEvent);
        SocketClient.Unsubscribe(resumeEvent, OnClientResumeSocketEvent);
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
        info = ConcertInfo.None;
        if (concertInfoRequest != null)
        {
            concertInfoRequest.onRequestEnd.RemoveListener(OnConcertInfoRequestEnd);
            if (concertInfoRequest.RequestStatus != HttpRequest.RequestStatus.Success)
            {
                ConcertInfo newInfo = concertInfoRequest.DeserializeResponse<ConcertInfo>();
                if (newInfo != info)
                {
                    info = newInfo;
                    onInfoUpdate.Invoke(info);
                }
            }
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
        bool paused = state.paused;
        state = ConcertState.None;
        state.Paused = paused;
        // Update rest or the concert state with response
        if (concertStateRequest != null)
        {
            concertStateRequest.onRequestEnd.RemoveListener(OnConcertStateRequestEnd);
            if (concertStateRequest.RequestStatus != HttpRequest.RequestStatus.Success)
            {
                ConcertState newState = concertStateRequest.DeserializeResponse<ConcertState>();
                if (newState != state)
                {
                    state = newState;
                    onStateUpdate.Invoke(state);
                }
            }
        }
    }

    public void ClearConcert()
    {
        info = ConcertInfo.None;
        state = ConcertState.None;
        onInfoUpdate.Invoke(info);
        onStateUpdate.Invoke(state);
    }

    private void OnPauseStateRequestEnd(HttpRequest request)
    {
        if (pauseStateRequest != null)
        {
            pauseStateRequest.onRequestEnd.RemoveListener(OnPauseStateRequestEnd);
            if (pauseStateRequest.RequestStatus != HttpRequest.RequestStatus.Success)
                state.Paused = pauseStateRequest.DeserializeResponse<ConcertState.PauseState>().paused;
        }
    }

    private void OnConcertStateSocketEvent(string eventName, SocketIOResponse response)
    {
        if (eventName != stateChangeEvent) return;
        string dataString = response.GetValue<string>();
        ConcertState stateUpdate = SocketIOResponseEvent.DeserializeResponse<ConcertState>(dataString);
        // Socket events are async, which can cause error with UnityEvents. One solution is to wait for next update to send event (pending).
        if (state != stateUpdate)
        {
            state = stateUpdate;
            pendingStateEvent = true;
        }
    }

    private void OnClientPauseSocketEvent(string eventName, SocketIOResponse response)
    {
        state.Paused = true;
    }

    private void OnClientResumeSocketEvent(string eventName, SocketIOResponse response)
    {
        state.Paused = false;
    }
}