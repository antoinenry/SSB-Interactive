using UnityEngine;
using UnityEngine.UI;

public class GUIButtonCountSlider : MonoBehaviour
{
    public enum ButtonCountValue { Total, Delta, RateRaw, RateSmooth }

    public string buttonID;
    public ButtonCountValue value;
    public bool adaptMaxValue = true;

    private Slider slider;
    private InputCounter inputSystem;

    public float MaxValue
    {
        get => slider != null ? slider.maxValue : 0f;
        set { if (slider) slider.maxValue = value; }
    }

    private void Awake()
    {
        slider = GetComponent<Slider>();
        inputSystem = FindObjectOfType<InputCounter>(true);
    }

    private void Update()
    {
        if (inputSystem == null || slider == null) return;
        float value = GetButtonValue();
        if (adaptMaxValue) slider.maxValue = Mathf.Max(value, slider.maxValue);
        slider.value = value;
    }

    private float GetButtonValue()
    {
        switch (value)
        {
            case ButtonCountValue.Total: return inputSystem.GetButtonData(buttonID).maxCount;
            case ButtonCountValue.Delta: return inputSystem.GetButtonData(buttonID).DeltaCount;
            case ButtonCountValue.RateRaw: return inputSystem.GetButtonRateRaw(buttonID);
            case ButtonCountValue.RateSmooth: return inputSystem.GetButtonRateSmooth(buttonID);
            default: return 0;
        }
    }
}
