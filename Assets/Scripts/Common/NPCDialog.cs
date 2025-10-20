using TMPro;
using UnityEngine;

public class NPCDialog : MonoBehaviour
{
    public TMP_Text textField;
    public string text;

    private void Update()
    {
        if (textField != null) textField.text = text;
    }
}
