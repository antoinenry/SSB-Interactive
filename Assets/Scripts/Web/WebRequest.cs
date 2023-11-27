using UnityEngine;
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

    public RequestType type = RequestType.GET;
    public string requestUri = "";
    public string mediaType = "application/json";
    public string requestBody = "body";

    private Task task;
    private HttpClient client;

    public TaskStatus Status => task != null ? task.Status : TaskStatus.WaitingForActivation;
    public string FullUri { get; private set; }
    public string ResponseBody { get; private set; }

    public void SendFrom(HttpClient httpClient, string serverUrl = "")
    {
        client = httpClient;
        FullUri = serverUrl + requestUri;
        ResponseBody = "";
        task = RequestAsync();
    }

    private async Task RequestAsync()
    {
        try
        {
            switch (type)
            {
                case RequestType.GET:
                    ResponseBody = await client.GetStringAsync(FullUri);
                    break;
                case RequestType.POST:
                    await client.PostAsync(FullUri, new StringContent(requestBody, Encoding.Default, mediaType));
                    break;
            }
        }
        catch (HttpRequestException e)
        {
            Debug.LogWarning("Exception Caught!");
            Debug.LogWarning("Message : " + e.Message);
        }
    }
}