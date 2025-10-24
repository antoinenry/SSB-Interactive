using UnityEngine;

public class NPCDialog : MonoBehaviour
{
    [Header("Components")]
    public GUIAnimatedText textField;
    public NPCDialogChoiceButton[] choiceButtons;
    [Header("Content")]
    public NPCDialogAsset dialog;
    public int currentLineIndex = 0;

    private void OnValidate()
    {
        SetDialog(dialog);
    }

    private void SetDialog(NPCDialogAsset setDialog)
    {
        dialog = setDialog;
        if (textField == null) return;
        NPCDialogContent.DynamicLine currentLine = dialog != null ? dialog.GetLine(currentLineIndex) : NPCDialogContent.DynamicLine.None;
        textField.text = currentLine.text;
        int buttonCount = choiceButtons != null ? choiceButtons.Length : 0;
        int answerCount = currentLine.AnswerCount;
        NPCDialogChoiceButton button;
        for (int i = 0; i < buttonCount; i++)
        {
            button = choiceButtons[i];
            if (button == null) continue;
            if (i > answerCount - 1) button.enabled = false;
            else
            {
                button.labelText = currentLine.answers[i];
                button.enabled = true;
            }
        }
    }
}
