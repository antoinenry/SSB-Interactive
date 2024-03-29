using System.Collections.Generic;
using UnityEngine;

// Behaviour to get input from the server and present it in a convenient way
public class InputSystem : MonoBehaviour
{
    public string buttonsRequestUri = "/buttons";
    public float minimumRequestTime = .2f;
    public float maxRequestTime = 1f;
    public float timeWindow = 1f;
    public float smoothRates = 1f;

    private HttpClientScriptable client;
    private HttpRequest buttonsRequest;
    private ButtonCounter buttonCounter;
    private List<ButtonTimeSpawnData> buttonCounts;
    private Dictionary<string, float> buttonRatesRaw;
    private Dictionary<string, float> buttonRatesSmooth;

    public float UpdateTime { get; private set; }
    public float RequestDuration { get; private set; }
    public float TimeBetweenRequests { get; private set; }
    public List<string> ButtonIDs { get; private set; }
    public ButtonTimeSpawnData[] ButtonCounts => buttonCounts != null ? buttonCounts.ToArray() : new ButtonTimeSpawnData[0];

    private void Awake()
    {
        CurrentAssetsManager.GetCurrent(ref client);
        buttonsRequest = new HttpRequest();
        buttonCounter = new ButtonCounter();
        ButtonIDs = new List<string>();
        buttonRatesRaw = new Dictionary<string, float>();
        buttonRatesSmooth = new Dictionary<string, float>();
    }

    private void Update()
    {
        UpdateTime = Time.time;
        RequestButtons();
        UpdateButtonsCounts();
        UpdateButtonsRatesRaw();
        UpdateButtonsRatesSmooth();
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

    private void UpdateButtonsCounts() => buttonCounts = buttonCounter?.GetButtonCounts(UpdateTime - timeWindow, UpdateTime);
    
    private void UpdateButtonsRatesRaw()
    {
        List<string> keys = new(buttonRatesRaw.Keys);
        foreach (string buttonID in keys) buttonRatesRaw[buttonID] = 0f;
        if (buttonCounts == null) return;
        foreach (ButtonTimeSpawnData b in buttonCounts)
        {
            if (buttonRatesRaw.ContainsKey(b.buttonID)) buttonRatesRaw[b.buttonID] = b.Rate;
            else buttonRatesRaw.Add(b.buttonID, b.Rate);
        }
    }

    private void UpdateButtonsRatesSmooth()
    {
        if (buttonCounts == null) return;
        List<string> keys = new(buttonRatesRaw.Keys);
        foreach (string buttonID in keys)
        {
            if (smoothRates > 0f)
            {
                float currentRate = 0f;
                if (buttonRatesSmooth.ContainsKey(buttonID)) currentRate = buttonRatesSmooth[buttonID];
                else buttonRatesSmooth.Add(buttonID, 0f);
                buttonRatesSmooth[buttonID] = Mathf.MoveTowards(currentRate, buttonRatesRaw[buttonID], Time.deltaTime / smoothRates);
            }
            else
                buttonRatesSmooth[buttonID] = buttonRatesRaw[buttonID];
        }
    }    

    public float GetButtonRateRaw(string buttonID)
    {
        if (buttonRatesRaw != null && buttonRatesRaw.ContainsKey(buttonID)) return buttonRatesRaw[buttonID];
        else return 0f;
    }

    public float GetButtonRateSmooth(string buttonID)
    {
        if (buttonRatesSmooth != null && buttonRatesSmooth.ContainsKey(buttonID)) return buttonRatesSmooth[buttonID];
        else return 0f;
    }
}