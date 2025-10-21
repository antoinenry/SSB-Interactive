using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static ClientButtonTracker;

public class AudienceInputSource : MonoBehaviour
{
    public struct ButtonInput
    {
        public int totalPresses;
        public float deltaPresses;
        public float deltaTime;

        public ButtonInput(int total, float delta_presses, float delta_time)
        {
            totalPresses = total;
            deltaPresses = delta_presses;
            deltaTime = delta_time;
        }

        public static ButtonInput None => new ButtonInput(0, 0, 0);

        public static ButtonInput Lerp(ButtonInput from, ButtonInput to, float t) =>  new(
            (int)Mathf.Lerp(from.Total, to.Total, t), 
            Mathf.Lerp(from.deltaPresses, to.deltaPresses, t), 
            Mathf.Lerp(from.deltaTime, to.deltaTime, t)
            );

        public int Total => totalPresses;
        public bool Pressed => deltaPresses > 0;
        public float Velocity => deltaTime != 0f ? deltaPresses / deltaTime : 0f;
    }

    public ClientButtonTracker buttonTracker;
    [Range(0f, 1f)] public float smoothRate = 1f;
    public AudienceInputConfiguration inputConfiguration;

    public UnityEvent onAudienceInput;

    private Dictionary<string,ButtonInput> buttonInputsRaw;
    private Dictionary<string, ButtonInput> buttonInputsSmooth;
    private bool firstButtonCountUpdate;

    public MultipleButtonTimedCount CurrentFrame { get; private set; }

    private void OnEnable()
    {
        firstButtonCountUpdate = true;
        AddButtonListeners();
    }

    private void OnDisable()
    {
        RemoveButtonListeners();
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
        if (buttonInputsRaw == null) buttonInputsRaw = new();
        foreach (string k in buttonInputsRaw.Keys.ToList()) buttonInputsRaw[k] = ButtonInput.None;
        if (buttonInputsSmooth == null) buttonInputsSmooth = new();
        foreach (string k in buttonInputsSmooth.Keys.ToList()) buttonInputsSmooth[k] = ButtonInput.None;
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

    private void SetButtonInputs(string id, int previousCount, int currentCount, float previousTime, float currentTime)
    {
        // Add keys if needed
        if (buttonInputsRaw == null) buttonInputsRaw = new();
        if (buttonInputsRaw.ContainsKey(id) == false) buttonInputsRaw.Add(id, ButtonInput.None);
        if (buttonInputsSmooth == null) buttonInputsSmooth = new();
        if (buttonInputsSmooth.ContainsKey(id) == false) buttonInputsSmooth.Add(id, ButtonInput.None);
        // Update raw and smoothes input
        ButtonInput oldInputRaw = ButtonInput.None,
            newInputRaw = new ButtonInput(currentCount, currentCount - previousCount, currentTime - previousTime);
        buttonInputsRaw[id] = newInputRaw;
        buttonInputsSmooth[id] = ButtonInput.Lerp(oldInputRaw, newInputRaw, 1f - .5f * smoothRate);
    }

    private float ComputeAxis(float negativeValue, float positiveValue)
    {
        if (negativeValue > positiveValue) return -negativeValue / (negativeValue + positiveValue);
        else if (positiveValue > negativeValue) return positiveValue / (negativeValue + positiveValue);
        else return 0f;
    }

    // Get Buttons and Axis using InputConfig
    public float GetButton(string buttonId)
    {
        if (inputConfiguration == null) return GetButton(buttonId, ButtonValueType.RawTotal);
        AudienceInputConfiguration.Button buttonConfig = inputConfiguration.GetButtonConfig(buttonId);
        return GetButton(buttonId, buttonConfig.type);
    }

    public float GetAxis(AudienceInputConfiguration.Axis.Direction direction)
    {
        if (inputConfiguration == null) return 0f;
        AudienceInputConfiguration.Axis axisConfig = inputConfiguration.GetAxisConfig(direction);
        return GetAxis(axisConfig.negativeButtonId, axisConfig.positiveButtonId);
    }

    // Get Buttons and Axis by specifying config parameters
    public ButtonInput GetButtonRaw(string buttonId) => buttonInputsRaw != null && buttonInputsRaw.ContainsKey(buttonId) ? buttonInputsRaw[buttonId] : ButtonInput.None;

    public ButtonInput GetButtonSmooth(string buttonId) => buttonInputsSmooth != null && buttonInputsSmooth.ContainsKey(buttonId) ? buttonInputsSmooth[buttonId] : ButtonInput.None;

    public float GetButton(string buttonId, ButtonValueType buttonConfig)
    {
        ButtonInput input = buttonConfig.smooth ? GetButtonSmooth(buttonId) : GetButtonRaw(buttonId);
        switch (buttonConfig.value)
        {
            case ButtonValueType.ValueType.Total: return input.Total;
            case ButtonValueType.ValueType.Delta: return input.deltaPresses;
            case ButtonValueType.ValueType.Velocity: return input.Velocity;
            default: return 0f;
        }
    }

    public float GetAxisRaw(string negativeButtonId, string positiveButtonId) => ComputeAxis(GetButtonRaw(negativeButtonId).deltaPresses, GetButtonRaw(positiveButtonId).deltaPresses);

    public float GetAxisSmooth(string negativeButtonId, string positiveButtonId) => ComputeAxis(GetButtonSmooth(negativeButtonId).deltaPresses, GetButtonSmooth(positiveButtonId).deltaPresses);

    public float GetAxis(string negativeButtonId, string positiveButtonId, bool smooth) => smooth ? GetAxisSmooth(negativeButtonId, positiveButtonId) : GetAxisRaw(negativeButtonId, positiveButtonId);

    public float GetAxis(string negativeButtonId, string positiveButtonId) => ComputeAxis(GetButton(negativeButtonId), GetButton(positiveButtonId));

    public bool EnableButtonTracker { get => buttonTracker != null && buttonTracker.enabled; set { if (buttonTracker) buttonTracker.enabled = value; } }
}