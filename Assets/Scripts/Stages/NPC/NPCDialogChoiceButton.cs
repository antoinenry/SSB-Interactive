using TMPro;
using UnityEngine;

[ExecuteAlways]
public class NPCDialogChoiceButton : MonoBehaviour
{
    public AudienceButtonListener button;
    public TMP_Text label;
    public string labelText = "OK";

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
            button.gameObject.SetActive(true);
        }
        if (label) label.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (button)
        {
            button.gameObject.SetActive(false);
            button.ResetButton();
        }
        if (label) label.gameObject.SetActive(false);
    }

    void Update()
    {
        if (label) label.text = labelText;
    }
}
