using System.Collections.Generic;
using UnityEngine;

// Behaviour to get audience input from the server and measure it
public class InputCounter : MonoBehaviour
{
    private struct Capture
    {
        public float time;
        public Dictionary<string, int> totalPresses;

        public Capture(float time, ButtonCountData[] data)
        {
            this.time = time;
            int dataCount = data != null ? data.Length : 0;
            totalPresses = new Dictionary<string, int>(dataCount);
            for (int i = 0; i < dataCount; i++)
            {
                ButtonCountData d = data[i];
                if (totalPresses.ContainsKey(d.ButtonID)) totalPresses[d.ButtonID] += d.InputCount;
                else totalPresses.Add(d.ButtonID, d.InputCount);
            }
        }

        static public int CompareByAge(Capture a, Capture b) => b.time.CompareTo(a.time);

        public void AddPresses(Dictionary<string, int> presses)
        {
            if (presses == null) return;
            if (totalPresses == null)
            {
                totalPresses = new(presses);
                return;
            }
            foreach (string key in presses.Keys)
            {
                if (totalPresses.ContainsKey(key)) totalPresses[key] += presses[key];
                else totalPresses.Add(key, presses[key]);
            }
        }
    }

    public string buttonsRequestUri = "/buttons";
    public float minimumRequestTime = .2f;
    public float maxRequestTime = 1f;
    public float timeWindow = 1f;
    public float smoothRateUp = 0f;
    public float smoothRateDown = .5f;

    private HttpClientScriptable client;
    private HttpRequest buttonsRequest;
    private List<Capture> captures;
    private List<ButtonTimeSpawnData> buttonCounts;
    private Dictionary<string, float> buttonRatesRaw;
    private Dictionary<string, float> buttonRatesSmooth;

    public float UpdateTime { get; private set; }
    public float RequestDuration { get; private set; }
    public float TimeBetweenRequests { get; private set; }
    public List<string> ButtonIDs { get; private set; }
    public ButtonTimeSpawnData[] ButtonCounts => buttonCounts != null ? buttonCounts.ToArray() : new ButtonTimeSpawnData[0];

    #region Unity Callbacks
    private void Awake()
    {
        CurrentAssetsManager.GetCurrent(ref client);
        buttonsRequest = new HttpRequest();
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
    #endregion

    #region Sending/Processing Requests
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
        ButtonCountData[] data = ButtonCountData.Deserialize(response);
        AddCapture(UpdateTime, data);
        captures?.RemoveAll(f => f.time < UpdateTime - timeWindow);
    }

    private void AddCapture(float time, ButtonCountData[] data)
    {
        if (captures == null) captures = new List<Capture>();
        Capture c = new(time, data);
        int captureIndex = captures.FindIndex(f => f.time == c.time);
        if (captureIndex != -1)
        {
            c.AddPresses(captures[captureIndex].totalPresses);
            captures[captureIndex] = c;
        }
        else
        {
            captures.Add(c);
            captures.Sort(Capture.CompareByAge);
        }
    }
    #endregion

    #region Updating Measurements
    private void UpdateButtonsCounts() => buttonCounts = GetButtonCounts(UpdateTime - timeWindow, UpdateTime);
    
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
            float currentRate = 0f;
            if (buttonRatesSmooth.ContainsKey(buttonID)) currentRate = buttonRatesSmooth[buttonID];
            else buttonRatesSmooth.Add(buttonID, 0f);
            float newRate = buttonRatesRaw[buttonID];
            if (newRate > currentRate && smoothRateUp > 0f)
                buttonRatesSmooth[buttonID] = Mathf.MoveTowards(currentRate, buttonRatesRaw[buttonID], Time.deltaTime / smoothRateUp);
            else if (newRate < currentRate && smoothRateDown > 0f)
                buttonRatesSmooth[buttonID] = Mathf.MoveTowards(currentRate, buttonRatesRaw[buttonID], Time.deltaTime / smoothRateDown);
            else
                buttonRatesSmooth[buttonID] = buttonRatesRaw[buttonID];
        }
    }
    #endregion

    #region Get Measurements
    public ButtonTimeSpawnData GetButtonData(string buttonID)
    {
        int buttonIndex = buttonCounts != null ? buttonCounts.FindIndex(b => b.buttonID == buttonID) : -1;
        return buttonIndex != -1 ? buttonCounts[buttonIndex] : new ButtonTimeSpawnData();
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

    public List<ButtonTimeSpawnData> GetButtonCounts(float fromTime, float toTime)
    {
        List<ButtonTimeSpawnData> getButtonCounts = new List<ButtonTimeSpawnData>();
        if (captures == null) return getButtonCounts;
        List<Capture> timedCaptures = captures.FindAll(f => f.time >= fromTime && f.time <= toTime);
        foreach (Capture c in timedCaptures)
        {
            if (c.totalPresses == null) continue;
            foreach (string buttonID in c.totalPresses.Keys)
            {
                int buttonIndex = getButtonCounts.FindIndex(b => b.buttonID == buttonID);
                if (buttonIndex == -1)
                {
                    getButtonCounts.Add(new(buttonID, c.time, c.totalPresses[buttonID]));
                }
                else
                {
                    ButtonTimeSpawnData delta = getButtonCounts[buttonIndex];
                    delta.AddCountAtTime(c.totalPresses[buttonID], c.time);
                    getButtonCounts[buttonIndex] = delta;
                }
            }
        }
        return getButtonCounts;
    }
    #endregion
}