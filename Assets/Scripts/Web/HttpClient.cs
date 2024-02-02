using UnityEngine;

// Manages the HTTP client (one per application is sufficient)
// Used to send requests
[CreateAssetMenu(fileName = "HttpClient", menuName = "Web/HttpClient")]
public class HttpClient : SingleScriptableObject
{
    protected override SingleScriptableObject CurrentObject { get => Current; set => Current = value as HttpClient; }
    static public HttpClient Current;

    public string defaultServerUrl = "https://smash3000.ovh/";

    private System.Net.Http.HttpClient httpClient;

    public void SendRequest(HttpRequest request, bool useServerURL = true)
    {
        if (useServerURL) request.SendFrom(httpClient, defaultServerUrl);
        else request.SendFrom(httpClient);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (httpClient == null)
        {
            httpClient = new System.Net.Http.HttpClient();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (httpClient != null)
        {
            httpClient.CancelPendingRequests();
        }
    }
}
