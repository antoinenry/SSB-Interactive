using UnityEngine;

public class NPCDialog : MonoBehaviour
{
    [Header("Components")]
    public GUIAnimatedText textField;
    public AudienceButtonListener nextButton;
    public NPCDialogChoiceButton[] answerButtons;
    [Header("Content")]
    [SerializeField] NPCDialogAsset dialog;
    [SerializeField] int lineIndex = 0;
    [SerializeField] bool isReacting = false;
    [SerializeField] int reactionIndex = -1;

    private void Reset()
    {
        textField = GetComponentInChildren<GUIAnimatedText>(true);
        answerButtons = GetComponentsInChildren<NPCDialogChoiceButton>(true);
    }

    private void OnValidate()
    {
        if (isReacting) ShowReactionLine();
        else ShowDialogLine();
    }

    private void OnEnable()
    {
        SetButtonListenersActive(true);
    }

    private void OnDisable()
    {
        SetButtonListenersActive(false);
        ClearText();
        HideAllButtons();
    }

    private void Update()
    {
        ButtonVisibilityUpdate();
    }

    private void SetButtonListenersActive(bool active)
    {
        if (nextButton != null)
        {
            if (active) nextButton.onValueMaxed.AddListener(OnNextButtonMaxed);
            else nextButton.onValueMaxed.RemoveListener(OnNextButtonMaxed);
        }
        if (answerButtons != null)
        {
            foreach (NPCDialogChoiceButton button in answerButtons)
            {
                if (button == null) continue;
                if (active) button.onValidateChoice.AddListener(OnChoseAnswer);
                else button.onValidateChoice.RemoveListener(OnChoseAnswer);
            }
        }
    }

    private void ButtonVisibilityUpdate()
    {
        if (   (textField != null && textField.IsAnimating)
            || (dialog == null || lineIndex >= dialog.LineCount)    )
        {
            HideAllButtons();
        }
        else if (isReacting || dialog.GetLine(lineIndex).AnswerCount == 0)
        {
            ShowAnswerButtons(null);
            SetNextButtonActive(true);
        }
        else
        {
            ShowAnswerButtons(dialog.GetLine(lineIndex).answers);
            SetNextButtonActive(false);
        }
    }

    private void OnNextButtonMaxed()
    {
        ResetButtons();
        ShowDialogLine(++lineIndex);
    }

    private void OnChoseAnswer(int answerIndex)
    {
        ResetButtons();
        if (dialog != null && dialog.HasReaction(lineIndex, answerIndex))
        {
            ShowReactionLine(lineIndex, answerIndex);
            SetNextButtonActive(true);
        }
        else
        {
            ShowDialogLine(++lineIndex);
        }
    }

    public void ShowText(string text)
    {
        if (textField == null) return;
        textField.text = text;
    }

    public void ClearText() => ShowText("");

    public void SetNextButtonActive(bool active)
    {
        if (nextButton == null) return;
        nextButton.gameObject.SetActive(active);
    }

    public void ShowAnswerButtons(string[] answerTexts)
    {
        int buttonCount = answerButtons != null ? answerButtons.Length : 0;
        int answerCount = answerTexts != null ? answerTexts.Length : 0;
        NPCDialogChoiceButton button;
        for (int i = 0; i < buttonCount; i++)
        {
            button = answerButtons[i];
            if (button == null) continue;
            if (i > answerCount - 1) button.gameObject.SetActive(false);
            else
            {
                button.choiceIndex = i;
                button.labelText = answerTexts[i];
                button.gameObject.SetActive(true);
            }
        }
    }

    public void HideAllButtons()
    {
        SetNextButtonActive(false);
        ShowAnswerButtons(null);
    }

    public void ResetButtons()
    {
        if (nextButton) nextButton.ResetButton();
        if (answerButtons != null) foreach (NPCDialogChoiceButton button in answerButtons) button?.ResetButton();
    }

    private void ShowDialogLine(NPCDialogAsset setDialog, int setLineIndex)
    {
        dialog = setDialog;
        lineIndex = setLineIndex;
        ShowDialogLine();
    }

    private void ShowDialogLine(int setLineIndex) => ShowDialogLine(dialog, setLineIndex);

    private void ShowDialogLine()
    {
        isReacting = false;
        NPCDialogContent.DynamicLine currentLine = dialog != null ? dialog.GetLine(lineIndex) : NPCDialogContent.DynamicLine.None;
        if (dialog == null || lineIndex >= dialog.LineCount)
        {
            ClearText();
            HideAllButtons();
            return;
        }
        ShowText(currentLine.text);
        SetNextButtonActive(currentLine.AnswerCount == 0);
        ShowAnswerButtons(currentLine.answers);
    } 

    private void ShowReactionLine(int setLineIndex, int setReactionIndex)
    {
        lineIndex = setLineIndex;
        reactionIndex = setReactionIndex;
        ShowReactionLine();
    }

    private void ShowReactionLine(int setReactionIndex) => ShowReactionLine(setReactionIndex);

    private void ShowReactionLine()
    {
        isReacting = true;
        HideAllButtons();
        ShowText(dialog.GetReaction(lineIndex, reactionIndex));
    }
}
