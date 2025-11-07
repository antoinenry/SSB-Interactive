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
    public float smoothGaugeSpeed = 2f;

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
            float fillAmount = maxValue > 0f ? value / maxValue : 1f;
            if (Application.isPlaying)
                gaugeFill.fillAmount = Mathf.MoveTowards(gaugeFill.fillAmount, fillAmount, smoothGaugeSpeed * Time.deltaTime);
            else
                gaugeFill.fillAmount = fillAmount;
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
