using UnityEngine;
using UnityEngine.Events;

public abstract class SelectorButton<T> : MonoBehaviour
{
    [SerializeField] AudienceButtonListener button;
    [SerializeField] T item;

    public UnityEvent<T,float,float> onItemValueChange;
    public UnityEvent<T> onItemValueMaxed;

    public AudienceButtonListener Button => button;

    public T Item
    {
        get => item;
        set
        {
            onItemValueChange.Invoke(value, ButtonValue, ButtonValueMax);
            item = value;
        }
    }

    protected virtual void Reset()
    {
        button = GetComponentInChildren<AudienceButtonListener>(true);
    }

    protected virtual void OnEnable()
    {
        if (button == null) return;
        button.onValueMaxed.AddListener(OnButtonMax);
        button.onValueChange.AddListener(OnButtonPress);
    }

    protected virtual void OnDisable()
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

    public float ButtonValue => button != null ? button.OutputValue : 0f;

    public float ButtonValueMax => button != null ? button.MaxValue : 0f;
}