using System;
using System.Collections;
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
    public enum ParameterFormat { Path, Query }

    public HttpRequest.RequestType requestType;
    public string requestUri = "";
    public string[] parameters;
    public ParameterFormat parametersFormat;
    public float requestTimeout = 1f;
    public LoopBehaviour loop;
    public int maxLoops = 0;
    public float minLoopDuration = .2f;

    public UnityEvent<HttpRequest> onRequestSend;
    public UnityEvent<HttpRequest> onRequestEnd;

    [SerializeField] private HttpRequest.RequestStatus status;
    [SerializeField] private FailureFlag failureInfo;
    [SerializeField] private float responseTime;
    [SerializeField] private int loopCount;
    [SerializeField] private string responseBody;

    private HttpClientScriptable client;
    private HttpRequest request;
    private bool hasProcessedResponse;
    private Coroutine updateCoroutine;
    private MonoBehaviour updateCoroutineHost;

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
        Reset();
    }

    public void Reset()
    {
        loopCount = 0;
        failureInfo = 0;
        request = new HttpRequest();
        status = HttpRequest.RequestStatus.Created;
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
                else Fail();
                break;
            case HttpRequest.RequestStatus.Running:
                // Request timeout: notify and relaunch
                status = HttpRequest.RequestStatus.Running;
                if (request.Duration > requestTimeout)
                {
                    failureInfo |= FailureFlag.Timeout;
                    if (loop.HasFlag(LoopBehaviour.LoopOnTimeout)) Loop();
                    else Fail();
                }
                break;
            case HttpRequest.RequestStatus.Canceled:
                // Waiting to launch a new loop after timeout
                if (status == HttpRequest.RequestStatus.Running) Loop();
                else status = HttpRequest.RequestStatus.Canceled;
                break;
        }
    }

    public void SetParameter(int index, string value, bool trimExcess  = true)
    {
        if (index >= 0)
        {
            if (parameters == null) parameters = new string[index + 1];
            else if (index >= parameters.Length) Array.Resize(ref parameters, index + 1);
            parameters[index] = value;
        }
        if (trimExcess) TrimExcessParameters();
    }

    public void TrimExcessParameters()
    {
        if (parameters == null) return;
        int parameterCount = parameters.Length;
        for (; parameterCount > 0; parameterCount--)
            if (parameters[parameterCount - 1] != null && parameters[parameterCount - 1].Length > 0) break;
        Array.Resize(ref parameters, parameterCount);
    }

    public string GetUriWithParameters()
    {
        int uriLength = requestUri != null ? requestUri.Length : 0;
        if (uriLength == 0) return "";
        int parameterCount = parameters != null ? parameters.Length : 0;
        int parameterIndex = 0;
        string fullUri = "";

        switch (parametersFormat)
        {
            case ParameterFormat.Path:
                for (int i = 0; i < uriLength; i++)
                {
                    if (requestUri[i] == '{')
                    {
                        if (parameterIndex < parameterCount) fullUri += parameters[parameterIndex++];
                        while (i < uriLength && requestUri[i] != '}') i++;
                    }
                    else
                    {
                        fullUri += requestUri[i];
                    }
                }
                break;

            case ParameterFormat.Query:
                for (int i = 0; i < uriLength; i++)
                {
                    if (i < uriLength - 1 && requestUri[i + 1] == '{')
                    {
                        fullUri += "?";
                        for (i += 2; i < uriLength; i++)
                        {
                            if (requestUri[i] == '}') break;
                            fullUri += requestUri[i];
                        }
                        fullUri += "=";
                        if (parameterIndex < parameterCount) fullUri += parameters[parameterIndex++];
                        else fullUri += "null";
                    }
                    else
                    {
                        fullUri += requestUri[i];
                    }
                }
                break;
        }

        return fullUri;
    }

    private void Send()
    {
        RequestsPerSeconds = float.IsNaN(request.StartTime) ? 0f : 1f / (Time.time - request.StartTime);
        request.requestUri = GetUriWithParameters();
        request.type = requestType;
        if (client != null)
        {
            client.SendRequest(request);
            onRequestSend.Invoke(request);
        }
        else
        {
            failureInfo |= FailureFlag.NullClient;
        }
        hasProcessedResponse = false;
        responseBody = null;
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

    public void Cancel()
    {
        request.Cancel();
        status = HttpRequest.RequestStatus.Canceled;
        onRequestEnd.Invoke(request);
    }

    private void Fail()
    {
        status = HttpRequest.RequestStatus.Failed;
        if (request != null)
        {
            if (request.Status == HttpRequest.RequestStatus.Running)
                request.Cancel();
            else
                onRequestEnd.Invoke(request);
        }
        else
            onRequestEnd.Invoke(null);
    }

    private void Receive()
    {
        responseTime = request.Duration;
        hasProcessedResponse = true;
        responseBody = request.ResponseBody;
        onRequestEnd.Invoke(request);
    }

    public void StartRequestCoroutine(MonoBehaviour host, bool restart = false)
    {
        if (restart && updateCoroutine != null && updateCoroutineHost != null) updateCoroutineHost.StopCoroutine(updateCoroutine);
        updateCoroutineHost = host;
        updateCoroutine = host?.StartCoroutine(UpdateCoroutine());
    }

    public void StopRequestCoroutine()
    {
        if (updateCoroutine != null && updateCoroutineHost != null) updateCoroutineHost?.StopCoroutine(updateCoroutine);
        updateCoroutineHost = null;
        updateCoroutine = null;
    }

    private IEnumerator UpdateCoroutine()
    {
        if (RequestStatus != HttpRequest.RequestStatus.Running) Init();
        do
        {
            Update();
            yield return null;
        }
        while (RequestStatus == HttpRequest.RequestStatus.Running);
    }

    public T DeserializeResponse<T>()
    {
        if (request != null)
        {
            return request.DeserializeResponse<T>();
        }
        else
        {
            return default(T);
        }
    }
}