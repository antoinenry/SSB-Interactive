﻿using UnityEngine;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

// Class representing a standard HTTP request
// Allow to configure, run and track a request
// Will be instanciated by various Unity component (like the Input System), and run by the WebClient component
[Serializable]
public class WebRequest
{
    public enum RequestType { GET, POST }
    public enum RequestStatus { Created, Running, Success, Failed }

    public RequestType type = RequestType.GET;
    public string requestUri = "";
    public string mediaType = "application/json";
    public string requestBody = "";

    private Task task;
    private HttpClient client;

    public string FullUri { get; private set; }
    public float StartTime { get; private set; }
    public float EndTime { get; private set; }
    public string ResponseBody { get; private set; }
    public RequestStatus Status { get; private set; }

    public WebRequest()
    {
        task = null;
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
        task = RequestAsync();
    }

    private async Task<HttpResponseMessage> RequestAsync()
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
                    response = await client.GetAsync(FullUri); 
                    response.EnsureSuccessStatusCode();
                    ResponseBody = await response.Content.ReadAsStringAsync();
                    break;
                case RequestType.POST:
                    response = await client.PostAsync(FullUri, new StringContent(requestBody, Encoding.Default, mediaType));
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
        finally
        {
            EndTime = Time.time;
            if (Status != RequestStatus.Failed) Status = RequestStatus.Success;
        }
        return response;
    }
}