using System;
using UnityEngine;
using UnityEngine.Events;

// A periodic client request: start, continue or stop, and process the response when there is one.

[Serializable]
public class HttpRequestLoop
{
    [Flags]
    public enum FailureFlag { NullClient = 1, RequestFailure = 2, Timeout = 4, MaxLoop = 8 }

    public string requestUri = "";
    public float minimumRequestTime = .2f;
    public float maxRequestTime = 1f;
    public bool infiniteLoops = true;
    public int maxLoops = 0;

    public UnityEvent<HttpRequest> onSendRequest;
    public UnityEvent<HttpRequest> onClientResponse;
    public UnityEvent<HttpRequest,FailureFlag> onCancelRequest;

    private HttpClientScriptable client;
    private HttpRequest request;
    private bool hasProcessedLastResponse;

    public float ResponseTime { get; private set; }
    public float RequestsPerSeconds { get; private set; }
    public HttpRequest.RequestStatus RequestStatus { get; private set; }
    public int LoopCount { get; private set; }
    public FailureFlag FailureInfo { get; private set; }


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
        LoopCount = 0;
        FailureInfo = 0;
    }

    public void Update()
    {
        if (client == null || request == null)
            Init();
        switch (request.Status)
        {
            case HttpRequest.RequestStatus.Created:
                // Launch request for the first time
                Send();
                break;
            case HttpRequest.RequestStatus.Failed:
                // Failed: notify and relaunch
                Debug.LogWarning("Request failure");
                FailureInfo |= FailureFlag.RequestFailure;
                Cancel();
                Send();
                break;
            case HttpRequest.RequestStatus.Running:
                // Request timeout: notify and relaunch
                if (request.Duration > maxRequestTime)
                {
                    Debug.LogWarning("Request timeout");
                    FailureInfo |= FailureFlag.Timeout;
                    Cancel();
                    Send();
                }
                break;
            case HttpRequest.RequestStatus.Success:
                // Requess succest: get result, wait and relaunch
                if (hasProcessedLastResponse == false) Receive();
                if (Time.time >= request.StartTime + minimumRequestTime) Send();
                break;
        }
        RequestStatus = request.Status;
    }

    public T DeserializeResponse<T>() => request != null ? request.DeserializeResponse<T>() : default(T);

    private void Send()
    {
        if (!infiniteLoops && LoopCount >= maxLoops)
        {
            RequestStatus = HttpRequest.RequestStatus.Failed;
            FailureInfo |= FailureFlag.MaxLoop;
            Cancel();
            return;
        }
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
            FailureInfo |= FailureFlag.NullClient;
        }
        hasProcessedLastResponse = false;
        LoopCount += 1;
    }

    private void Cancel()
    {
        request.Cancel();
        onCancelRequest.Invoke(request, FailureInfo);
    }

    private void Receive()
    {
        ResponseTime = request.Duration;
        hasProcessedLastResponse = true;
        onClientResponse.Invoke(request);
    }
}