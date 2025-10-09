using SocketIOClient;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ConcertClient : MonoBehaviour
{
    [Header("Concert Info")]
    public string concertInfoRequestUri = "concert/today";
    public ConcertInfoData concertInfo;
    [Header("Crowd info")]
    public HttpRequestLoop crowdInfoRequestLoop = new("concert/crowd");
    public int crowdSize = 0;
    [Header("Concert Progress")]
    public string stateChangeEvent = "concert_state";
    public ConcertStateData concertState;
    public string pauseEvent = "pause";
    public string resumeEvent = "resume";
    [Header("Editor Tools")]
    public ObjectMethodCaller editorButtons = new ObjectMethodCaller("GetConcertInfo", "ValidateConcertState");
    [Header("Events")]
    public UnityEvent onClientConnected;
    public UnityEvent onClientDisconnected;

    private StageLoader stageLoader;
    private HttpRequest concertInfoRequest;
    private bool pendingConcertState;
    private bool pendingPauseState;
    private bool paused;

    public HttpClientScriptable HttpClient { get; private set; }
    public SocketIOClientScriptable SocketClient { get; private set; }

    private void Awake()
    {
        stageLoader = FindObjectOfType<StageLoader>(true);
        HttpClient = CurrentAssetsManager.GetCurrent<HttpClientScriptable>();
        concertInfoRequest = new();
        crowdInfoRequestLoop?.Init();
        SocketClient = CurrentAssetsManager.GetCurrent<SocketIOClientScriptable>();
        SocketClient.Connect();
    }

    private void OnEnable()
    {
        GetConcertInfo();
        AddClientListenners();
    }

    private void OnDisable()
    {
        RemoveClientListeners();
    }

    private void Update()
    {
        crowdInfoRequestLoop?.Update();
        if (stageLoader)
        {
            if (pendingConcertState) ValidateConcertState();
            if (pendingPauseState) stageLoader.Pause = paused;
        }
        pendingConcertState = false;
        pendingPauseState = false;
    }

    private void AddClientListenners()
    {
        // Rest
        if (crowdInfoRequestLoop != null) crowdInfoRequestLoop.onClientResponse.AddListener(OnCrowdInfoRequestResponse);
        // Socket
        SocketClient.onConnected.AddListener(OnClientSocketConnected);
        SocketClient.onDisconnected.AddListener(OnClientSocketDisconnected);
        SocketClient.Subscribe(stateChangeEvent, OnConcertStateSocketEvent);
        SocketClient.Subscribe(pauseEvent, OnClientPauseSocketEvent);
        SocketClient.Subscribe(resumeEvent, OnClientResumeSocketEvent);

    }

    private void RemoveClientListeners()
    {
        // Rest
        if (crowdInfoRequestLoop != null) crowdInfoRequestLoop.onClientResponse.RemoveListener(OnCrowdInfoRequestResponse);
        // Socket
        SocketClient.onConnected.RemoveListener(OnClientSocketConnected);
        SocketClient.onDisconnected.RemoveListener(OnClientSocketDisconnected);
        SocketClient.Unsubscribe(stateChangeEvent, OnConcertStateSocketEvent);
        SocketClient.Unsubscribe(pauseEvent, OnClientPauseSocketEvent);
        SocketClient.Unsubscribe(resumeEvent, OnClientResumeSocketEvent);
    }

    private void OnCrowdInfoRequestResponse(HttpRequest request)
    {
        if (request == null) return;
        crowdSize = request.DeserializeResponse<int>();
    }

    private void OnClientSocketConnected()
    {
        onClientConnected.Invoke();
    }

    private void OnClientSocketDisconnected()
    {
        onClientDisconnected.Invoke();
    }

    private void OnConcertStateSocketEvent(string eventName, SocketIOResponse response)
    {
        string dataString = response.GetValue<string>();
        concertState = ConcertStateData.Deserialize(dataString);
        pendingConcertState = true;
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

    private IEnumerator GetConcertInfoCoroutine()
    {
        concertInfoRequest.requestUri = concertInfoRequestUri;
        concertInfoRequest.type = HttpRequest.RequestType.GET;
        if (HttpClient != null) HttpClient.SendRequest(concertInfoRequest);
        yield return new WaitUntil(() => concertInfoRequest.Status == HttpRequest.RequestStatus.Success);
        concertInfo = concertInfoRequest.DeserializeResponse<ConcertInfoData>();
        stageLoader.mainScore.publicName = concertInfo.Location;
    }

    public void GetConcertInfo()
    {
        StartCoroutine(GetConcertInfoCoroutine());
    }

    public void ValidateConcertState()
    {
        if (stageLoader)
        {
            stageLoader.LoadStage(serverStageName: concertState.stage, moment: concertState.Moment);
            if (paused) stageLoader.Pause = true;
        }
    }
}
