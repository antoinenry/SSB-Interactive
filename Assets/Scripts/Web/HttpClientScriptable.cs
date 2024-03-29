using UnityEngine;
using System.Net.Http;

// Manages the HTTP client (one per application is sufficient)
// Used to send requests
[CreateAssetMenu(fileName = "HttpClient", menuName = "Client/HttpClient")]
public class HttpClientScriptable : ScriptableObject
{
    [CurrentToggle] public bool isCurrent;
    public string serverURL = "https://smash3000.ovh/";

    private HttpClient httpClient;

    public void SendRequest(HttpRequest request, bool useServerURL = true)
    {
        if (useServerURL) request.SendFrom(httpClient, serverURL);
        else request.SendFrom(httpClient);
    }

    private void OnEnable()
    {
        if (httpClient == null) httpClient = new HttpClient();
    }

    private void OnDisable()
    {
        if (httpClient != null)  httpClient.CancelPendingRequests();
    }
}
