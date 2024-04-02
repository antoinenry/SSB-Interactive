using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputSource : MonoBehaviour
{
    private struct Button
    {
        static public int valueTypes => Enum.GetNames(typeof(ButtonValueType)).Length;
        static public Button New() => new() { valuesByType = new float[valueTypes] };

        private float[] valuesByType;

        public float Get(ButtonValueType type) => valuesByType[(int)type];
        public float Set(ButtonValueType type, float value) => valuesByType[(int)type] = value;
    }

    [Serializable]
    public struct Axis
    {
        public enum Direction { Horizontal, Vertical }
        public string positiveButton;
        public string negativeButton;
    }

    public ClientInputCounter clientInput;
    public string[] buttonIDs;
    public Axis horizontalAxis;
    public Axis verticalAxis;
    public float smoothRateUp = 0f;
    public float smoothRateDown = .5f;

    static private int instanceCount;
    static private InputSource instance;
    private Dictionary<string, Button> buttons;
    private SingleButtonCountOverTime[] buttonCounts;

    private void Awake()
    {
        clientInput?.InitClient();
        buttons = new();
        instance = this;
        instanceCount++;
        if (instanceCount > 1) Debug.LogWarning("Multiple instances");
    }

    private void Update()
    {
        if (clientInput == null) return;
        clientInput.UpdateCount();
        UpdateButtonIDs();
        buttonCounts = clientInput.GetCurrentButtonCounts(buttonIDs);
        UpdateValues();
    }

    private void UpdateButtonIDs()
    {
        List<string> keys = new(buttons.Keys);
        foreach (string key in keys) if (buttonIDs.Contains(key) == false) buttons.Remove(key);
        foreach (string id in buttonIDs) buttons.TryAdd(id, Button.New());
    }

    private void UpdateValues()
    {
        float deltaTime = Time.deltaTime;
        foreach (SingleButtonCountOverTime b in buttonCounts)
        {
            string id = b.buttonID;
            if (buttons.ContainsKey(id) == false) continue;
            int total = b.maxCount;
            float rawRate = b.Rate;
            float previousRawRate = buttons[id].Get(ButtonValueType.RateRaw);
            float previousSmoothRate = buttons[id].Get(ButtonValueType.RateSmooth);
            float smoothRate = SmoothValue(previousSmoothRate, rawRate, deltaTime);            
            float acc = (rawRate - previousRawRate) / deltaTime;
            buttons[id].Set(ButtonValueType.Total, total);
            buttons[id].Set(ButtonValueType.Acceleration, acc);
            buttons[id].Set(ButtonValueType.RateRaw, rawRate);
            buttons[id].Set(ButtonValueType.RateSmooth, smoothRate);
        }
    }

    private float SmoothValue(float current, float target, float deltaTime)
    {
        float smooth;
        if (target > current && smoothRateUp > 0f) smooth = Mathf.Lerp(current, target, deltaTime / smoothRateUp);
        else if (target < current && smoothRateDown > 0f) smooth = Mathf.Lerp(current, target, deltaTime / smoothRateDown);
        else smooth = target;
        return smooth;
    }

    static public float Get(string buttonID, ButtonValueType type)
        => (instance?.buttons != null && instance.buttons.ContainsKey(buttonID)) ? instance.buttons[buttonID].Get(type) : 0f;

    static public float GetAxis(Axis.Direction axis, ButtonValueType type, bool directionOnly = false, bool avoidInversion = true)
    {
        if (instance == null) return 0f;
        Axis a;
        switch (axis)
        {
            case Axis.Direction.Horizontal: a = instance.horizontalAxis; break;
            case Axis.Direction.Vertical: a = instance.verticalAxis; break;
            default: return 0f;
        }
        float positive = Get(a.positiveButton, type);
        float negative = Get(a.negativeButton, type);
        if (avoidInversion)
        {
            positive = Mathf.Max(positive, 0f);
            negative = Mathf.Max(negative, 0f);
        }
        if (directionOnly)
        {
            if (positive == negative) return 0f;
            else return Mathf.Sign(positive - negative);
        }
        else
            return positive - negative;
    }

    static public string GetLog()
    {
        string logText = "";
        SingleButtonCountOverTime[] window = instance?.clientInput.GetCurrentButtonCounts();
        if (window != null)
        {
            logText += "\nTime window: " + instance.clientInput.timeWindow + "s";
            logText += "\nRequest duration: " + instance.clientInput.RequestDuration + "s (every " + instance.clientInput.TimeBetweenRequests + "s)";
            logText += "\nButton counts:";
            foreach (SingleButtonCountOverTime b in window)
                logText += "\n- " + b.buttonID + ": " + b.maxCount + "; " + b.Rate.ToString("0.0") + "/s; ";
        }
        else
            logText = "NULL";
        if (instanceCount > 1) logText += "\nWarning: multiple input sources detected.";
        return logText;
    }
}