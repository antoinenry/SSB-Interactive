using UnityEngine;
using UnityEngine.Events;

public class AudienceButtonListener : MonoBehaviour
{
    public enum ValueType { Delta, Velocity, Total, Counter }

    [Header("Input source")]
    public string buttonID = "a";
    public ValueType inputType = ValueType.Delta;
    public bool smoothInput = false;
    [Header("Range")]
    public float maxValue = 10;
    public bool clampZeroMax = true;
    [Header("Dynamics")]
    public bool enableDamping = true;
    [Min(0f)] public float damping;
    public bool enableAutoPress = true;
    public float autoPress = -1f;
    [Min(0f)] public float autoPressTriggerDelay = 1f;
    [Header("Output")]
    public UnityEvent<float,float> onValueChange;
    public UnityEvent onValueMaxed;

    public float InputValue { get; private set; }
    public float OutputValue { get; private set; }
    public float StaticOutputValue { get; private set; }

    private float autoPressTimer;

    private void Awake()
    {
        Init();
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
        float outputValueUpdate = ProcessDynamicValue();
        if (outputValueUpdate != OutputValue)
        {
            OutputValue = outputValueUpdate;
            onValueChange.Invoke(outputValueUpdate, maxValue);
        }
    }

    public void Init()
    {
        InputValue = 0f;
        OutputValue = 0f;
        StaticOutputValue = 0f;
        onValueChange.Invoke(0f, maxValue);
    }

    private void OnAudienceInput()
    {
        InputValue = GetInputValue();
        StaticOutputValue = ProcessInputValue();
        if (AudienceInput.ButtonDown(buttonID)) autoPressTimer = 0f;
    }

    private float GetInputValue()
    {
        switch (inputType)
        {
            case ValueType.Delta:       return AudienceInput.GetButton(buttonID, new(ButtonValueType.ValueType.Delta, smoothInput));
            case ValueType.Counter:     return AudienceInput.GetButton(buttonID, new(ButtonValueType.ValueType.Delta, smoothInput));
            case ValueType.Velocity:    return AudienceInput.GetButton(buttonID, new(ButtonValueType.ValueType.Velocity, smoothInput));
            case ValueType.Total:       return AudienceInput.GetButton(buttonID, new(ButtonValueType.ValueType.Total, smoothInput));
            default:                    return 0f;
        }
    }

    private float ProcessInputValue()
    {
        float processed;
        if (inputType == ValueType.Counter)
            processed = OutputValue + InputValue;
        else
            processed = InputValue;
        if (clampZeroMax)
            processed = Mathf.Clamp(processed, 0f, maxValue);
        return processed;
    }

    private float ProcessDynamicValue()
    {
        float processed = StaticOutputValue;
        float deltaTime = Time.deltaTime;
        if (enableDamping && damping > 0f)
        {
            processed = Mathf.MoveTowards(OutputValue, processed, deltaTime * Mathf.Abs(OutputValue - processed) / damping);
        }
        if (enableAutoPress && autoPress != 0f)
        {
            if (autoPressTimer < autoPressTriggerDelay) autoPressTimer += deltaTime;
            if (autoPressTimer >= autoPressTriggerDelay) processed += deltaTime * autoPress;
        }
        return processed;
    }

    public void PressButton(int presses = 1)
    {
        InputValue += presses;
        StaticOutputValue = ProcessInputValue();
        autoPressTimer = 0f;
    }
}