using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SliderCounter : MonoBehaviour
{
    public int value;
    public int maxValue;

    private Slider slider;
    private TMP_Text counter;


    private void Awake()
    {
        slider = GetComponentInChildren<Slider>(true);
        counter = GetComponentInChildren<TMP_Text>(true);
    }

    private void Update()
    {
        slider.value = value;
        counter.text = value != 0 ? value.ToString() : "";
        slider.maxValue = maxValue;
    }
}
