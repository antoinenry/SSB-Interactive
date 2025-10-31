using UnityEngine;
using UnityEngine.Events;

public abstract class SelectorButton<T> : MonoBehaviour
{
    [SerializeField] AudienceButtonListener button;
    public T item;

    public UnityEvent<T,float,float> onItemValueChange;
    public UnityEvent<T> onItemValueMaxed;

    public AudienceButtonListener Button => button;

    private void OnEnable()
    {
        if (button == null) return;
        button.onValueMaxed.AddListener(OnButtonMax);
        button.onValueChange.AddListener(OnButtonPress);
    }

    private void OnDisable()
    {
        if (button == null) return;
        button.onValueMaxed.RemoveListener(OnButtonMax);
        button.onValueChange.RemoveListener(OnButtonPress);
    }

    private void OnButtonPress(float value, float maxValue)
    {
        onItemValueChange.Invoke(item, value, maxValue);
    }

    private void OnButtonMax()
    {
        onItemValueMaxed.Invoke(item);
    }
}