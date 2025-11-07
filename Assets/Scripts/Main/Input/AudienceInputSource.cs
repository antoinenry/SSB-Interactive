using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    [Header("Sources")]
    public ClientButtonTracker buttonTracker;
    public HttpRequestLoop playerCountRequest = new HttpRequestLoop(HttpRequest.RequestType.GET, "concert/crowd");
    public int substractedPlayerCount = 2;
    [Header("Axes")]
    public AxisConfiguration horizontalAxis;
    public AxisConfiguration verticalAxis;
    [Header("Velocity")]
    public float velocitySmoothDelay = 1f;
    public float velocityRounding = 1f;
    [Header("Events")]
    public UnityEvent onAudienceInput;

    private Dictionary<string,AudienceButtonInput> currentButtonInputs;

    public float DeltaTime { get; private set; }
    public MultipleButtonTimedCount CurrentInputFrame { get; private set; }
    public int PlayerCount { get; private set; }

    private void OnEnable()
    {
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
            CurrentInputFrame = buttonTracker.Current;
            DeltaTime = 0f;
            //AddFrameToBuffer(buttonTracker.Current);
        }
    }

    //private void OnButtonCountUpdate(MultipleButtonTimedCount frame)
    //{
    //    // Set current and previous frame
    //    MultipleButtonTimedCount previousFrame = CurrentFrame;
    //    CurrentFrame = frame;        
    //    float previousTime = previousFrame.time, currentTime = CurrentFrame.time;
    //    // Clear button inputs
    //    if (currentButtonInputs == null) currentButtonInputs = new();
    //    foreach (string k in currentButtonInputs.Keys.ToList()) currentButtonInputs[k] = AudienceButtonInput.None;
    //    // Set new button inputs
    //    if (frame.counts == null) return;
    //    if (firstButtonCountUpdate)
    //    {
    //        // Shortcup input delta for first update
    //        foreach (KeyValuePair<string, int> b in frame.counts)
    //            SetButtonInputs(b.Key, b.Value, b.Value, currentTime, currentTime);
    //        firstButtonCountUpdate = false;
    //    }
    //    else
    //    {
    //        foreach (KeyValuePair<string, int> b in frame.counts)
    //            SetButtonInputs(b.Key, previousFrame[b.Key], b.Value, previousTime, currentTime);
    //    }
    //    // Notify update
    //    onAudienceInput.Invoke();
    //}

    private void OnButtonCountUpdate(MultipleButtonTimedCount frame)
    {
        //AddFrameToBuffer(frame);
        //ClearOldBufferFrames();
        DeltaTime = frame.time - CurrentInputFrame.time;
        CurrentInputFrame = frame;
        UpdateCurrentButtonInputs();
        onAudienceInput.Invoke();
    }

    private void OnPlayerCountRequestEnd(HttpRequest request)
    {
        if (playerCountRequest == null || request.Status != HttpRequest.RequestStatus.Success) return;
        PlayerCount = playerCountRequest.DeserializeResponse<int>() - substractedPlayerCount;
        if (PlayerCount < 0) PlayerCount = 0;
    }

    private void UpdateCurrentButtonInputs()
    {
        // Clear button inputs delta
        if (currentButtonInputs == null) currentButtonInputs = new();
        string[] buttonIDs = currentButtonInputs.Keys.ToArray();
        foreach (string id in buttonIDs) currentButtonInputs[id].ClearDynamicValues();
        // Update dictionary
        Dictionary<string, int> buttonCountsUpdate = CurrentInputFrame.counts;
        buttonIDs = buttonCountsUpdate?.Keys?.ToArray();
        if (buttonIDs != null) foreach (string id in buttonIDs) SetButtonInput(id, buttonCountsUpdate[id]);
    }

    //private void AddFrameToBuffer(MultipleButtonTimedCount frame)
    //{
    //    if (buttonInputsBuffer == null) buttonInputsBuffer = new();
    //    buttonInputsBuffer.Enqueue(frame);
    //}

    //private void ClearOldBufferFrames()
    //{
    //    if (buttonInputsBuffer == null || buttonInputsBuffer.Count == 0) return;
    //    float currentTime = Time.time;
    //    MultipleButtonTimedCount oldestFrame = buttonInputsBuffer.Peek();
    //    while (oldestFrame.time < currentTime - bufferLifetime)
    //    {
    //        buttonInputsBuffer.Dequeue();
    //        oldestFrame = buttonInputsBuffer.Peek();
    //    }
    //}

    private void SetButtonInput(string id, int totalPresses)
    {
        // Update dictionnary
        if (currentButtonInputs == null) currentButtonInputs = new();
        bool hasPreviousCount = currentButtonInputs.TryGetValue(id, out AudienceButtonInput previousInput);
        float deltaPresses, velocity;
        if (hasPreviousCount)
        {
            deltaPresses = totalPresses - previousInput.totalPresses;
            if (DeltaTime <= 0f) velocity = 0f;
            else velocity = Mathf.Lerp(deltaPresses / DeltaTime, previousInput.velocity, Mathf.Clamp01(DeltaTime / velocitySmoothDelay));
            if (velocityRounding > 0f) velocity = Mathf.Round(velocity / velocityRounding) * velocityRounding;
            currentButtonInputs[id] = new AudienceButtonInput(totalPresses, deltaPresses, velocity);
        }
        else
        {
            deltaPresses = 0f;
            velocity = 0f;
            currentButtonInputs.Add(id, new AudienceButtonInput(totalPresses, deltaPresses, velocity));
        }
    }

    public float GetLastInputTime()
    {
        return CurrentInputFrame.time;
    }

    public AudienceButtonInput GetButton(string id)
    {
        if (currentButtonInputs == null) return AudienceButtonInput.None;
        if (currentButtonInputs.TryGetValue(id, out AudienceButtonInput b)) return b;
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