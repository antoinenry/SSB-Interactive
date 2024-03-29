using System;
using System.Collections.Generic;
using UnityEngine;

// Behaviour to get input from the server and present it in a convenient way
public class InputSystem : MonoBehaviour
{
    public string buttonsRequestUri = "/buttons";
    public float minimumRequestTime = .2f;
    public float maxRequestTime = 1f;
    public float timeWindow = 1f;

    private HttpClientScriptable client;
    private HttpRequest buttonsRequest;
    private ButtonCounter buttonCounter;

    public float UpdateTime { get; private set; }
    public float RequestDuration { get; private set; }
    public float TimeBetweenRequests { get; private set; }
    public List<string> ButtonIDs { get; private set; }

    private void Awake()
    {
        CurrentAssetsManager.GetCurrent(ref client);
        buttonsRequest = new HttpRequest();
        buttonCounter = new ButtonCounter();
        ButtonIDs = new List<string>();
    }

    private void Update()
    {
        UpdateTime = Time.time;
        RequestButtons();
    }

    private void RequestButtons()
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
                    TimeBetweenRequests = buttonsRequest.Duration;
                    Debug.LogWarning("Button request timeout");
                    CancelButtonRequest();
                    SendButtonRequest();
                }
                break;
            case HttpRequest.RequestStatus.Success:
                // Requess succest: get result, wait and relaunch
                if (UpdateTime >= buttonsRequest.StartTime + minimumRequestTime)
                {
                    TimeBetweenRequests = UpdateTime - buttonsRequest.StartTime;
                    RequestDuration = buttonsRequest.Duration;
                    ProcessButtonRequestResponse();
                    SendButtonRequest();
                }
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
        buttonCounter.Add(UpdateTime, response);
        buttonCounter.ClearCapturesBefore(UpdateTime - timeWindow);
    }

    public List<ButtonTimeSpawnData> GetWindow() => buttonCounter?.GetButtonCounts(UpdateTime - timeWindow, UpdateTime);

    
}
