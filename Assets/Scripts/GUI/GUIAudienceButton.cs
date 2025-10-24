using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class GUIAudienceButton : MonoBehaviour
{
    [Header("Components")]
    public Image gaugeFill;
    [Header("Input")]
    public float value = 5;
    public float maxValue = 5;
    [Header("Aspect")]
    public bool showGauge = true;

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
