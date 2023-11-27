using UnityEngine;
using System.Net.Http;

// Manages the HTTP client (one per application is sufficient)
// Used to send requests
public class WebClient : MonoBehaviour
{
    public string defaultServerUrl = "https://smash3000.ovh/";

    private HttpClient httpClient;

    public void SendRequest(WebRequest request, bool useServerURL = true)
    {
        if (useServerURL) request.SendFrom(httpClient, defaultServerUrl);
        else request.SendFrom(httpClient);
    }

    private void Awake()
    {
        httpClient = new HttpClient();
    }

    private void OnDestroy()
    {
        httpClient.CancelPendingRequests();
    }
}
