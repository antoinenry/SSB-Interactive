using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Text.Json;

// Class representing a standard HTTP request
// Allow to configure, run and track a request
// Will be instanciated by various Unity component (like the Input System), and run by the WebClient component
[Serializable]
public class HttpRequest
{
    public enum RequestType { GET, POST, PUT }
    public enum RequestStatus { Created, Running, Success, Failed, Canceled }

    public RequestType type = RequestType.GET;
    public string requestUri = "";
    public string mediaType = "application/json";
    public string requestBody = "";

    private HttpClient client;
    private Task<HttpResponseMessage> pendingTask;
    private CancellationTokenSource cancelSource;

    public string FullUri { get; private set; }
    public float StartTime { get; private set; }
    public float EndTime { get; private set; }
    public string ResponseBody { get; private set; }
    public RequestStatus Status { get; private set; }

    public HttpRequest()
    {
        // Initialize thread
        pendingTask = null;
        cancelSource = null;
        // Initialize parameters
        client = null;
        StartTime = float.NaN;
        EndTime = float.NaN;
        Status = RequestStatus.Created;
    }

    public float Duration
    {
        get
        {
            if (float.IsNaN(StartTime)) return 0f;
            else if (float.IsNaN(EndTime)) return Time.time - StartTime;
            else return EndTime - StartTime;
        }
    }

    public void SendFrom(HttpClient httpClient, string serverUrl = "")
    {
        // Set request parameters
        client = httpClient;
        FullUri = serverUrl + requestUri;
        // Run request on a separate thread
        cancelSource = new CancellationTokenSource();
        pendingTask = RequestAsync(cancelSource.Token);
    }

    public void Cancel()
    {
        cancelSource.Cancel();
    }

    private async Task<HttpResponseMessage> RequestAsync(CancellationToken cancelToken)
    {
        // Initialize status and response
        HttpResponseMessage response = null;
        StartTime = Time.time;
        EndTime = float.NaN;
        ResponseBody = "";
        Status = RequestStatus.Running;
        try
        {
            switch (type)
            {
                case RequestType.GET:
                    response = await client.GetAsync(FullUri, cancelToken);
                    response.EnsureSuccessStatusCode();
                    ResponseBody = await response.Content.ReadAsStringAsync();
                    break;
                case RequestType.POST:
                    response = await client.PostAsync(FullUri, new StringContent(requestBody, Encoding.Default, mediaType), cancelToken);
                    response.EnsureSuccessStatusCode();
                    break;
                case RequestType.PUT:
                    response = await client.PutAsync(FullUri, new StringContent(requestBody, Encoding.Default, mediaType), cancelToken);
                    response.EnsureSuccessStatusCode();
                    break;
            }
        }
        catch (HttpRequestException e)
        {
            Debug.LogWarning("Exception Caught!");
            Debug.LogWarning("Message : " + e.Message);
            Status = RequestStatus.Failed;
        }
        catch (TaskCanceledException)
        {
            Status = RequestStatus.Canceled;
        }
        finally
        {
            EndTime = Time.time;
            if (Status == RequestStatus.Running) Status = RequestStatus.Success;
        }
        pendingTask = null;
        return response;
    }

    public T DeserializeResponse<T>()
    {
        if (ResponseBody == null || ResponseBody.Length == 0) return default;
        try 
        {
            return JsonSerializer.Deserialize<T>(ResponseBody);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Deserialize exception: " + e);
            Debug.Log("...when deserializing " + ResponseBody);
            return default;
        }
    }

    public void SerializeBody<T>(T data) => requestBody = JsonSerializer.Serialize(data);
}