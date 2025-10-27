using TMPro;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class NPCDialogChoiceButton : MonoBehaviour
{
    public AudienceButtonListener button;
    public TMP_Text label;
    public string labelText = "OK";
    public int choiceIndex = -1;

    public UnityEvent<int> onValidateChoice;

    private void Reset()
    {
        button = GetComponentInChildren<AudienceButtonListener>(true);
        label = GetComponentInChildren<TMP_Text>(true);
    }

    private void OnEnable()
    {
        if (button)
        {
            button.ResetButton();
            button.onValueMaxed.AddListener(OnButtonValueMaxed);
        }
    }

    private void OnDisable()
    {
        if (button)
        {
            button.ResetButton();
            button.onValueMaxed.RemoveListener(OnButtonValueMaxed);
        }
    }

    private void OnButtonValueMaxed()
    {
        onValidateChoice.Invoke(choiceIndex);
        ResetButton();
    }

    void Update()
    {
        if (label) label.text = labelText;
    }

    public void ResetButton() => button?.ResetButton();
}
