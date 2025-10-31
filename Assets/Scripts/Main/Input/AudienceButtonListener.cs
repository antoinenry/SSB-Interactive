using UnityEngine;
using UnityEngine.Events;
using System;

public class AudienceButtonListener : MonoBehaviour
{
    public string buttonID = "a";
    public Configuration configuration;
    public UnityEvent<float,float> onValueChange;
    public UnityEvent onValueMaxed;

    public enum ValueType { Total, Counter, Velocity }

    [Serializable]
    public struct Configuration
    {
        [Header("Input")]
        public ValueType inputType;
        [Header("Range")]
        public float maxValue;
        public bool clampZeroMax;
        [Header("Autopress")]
        public bool enableAutoPress;
        public float autoPressSpeed;
        [Min(0f)] public float autoPressDelay;

        public static Configuration Default = new Configuration()
        {
            inputType = ValueType.Counter,
            maxValue = 10f,
            clampZeroMax = true,
            enableAutoPress = false,
            autoPressSpeed = 1f,
            autoPressDelay = 0f
        };
    }

    public float AudienceInputValue {  get; private set; }
    public float AudienceInputTime { get; private set; }
    public float ExternalInputValue { get; private set; }
    public float AutopressValue { get; private set; }
    public float OutputValue { get; private set; }

    private float autoPressTimer;

    private void Awake()
    {
        ResetButton();
    }

    private void OnEnable()
    {
        AudienceInputSource.Current.onAudienceInput.AddListener(OnAudienceInput);
    }

    private void OnDisable()
    {
        AudienceInputSource.Current.onAudienceInput.RemoveListener(OnAudienceInput);
    }

    private void Update()
    {
        UpdateAutopress(Time.deltaTime);
        UpdateOutputValue();
    }

    private void OnAudienceInput()
    {
        switch (configuration.inputType)
        {
            case ValueType.Counter:
                AudienceInputValue = AudienceInputSource.Current.GetButton(buttonID).deltaPresses; break;
            case ValueType.Total:
                AudienceInputValue = AudienceInputSource.Current.GetButton(buttonID).totalPresses; break;
            case ValueType.Velocity:
                throw new Exception("Button velocity not implemented");
            default:
                AudienceInputValue = 0f; break;
        }
    }

    private float GetCurrentFrameAudienceInput()
    {
        float audienceInputTimeUpdate = AudienceInputSource.Current.GetLastInputTime();
        if (audienceInputTimeUpdate != AudienceInputTime)
        {
            AudienceInputTime = audienceInputTimeUpdate;
            return AudienceInputValue;
        }
        return 0f;
    }

    private void UpdateAutopress(float deltaTime)
    {
        // Simulate button press
        float simulatedInputValue = 0f;
        if (configuration.enableAutoPress && configuration.autoPressSpeed != 0f)
        {
            if (autoPressTimer < configuration.autoPressDelay) autoPressTimer += deltaTime;
            else if (autoPressTimer >= configuration.autoPressDelay) simulatedInputValue = deltaTime * configuration.autoPressSpeed;
        }
        // Different autopress behaviour depending on input type
        switch (configuration.inputType)
        {
            case ValueType.Total:
                AutopressValue += simulatedInputValue;
                break;
            case ValueType.Counter:
                AutopressValue = simulatedInputValue;
                break;
            case ValueType.Velocity:
                AutopressValue = configuration.enableAutoPress ? configuration.autoPressSpeed : 0f;
                break;
        }
    }

    public void PressButton(float presses = 1f)
    {
        // Different press behaviour depending on input type
        switch (configuration.inputType)
        {
            case ValueType.Total:
            case ValueType.Counter:
                ExternalInputValue += presses;
                break;
            case ValueType.Velocity:

                break;
        }
    }

    private void UpdateOutputValue()
    {
        float outputValueUpdate = 0f;
        // Different output behaviour depending on input type
        switch (configuration.inputType)
        {
            case ValueType.Total:
                outputValueUpdate = AudienceInputValue;
                break;
            case ValueType.Counter:
                outputValueUpdate = OutputValue + GetCurrentFrameAudienceInput();
                break;
            case ValueType.Velocity:

                break;
        }
        // Additional values
        outputValueUpdate += AutopressValue + ExternalInputValue;
        ExternalInputValue = 0f;
        // Update output and notify if value has changed
        if (outputValueUpdate != OutputValue)
        {
            onValueChange.Invoke(outputValueUpdate, configuration.maxValue);
            OutputValue = outputValueUpdate;
        }
        // Notify if value reaches max
        if (IsMaxed) onValueMaxed.Invoke();
    }

    public void ResetButton()
    {
        AudienceInputValue = 0f;
        AudienceInputTime = 0f;
        AutopressValue = 0f;
        ExternalInputValue = 0f;
        OutputValue = 0f;
        autoPressTimer = 0f;
        onValueChange.Invoke(0f, configuration.maxValue);
    }

    public float MaxValue
    {
        get => configuration.maxValue;
        set => configuration.maxValue = value;
    }

    public bool IsMaxed => OutputValue >= MaxValue;
}