using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class GUISliderGroup : MonoBehaviour
{
    public Slider[] sliders;
    public bool normalizeMaxValues = true;

    private void Reset()
    {
        sliders = GetComponentsInChildren<Slider>(true);
    }

    private void Update()
    {
        if (sliders == null) return;
        if (normalizeMaxValues) NormalizeMaxValues();
    }

    private void NormalizeMaxValues()
    {
        float[] values = Array.ConvertAll(sliders, s => s.value);
        float[] minValues = Array.ConvertAll(sliders, s => s.minValue);
        float maxValue = Mathf.Max(values);
        float minValue = Mathf.Max(minValues);
        foreach (Slider s in sliders)
        {
            s.minValue = minValue;
            if (maxValue > minValue) s.maxValue = maxValue;
        }
    }
}