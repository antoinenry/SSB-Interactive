using UnityEngine;

// Behaviour to get input from the server and present it in a convenient way
public class InputSystem : MonoBehaviour
{
    public HttpClientScriptable client;
    public string buttonsRequestUri = "/buttons";
    public float minimumRequestTime = .2f;
    public float maxRequestTime = 1f;

    private HttpRequest buttonsRequest;
    private ButtonCountData[] buttonInputs;

    public float RequestTime { get; private set; }

    private void Awake()
    {
        buttonsRequest = new HttpRequest();
    }

    private void Update()
    {
        GetButtons();
    }

    private void GetButtons()
    {
        switch (buttonsRequest.Status)
        {
            case HttpRequest.RequestStatus.Created:
                // Launch request for the first time
                SendButtonRequest();
                break;
            case HttpRequest.RequestStatus.Failed:
                // Failed: notify and relaunch
                Debug.LogWarning("Button request timeout");
                CancelButtonRequest();
                SendButtonRequest();
                break;
            case HttpRequest.RequestStatus.Running:
                // Request timeout: notify and relaunch
                if (buttonsRequest.Duration > maxRequestTime)
                {
                    RequestTime = buttonsRequest.Duration;
                    Debug.LogWarning("Button request timeout");
                    CancelButtonRequest();
                    SendButtonRequest();
                }
                break;
            case HttpRequest.RequestStatus.Success:
                // Requess succest: get result, wait and relaunch
                if (Time.time < buttonsRequest.StartTime + minimumRequestTime)
                {
                    RequestTime = buttonsRequest.Duration;
                    ProcessButtonRequestResponse();
                }
                else
                    SendButtonRequest();
                break;
        }
    }

    private void SendButtonRequest()
    {
        buttonsRequest.requestUri = buttonsRequestUri;
        buttonsRequest.type = HttpRequest.RequestType.GET;
        if (client != null) client.SendRequest(buttonsRequest);
    }

    private void CancelButtonRequest()
    {
        buttonsRequest.Cancel();
    }

    private void ProcessButtonRequestResponse()
    {
        string response = buttonsRequest.ResponseBody;
        buttonInputs = ButtonCountData.Deserialize(response);
    }
}
