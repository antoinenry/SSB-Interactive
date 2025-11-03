using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class GUIAudienceButton : MonoBehaviour
{
    [Header("Components")]
    public AudienceButtonListener button;
    public Image gaugeFill;
    [Header("Input")]
    public float value = 5;
    public float maxValue = 5;
    [Header("Aspect")]
    public bool showGauge = true;

    private void Reset()
    {
        button = GetComponentInChildren<AudienceButtonListener>(true);
        gaugeFill = GetComponentInChildren<Image>(true);
    }

    private void OnEnable()
    {
        if (button)
        {
            value = button.OutputValue;
            maxValue = button.MaxValue;
            button.onValueChange.AddListener(SetValues);
        }
    }

    private void OnDisable()
    {
        if (button) button.onValueChange.RemoveListener(SetValues);
    }

    private void Update()
    {
        UpdateGauge();
    }

    private void UpdateGauge()
    {
        if (gaugeFill == null) return;
        if (showGauge)
        {
            gaugeFill.gameObject.SetActive(true);
            if (maxValue <= 0) gaugeFill.fillAmount = 1;
            else gaugeFill.fillAmount = value / maxValue;
        }
        else
        {
            gaugeFill.gameObject.SetActive(false);
        }
    }

    public void SetValues(float value = float.NaN, float maxValue = float.NaN)
    {
        if (!float.IsNaN(value)) this.value = value;
        if (!float.IsNaN(maxValue)) this.maxValue = maxValue;
    }
}
