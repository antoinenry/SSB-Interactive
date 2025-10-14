using System;
using UnityEngine;
using UnityEngine.Events;

// A periodic client request: start, continue or stop, and process the response when there is one.

[Serializable]
public class HttpRequestLoop
{
    [Flags]
    public enum LoopBehaviour { NoLoop = 0, LoopOnFailure  = 1, LoopOnTimeout = 2, LoopOnSuccess = 4, InfiniteLoop = 8 };
    [Flags]
    public enum FailureFlag { NullClient = 1, RequestFailure = 2, Timeout = 4, MaxLoop = 8 }

    public string requestUri = "";
    public float requestTimeout = 1f;
    public LoopBehaviour loop;
    public int maxLoops = 0;
    public float minLoopDuration = .2f;

    public UnityEvent<HttpRequest> onSendRequest;
    public UnityEvent<HttpRequest> onClientResponse;
    public UnityEvent<HttpRequest,FailureFlag> onCancelRequest;

    [SerializeField] private HttpRequest.RequestStatus status;
    [SerializeField] private FailureFlag failureInfo;
    [SerializeField] private float responseTime;
    [SerializeField] private int loopCount;

    private HttpClientScriptable client;
    private HttpRequest request;
    private bool hasProcessedResponse;

    public HttpRequest.RequestStatus RequestStatus => status;
    public FailureFlag FailureInfo => failureInfo;
    public float ResponseTime => responseTime;
    public int LoopCount => loopCount;
    public float RequestsPerSeconds { get; private set; }


    public HttpRequestLoop(string uri)
    {
        requestUri = uri;
    }

    public string GetLog()
    {
        string logText = request.FullUri + " request status :";
        logText += "\n- response time : " + ResponseTime.ToString("0.000");
        logText += "\n- requests per seconds : " + RequestsPerSeconds.ToString("0.000");
        return logText;
    }

    public void Init()
    {
        CurrentAssetsManager.GetCurrent(ref client);
        request = new HttpRequest();
        Reset();
    }

    public void Reset()
    {
        loopCount = 0;
        failureInfo = 0;
        
    }

    public void Update()
    {
        if (client == null || request == null)
            Init();
        switch (request.Status)
        {
            case HttpRequest.RequestStatus.Created:
                // Launch request for the first time
                status = HttpRequest.RequestStatus.Running;
                Send();
                break;
            case HttpRequest.RequestStatus.Success:
                // Requess succest: get result, wait and relaunch
                if (hasProcessedResponse == false) Receive();
                if (loop.HasFlag(LoopBehaviour.LoopOnSuccess)) Loop();
                else status = HttpRequest.RequestStatus.Success;
                    break;
            case HttpRequest.RequestStatus.Failed:
                // Failed: notify and relaunch
                status = HttpRequest.RequestStatus.Failed;
                failureInfo |= FailureFlag.RequestFailure;
                responseTime = request.Duration;
                if (loop.HasFlag(LoopBehaviour.LoopOnFailure)) Loop();
                else Cancel();
                break;
            case HttpRequest.RequestStatus.Running:
                // Request timeout: notify and relaunch
                status = HttpRequest.RequestStatus.Running;
                if (request.Duration > requestTimeout)
                {
                    failureInfo |= FailureFlag.Timeout;
                    if (loop.HasFlag(LoopBehaviour.LoopOnTimeout)) Loop();
                    else Cancel();
                }
                break;
            case HttpRequest.RequestStatus.Canceled:
                // Waiting to launch a new loop after timeout
                if (status == HttpRequest.RequestStatus.Running) Loop();
                else status = HttpRequest.RequestStatus.Canceled;
                break;
        }
    }

    public T DeserializeResponse<T>() => request != null ? request.DeserializeResponse<T>() : default(T);

    private void Send()
    {
        RequestsPerSeconds = float.IsNaN(request.StartTime) ? 0f : 1f / (Time.time - request.StartTime);
        request.requestUri = requestUri;
        request.type = HttpRequest.RequestType.GET;
        if (client != null)
        {
            client.SendRequest(request);
            onSendRequest.Invoke(request);
        }
        else
        {
            failureInfo |= FailureFlag.NullClient;
        }
        hasProcessedResponse = false;
    }

    private void Loop()
    {
        // Already running, stop current request
        if (status == HttpRequest.RequestStatus.Running)
        {
            request.Cancel();
        }
        // Too much loops, fail
        if (loop.HasFlag(LoopBehaviour.InfiniteLoop) == false && loopCount >= maxLoops)
        {
            status = HttpRequest.RequestStatus.Failed;
            failureInfo |= FailureFlag.MaxLoop;
            Cancel();
            return;
        }
        // Wait and launch a new loop
        if (Time.time < request.StartTime + minLoopDuration)
        {
            status = HttpRequest.RequestStatus.Running;
        }
        else
        {
            loopCount++;
            failureInfo = 0;
            Send();
        }
    }

    private void Cancel()
    {
        request.Cancel();
        status = HttpRequest.RequestStatus.Canceled;
        onCancelRequest.Invoke(request, FailureInfo);
    }

    private void Receive()
    {
        responseTime = request.Duration;
        hasProcessedResponse = true;
        onClientResponse.Invoke(request);
    }
}