using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

using static ClientButtonTracker;

public class AudienceInputSource : MonoBehaviourSingleton<AudienceInputSource>
{
    [Serializable]
    public struct AxisConfiguration
    {
        public string axisID;
        public string negativeButtonID;
        public string positiveButtonID;
    }

    public ClientButtonTracker buttonTracker;
    public AxisConfiguration horizontalAxis;
    public AxisConfiguration verticalAxis;
    public HttpRequestLoop playerCountRequest = new HttpRequestLoop(HttpRequest.RequestType.GET, "concert/crowd");
    public int substractedPlayerCount = 2;
    public UnityEvent onAudienceInput;

    private Dictionary<string,AudienceButtonInput> buttonInputs;
    private bool firstButtonCountUpdate;

    public MultipleButtonTimedCount CurrentFrame { get; private set; }
    public int PlayerCount { get; private set; }

    private void OnEnable()
    {
        firstButtonCountUpdate = true;
        AddButtonListeners();
        StartPlayerCounter();
    }

    private void OnDisable()
    {
        RemoveButtonListeners();
        StopPlayerCounter();
    }

    private void AddButtonListeners()
    {
        if (buttonTracker == null) return;
        buttonTracker.onSetEnabled.AddListener(OnSetButtonTrackerEnabled);
        buttonTracker.onCountUpdate.AddListener(OnButtonCountUpdate);

    }

    private void RemoveButtonListeners()
    {
        if (buttonTracker == null) return;
        buttonTracker.onSetEnabled.RemoveListener(OnSetButtonTrackerEnabled);
        buttonTracker.onCountUpdate.RemoveListener(OnButtonCountUpdate);
    }

    private void StartPlayerCounter()
    {
        if (playerCountRequest == null) return;
        playerCountRequest.Init();
        playerCountRequest.StartRequestCoroutine(this);
        playerCountRequest.onRequestEnd.AddListener(OnPlayerCountRequestEnd);
    }

    private void StopPlayerCounter()
    {
        if (playerCountRequest == null) return;
        playerCountRequest.StopRequestCoroutine();
        playerCountRequest.onRequestEnd.RemoveListener(OnPlayerCountRequestEnd);
    }

    private void OnSetButtonTrackerEnabled(bool buttonTrackerEnabled)
    {
        if (buttonTrackerEnabled == true)
        {
            CurrentFrame = buttonTracker.Current;
            firstButtonCountUpdate = true;
        }
    }

    private void OnButtonCountUpdate(MultipleButtonTimedCount frame)
    {
        // Set current and previous frame
        MultipleButtonTimedCount previousFrame = CurrentFrame;
        CurrentFrame = frame;        
        float previousTime = previousFrame.time, currentTime = CurrentFrame.time;
        // Clear button inputs
        if (buttonInputs == null) buttonInputs = new();
        foreach (string k in buttonInputs.Keys.ToList()) buttonInputs[k] = AudienceButtonInput.None;
        // Set new button inputs
        if (frame.counts == null) return;
        if (firstButtonCountUpdate)
        {
            // Shortcup input delta for first update
            foreach (KeyValuePair<string, int> b in frame.counts)
                SetButtonInputs(b.Key, b.Value, b.Value, currentTime, currentTime);
            firstButtonCountUpdate = false;
        }
        else
        {
            foreach (KeyValuePair<string, int> b in frame.counts)
                SetButtonInputs(b.Key, previousFrame[b.Key], b.Value, previousTime, currentTime);
        }
        // Notify update
        onAudienceInput.Invoke();
    }

    private void OnPlayerCountRequestEnd(HttpRequest request)
    {
        if (playerCountRequest == null || request.Status != HttpRequest.RequestStatus.Success) return;
        PlayerCount = playerCountRequest.DeserializeResponse<int>() - substractedPlayerCount;
        if (PlayerCount < 0) PlayerCount = 0;
    }

    private void SetButtonInputs(string id, int previousCount, int currentCount, float previousTime, float currentTime)
    {
        // Add keys if needed
        if (buttonInputs == null) buttonInputs = new();
        if (buttonInputs.ContainsKey(id) == false) buttonInputs.Add(id, AudienceButtonInput.None);
        // Update dictionnary
        buttonInputs[id] = new AudienceButtonInput(currentCount, currentCount - previousCount, 0f);
    }

    public float GetLastInputTime()
    {
        return CurrentFrame.time;
    }

    public AudienceButtonInput GetButton(string id)
    {
        if (buttonInputs == null) return AudienceButtonInput.None;
        if (buttonInputs.TryGetValue(id, out AudienceButtonInput b)) return b;
        else return AudienceButtonInput.None;
    }

    public AudienceButtonInput GetAxis(string negativeButtonID,  string positiveButtonID)
    {
        return AudienceButtonInput.GetAxis(GetButton(negativeButtonID), GetButton(positiveButtonID));
    }
    public AudienceButtonInput GetAxis(AxisConfiguration axis) => GetAxis(axis.negativeButtonID, axis.positiveButtonID);
    public AudienceButtonInput GetHorizontalAxis() => GetAxis(horizontalAxis);
    public AudienceButtonInput GetVerticalAxis() => GetAxis(verticalAxis);
}