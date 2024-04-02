using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class GUISliderGroup : MonoBehaviour
{
    public Slider[] sliders;
    public bool normalizeMaxValues = true;
    public float maxValuesFloor = 10f;

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
        float maxValue = Mathf.Max(Mathf.Max(values), maxValuesFloor);
        float minValue = Mathf.Max(minValues);
        foreach (Slider s in sliders)
        {
            s.minValue = minValue;
            if (maxValue > minValue) s.maxValue = maxValue;
        }
    }
}