using UnityEngine;
using UnityEngine.Events;
using System;

public class AudienceButtonListener : MonoBehaviour
{
    public string buttonID = "a";
    public ButtonConfiguration configuration;
    public UnityEvent<float,float> onValueChange;
    public UnityEvent onValueMaxed;

    public enum ValueType { Delta, Velocity, Total, Counter }

    [Serializable]
    public struct ButtonConfiguration
    {
        [Header("Input")]
        public ValueType inputType;
        public bool smoothInput;
        [Header("Range")]
        public float maxValue;
        public bool clampZeroMax;
        [Header("Autopress")]
        public bool enableAutoPress;
        public float autoPressSpeed;
        [Min(0f)] public float autoPressTriggerDelay;
    }

    public float InputValue { get; private set; }
    public float OutputValue { get; private set; }
    public float StaticOutputValue { get; private set; }

    private float autoPressTimer;

    private void Awake()
    {
        ResetButton();
    }

    private void OnEnable()
    {
        AudienceInput.OnAudienceInput.AddListener(OnAudienceInput);
    }

    private void OnDisable()
    {
        AudienceInput.OnAudienceInput.RemoveListener(OnAudienceInput);
    }

    private void Update()
    {
        float outputValueUpdate = ProcessDynamicValue(Time.deltaTime);
        if (outputValueUpdate != OutputValue)
        {
            OutputValue = outputValueUpdate;
            onValueChange.Invoke(outputValueUpdate, configuration.maxValue);
        }
        if (IsMaxed)
        {
            onValueMaxed.Invoke();
        }
    }

    public void ResetButton()
    {
        InputValue = 0f;
        OutputValue = 0f;
        StaticOutputValue = 0f;
        onValueChange.Invoke(0f, configuration.maxValue);
    }

    private void OnAudienceInput()
    {
        InputValue = GetInputValue();
        StaticOutputValue = ProcessInputValue();
        if (AudienceInput.ButtonDown(buttonID)) autoPressTimer = 0f;
    }

    private float GetInputValue()
    {
        switch (configuration.inputType)
        {
            case ValueType.Delta:       return AudienceInput.GetButton(buttonID, new(ButtonValueType.ValueType.Delta, configuration.smoothInput));
            case ValueType.Counter:     return AudienceInput.GetButton(buttonID, new(ButtonValueType.ValueType.Delta, configuration.smoothInput));
            case ValueType.Velocity:    return AudienceInput.GetButton(buttonID, new(ButtonValueType.ValueType.Velocity, configuration.smoothInput));
            case ValueType.Total:       return AudienceInput.GetButton(buttonID, new(ButtonValueType.ValueType.Total, configuration.smoothInput));
            default:                    return 0f;
        }
    }

    private float ProcessInputValue()
    {
        float processed;
        if (configuration.inputType == ValueType.Counter)
            processed = OutputValue + InputValue;
        else
            processed = InputValue;
        if (configuration.clampZeroMax)
            processed = Mathf.Clamp(processed, 0f, configuration.maxValue);
        return processed;
    }

    private float ProcessDynamicValue(float deltaTime)
    {
        float processed = OutputValue;
        if (configuration.enableAutoPress && configuration.autoPressSpeed != 0f)
        {
            if (autoPressTimer < configuration.autoPressTriggerDelay) autoPressTimer += deltaTime;
            if (autoPressTimer >= configuration.autoPressTriggerDelay) processed += deltaTime * configuration.autoPressSpeed;
        }
        return processed;
    }

    public void PressButton(float presses = 1f)
    {
        InputValue = presses;
        StaticOutputValue = ProcessInputValue();
        //autoPressTimer = 0f;
    }

    public bool IsMaxed => OutputValue >= configuration.maxValue;
}