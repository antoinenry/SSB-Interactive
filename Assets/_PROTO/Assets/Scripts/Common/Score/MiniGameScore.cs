using UnityEngine;
using TMPro;

[ExecuteAlways]
public class MiniGameScore : MonoBehaviour
{
    public TMP_Text scoreField;
    public float unitValue;
    public int precision;
    public string unit;
    public bool showUnitInFront;
    public int pointsPerUnit;

    public int PointsValue => RoundedUnitValue * pointsPerUnit;
    public int RoundedUnitValue
    {
        get
        {
            float rounder = Mathf.Pow(10f, precision);
            return (int)(Mathf.Round(unitValue * rounder) / rounder);
        }
    }
    public string ScoreString => showUnitInFront ? unit + RoundedUnitValue : RoundedUnitValue + unit;

    private void Update()
    {
        if (scoreField) scoreField.text = ScoreString;
    }
}
