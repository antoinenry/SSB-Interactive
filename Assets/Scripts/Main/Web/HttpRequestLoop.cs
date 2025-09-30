using System;
using UnityEngine;
using UnityEngine.Events;

// A periodic client request: start, continue or stop, and process the response when there is one.

[Serializable]
public class HttpRequestLoop
{
    public string requestUri = "";
    public float minimumRequestTime = .2f;
    public float maxRequestTime = 1f;

    public UnityEvent<HttpRequest> onSendRequest;
    public UnityEvent<HttpRequest> onClientResponse;

    private HttpClientScriptable client;
    private HttpRequest request;
    private bool hasProcessedLastResponse;

    public float ResponseTime { get; private set; }
    public float RequestsPerSeconds { get; private set; }

    public HttpRequestLoop(string uri)
    {
        requestUri = uri;
    }

    public void Init()
    {
        CurrentAssetsManager.GetCurrent(ref client);
        request = new HttpRequest();
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
                Cancel();
                Send();
                break;
            case HttpRequest.RequestStatus.Running:
                // Request timeout: notify and relaunch
                if (request.Duration > maxRequestTime)
                {
                    Debug.LogWarning("Request timeout");
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
    }

    public string GetLog()
    {
        string logText = request.FullUri + " request status :";
        logText += "\n- response time : " + ResponseTime.ToString("0.000");
        logText += "\n- requests per seconds : " + RequestsPerSeconds.ToString("0.000");
        return logText;

    }

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
        hasProcessedLastResponse = false;
    }

    private void Cancel()
    {
        request.Cancel();
    }

    private void Receive()
    {
        ResponseTime = request.Duration;
        hasProcessedLastResponse = true;
        onClientResponse.Invoke(request);
    }
}