using UnityEngine;
using UnityEngine.UI;

public class GUIButtonCountSlider : MonoBehaviour
{
    public string buttonID;
    public ButtonValueType valueType;
    public bool adaptMaxValue = true;
    public float accelerationScaleEffect = 1.5f;
    public float accelerationScaleEffectCooldown = .5f;

    private Slider slider;

    public float MaxValue
    {
        get => slider != null ? slider.maxValue : 0f;
        set { if (slider) slider.maxValue = value; }
    }

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        if (slider == null) return;
        float buttonValue = InputSource.Get(buttonID, valueType);
        if (adaptMaxValue) slider.maxValue = Mathf.Max(buttonValue, slider.maxValue);
        slider.value = buttonValue;
        float acceleration = InputSource.Get(buttonID, ButtonValueType.Acceleration);
        float scale = slider.transform.localScale.x;
        if (acceleration > 0f) scale = accelerationScaleEffect;
        else scale = Mathf.MoveTowards(scale, 1f, Time.deltaTime / accelerationScaleEffectCooldown);
        slider.transform.localScale = scale * Vector3.one;
    }
}
