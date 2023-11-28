using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public string buttonsRequestUri = "/buttons";
    public float minimumRequestTime = .2f;
    public float maxRequestTime = 1f;
    public string response;

    private WebRequest buttonsRequest;


    private void Awake()
    {
        buttonsRequest = new WebRequest();
    }

    private void Update()
    {
        GetButtons();
    }

    private void GetButtons()
    {
        switch (buttonsRequest.Status)
        {
            case WebRequest.RequestStatus.Created:
                // Launch request for the first time
                SendButtonRequest();
                break;
            case WebRequest.RequestStatus.Failed:
                // Failed: notify and relaunch
                Debug.LogWarning("Button request timeout");
                CancelButtonRequest();
                SendButtonRequest();
                break;
            case WebRequest.RequestStatus.Running:
                // Wait while request is running
                if (Time.time < buttonsRequest.StartTime + maxRequestTime)
                    response = "(wait)";
                // Request timeout: notify and relaunch
                else
                {
                    Debug.LogWarning("Button request timeout");
                    CancelButtonRequest();
                    SendButtonRequest();
                }
                break;
            case WebRequest.RequestStatus.Success:
                // Requess succest: get result, wait and relaunch
                if (Time.time < buttonsRequest.StartTime + minimumRequestTime)
                    response = buttonsRequest.ResponseBody;
                else
                    SendButtonRequest();
                break;
        }
    }

    private void SendButtonRequest()
    {
        buttonsRequest.requestUri = buttonsRequestUri;
        buttonsRequest.type = WebRequest.RequestType.GET;
        if (WebClient.current != null) WebClient.current.SendRequest(buttonsRequest);
    }

    private void CancelButtonRequest()
    {

    }
}
