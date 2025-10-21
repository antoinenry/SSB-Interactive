using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class GUIVoteGauge : MonoBehaviour
{
    public enum StatFormat { Percentage, Fraction, Unitary, None }

    [Header("Components")]
    public TMP_Text labelField;
    public Slider slider;
    public TMP_Text statField;
    [Header("Input")]
    public string label = "Choix";
    public int votes = 0;
    public int maxVotes = 20;
    public StatFormat statFormat;
    public bool clampVotes = true;

    private void Update()
    {
        UpdateVotes();
        UpdateLabel();
        UpdateSlider();
        UpdateStat();
    }

    private void UpdateVotes()
    {
        if (clampVotes) votes = Mathf.Clamp(votes, 0, maxVotes);
    }

    private void UpdateLabel()
    {
        if (labelField == null) return;
        labelField.text = label;
    }

    private void UpdateSlider()
    {
        if (slider == null) return;
        slider.maxValue = 1f;
        slider.value = (float)votes / maxVotes;
    }

    private void UpdateStat()
    {
        if (statField == null) return;
        switch(statFormat)
        {
            case StatFormat.Percentage:
                statField.text = (100f * votes / maxVotes).ToString("0") + " %";
                break;
            case StatFormat.Fraction:
                statField.text = votes + "/" + maxVotes;
                break;
            case StatFormat.Unitary:
                statField.text = ((float)votes / maxVotes).ToString("0.00");
                break;
            default:
                statField.text = "";
                break;
        }
    }
}
