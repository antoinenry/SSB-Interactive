using System.Collections.Generic;
using UnityEngine;
using static ClientButtonTracker;

public class InputSource : MonoBehaviour
{
    public struct ButtonInputDelta
    {
        public int totalPresses;
        public int deltaPresses;
        public float deltaTime;
    }

    public struct Axis
    { 
        public enum Direction { Vertical,  Horizontal };
    };

    public ClientButtonTracker buttonCounter;

    private Dictionary<string,ButtonInputDelta> buttonDeltas;

    public ClientButtonTracker.MultipleButtonTimedCount CurrentFrame { get; private set; }

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

    private void OnButtonCountUpdate(MultipleButtonTimedCount count)
    {
        if (buttonCounter == null) return;
        MultipleButtonTimedCount previousFrame = CurrentFrame;
        CurrentFrame = count;
        float previousTime = previousFrame.time, currentTime = CurrentFrame.time;
        foreach (KeyValuePair<string, int> b in buttonCounter.Current.counts)
        {
            UpdateButtonCount(b.Key, previousFrame[b.Key], b.Value, previousTime, currentTime);
        }
    }

    private void UpdateButtonCount(string id, int previousCount, int currentCount, float previousTime, float currentTime)
    {
        if (buttonDeltas == null) buttonDeltas = new();
        ButtonInputDelta newInput = new ButtonInputDelta()
        {
            totalPresses = currentCount,
            deltaPresses = currentCount - previousCount,
            deltaTime = currentTime - previousTime
        };
        if (buttonDeltas.ContainsKey(id) == false) buttonDeltas.Add(id, newInput);
        else buttonDeltas[id] = newInput;
    }

    static public float Get(string id, ButtonValueType type)
    {
        return float.NaN;
    }

    static public float GetAxis(Axis.Direction direction, ButtonValueType type, bool directionOnly = false) => float.NaN;
}