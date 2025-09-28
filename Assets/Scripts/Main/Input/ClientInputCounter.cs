using System;
using System.Collections.Generic;
using UnityEngine;

//
//

[Serializable]
public class ClientInputCounter
{   
    public string buttonsRequestUri = "buttons";
    public float minimumRequestTime = .2f;
    public float maxRequestTime = 1f;
    public float timeWindow = 1f;

    private HttpClientScriptable client;
    private HttpRequest buttonsRequest;
    private List<MultipleButtonsCount> responseButtonCounts;
    private List<SingleButtonCountOverTime> updateButtonCounts;

    public float UpdateTime { get; private set; }
    public float RequestDuration { get; private set; }
    public float TimeBetweenRequests { get; private set; }

    public void InitClient()
    {
        CurrentAssetsManager.GetCurrent(ref client);
        buttonsRequest = new HttpRequest();
    }

    public void UpdateCount()
    {
        UpdateTime = Time.time;
        RequestButtons();
        updateButtonCounts = GetButtonCountsOverTime(UpdateTime - timeWindow, UpdateTime);
        responseButtonCounts?.RemoveAll(f => f.time < UpdateTime - timeWindow);
    }

    #region Sending/Processing Requests
    private void RequestButtons()
    // Update client request: start, continue or stop, and process the response when there is one.
    {
        if (client == null || buttonsRequest == null)
            InitClient();
        switch (buttonsRequest.Status)
        {
            case HttpRequest.RequestStatus.Created:
                // Launch request for the first time
                SendButtonRequest();
                break;
            case HttpRequest.RequestStatus.Failed:
                // Failed: notify and relaunch
                Debug.LogWarning("Button request failure");
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
        SingleButtonCount[] data = buttonsRequest.DeserializeResponse<SingleButtonCount[]>();
        AddCountData(UpdateTime, data);
    }

    private void AddCountData(float time, SingleButtonCount[] data)
    {
        if (responseButtonCounts == null) responseButtonCounts = new List<MultipleButtonsCount>();
        MultipleButtonsCount c = new(time, data);
        int timeIndex = responseButtonCounts.FindIndex(f => f.time == c.time);
        if (timeIndex != -1)
        {
            c.AddPresses(responseButtonCounts[timeIndex].count);
            responseButtonCounts[timeIndex] = c;
        }
        else
        {
            responseButtonCounts.Add(c);
            responseButtonCounts.Sort(MultipleButtonsCount.CompareByAge);
        }
    }
    #endregion

    #region Get Counts
    public SingleButtonCountOverTime[] GetCurrentButtonCounts()
        => updateButtonCounts != null ? updateButtonCounts.ToArray() : new SingleButtonCountOverTime[0];

    public SingleButtonCountOverTime GetCurrentButtonCount(string buttonID)
        => updateButtonCounts != null ? updateButtonCounts.Find(b => b.buttonID == buttonID) : new();

    public SingleButtonCountOverTime[] GetCurrentButtonCounts(params string[] buttonIDs)
        => (updateButtonCounts != null && buttonIDs != null) ? 
        updateButtonCounts.FindAll(b => Array.IndexOf(buttonIDs, b.buttonID) != -1).ToArray() : 
        new SingleButtonCountOverTime[0];

    public List<SingleButtonCountOverTime> GetButtonCountsOverTime(float fromTime, float toTime)
    {
        List<SingleButtonCountOverTime> getButtonCounts = new List<SingleButtonCountOverTime>();
        if (responseButtonCounts == null) return getButtonCounts;
        List<MultipleButtonsCount> timedCaptures = responseButtonCounts.FindAll(f => f.time >= fromTime && f.time <= toTime);
        foreach (MultipleButtonsCount c in timedCaptures)
        {
            if (c.count == null) continue;
            foreach (string buttonID in c.count.Keys)
            {
                int buttonIndex = getButtonCounts.FindIndex(b => b.buttonID == buttonID);
                if (buttonIndex == -1)
                {
                    getButtonCounts.Add(new(buttonID, c.time, c.count[buttonID]));
                }
                else
                {
                    SingleButtonCountOverTime delta = getButtonCounts[buttonIndex];
                    delta.AddCountAtTime(c.count[buttonID], c.time);
                    getButtonCounts[buttonIndex] = delta;
                }
            }
        }
        return getButtonCounts;
    }
    #endregion
}