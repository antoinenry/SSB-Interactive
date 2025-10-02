using System.Collections.Generic;
using UnityEngine;
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

    public ClientButtonTracker buttonCounter;
    [Range(0f, 1f)] public float smoothRate = 1f;
    public AudienceInputConfiguration inputConfiguration;

    private Dictionary<string,ButtonInput> buttonDeltasRaw;
    private Dictionary<string, ButtonInput> buttonDeltasSmooth;

    public MultipleButtonTimedCount CurrentFrame { get; private set; }

    private void OnEnable() => AddButtonListeners();

    private void OnDisable() => RemoveButtonListeners();

    private void AddButtonListeners()
    {
        if (buttonCounter == null) return;
        buttonCounter.onCountUpdate.AddListener(OnButtonCountUpdate);

    }

    private void RemoveButtonListeners()
    {
        if (buttonCounter == null) return;
        buttonCounter.onCountUpdate.RemoveListener(OnButtonCountUpdate);
    }

    private void OnButtonCountUpdate(MultipleButtonTimedCount frame)
    {
        // Set current and previous frame
        MultipleButtonTimedCount previousFrame = CurrentFrame;
        CurrentFrame = frame;        
        float previousTime = previousFrame.time, currentTime = CurrentFrame.time;
        // Clear button deltas
        foreach (string k in buttonDeltasRaw.Keys)
            buttonDeltasRaw[k] = ButtonInput.None;
        foreach (string k in buttonDeltasSmooth.Keys)
            buttonDeltasSmooth[k] = ButtonInput.None;
        // Set new button deltas
        foreach (KeyValuePair<string, int> b in frame.counts)
            SetButtonDeltas(b.Key, previousFrame[b.Key], b.Value, previousTime, currentTime); 
    }

    private void SetButtonDeltas(string id, int previousCount, int currentCount, float previousTime, float currentTime)
    {
        // Add keys if needed
        if (buttonDeltasRaw == null) buttonDeltasRaw = new();
        if (buttonDeltasRaw.ContainsKey(id) == false) buttonDeltasRaw.Add(id, ButtonInput.None);
        if (buttonDeltasSmooth == null) buttonDeltasSmooth = new();
        if (buttonDeltasSmooth.ContainsKey(id) == false) buttonDeltasSmooth.Add(id, ButtonInput.None);
        // Update raw and smoothes input
        ButtonInput oldInputRaw = ButtonInput.None,
            newInputRaw = new ButtonInput(currentCount, currentCount - previousCount, currentTime - previousTime);
        buttonDeltasRaw[id] = newInputRaw;
        buttonDeltasSmooth[id] = ButtonInput.Lerp(oldInputRaw, newInputRaw, .5f * (1f - smoothRate));
    }

    private float ComputeAxis(float negativeValue, float positiveValue)
    {
        if (negativeValue < positiveValue) return negativeValue / (negativeValue + positiveValue);
        else if (negativeValue > positiveValue) return positiveValue / (negativeValue + positiveValue);
        else return 0f;
    }

    // Get Buttons and Axis using InputConfig
    public float GetButton(string buttonId)
    {
        if (inputConfiguration == null) return GetButton(buttonId, ButtonValueType.Default);
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
    public ButtonInput GetButtonRaw(string buttonId) => buttonDeltasRaw != null && buttonDeltasRaw.ContainsKey(buttonId) ? buttonDeltasRaw[buttonId] : ButtonInput.None;

    public ButtonInput GetButtonSmooth(string buttonId) => buttonDeltasSmooth != null && buttonDeltasSmooth.ContainsKey(buttonId) ? buttonDeltasSmooth[buttonId] : ButtonInput.None;

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

    
}