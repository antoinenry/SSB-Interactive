using UnityEngine;
using UnityEngine.Events;

namespace NPC
{
    public class NPCDialog : MonoBehaviour
    {
        [Header("Components")]
        public Animation animatedGUI;
        public GUIAnimatedText animatedText;
        public AudienceButtonListener nextButton;
        public NPCDialogChoiceButton[] answerButtons;
        [Header("Content")]
        public NPCDialogContentAsset dialog;
        public NPCDialogInjector injector;
        [SerializeField] int lineIndex = 0;
        [SerializeField] bool isReacting = false;
        [SerializeField] int reactionIndex = -1;
        [Header("Sequence")]
        public bool messageDialogToAdmin = true;
        public string messageOnDialogEnd = "(fin du dialogue)";
        public AnimationClip showAnimation;
        public AnimationClip hideAnimation;
        public UnityEvent onDialogEnd;

        public bool IsShowingDialog { get; private set; }

        private void Reset()
        {
            animatedGUI = GetComponentInChildren<Animation>(true);
            animatedText = GetComponentInChildren<GUIAnimatedText>(true);
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
            IsShowingDialog = false;
            SetButtonListenersActive(false);
            ClearText();
            HideAllButtons();
        }

        private void Start()
        {
            ClearText();
            HideAllButtons();
            ShowDialogLine();
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
            if ((animatedText != null && animatedText.IsAnimating)
                || (dialog == null || lineIndex >= dialog.LineCount))
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
            text = InjectText(text);
            if (messageDialogToAdmin && text != "" && Application.isPlaying)
                MessengerAdmin.Send(text);
            if (animatedText == null) return;
            animatedText.text = text;
        }

        public string InjectText(string text)
        {
            if (injector != null)
            {
                injector.UpdateDictionary();
                return injector.Inject(text);
            }
            return text;
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
                    button.labelText = InjectText(answerTexts[i]);
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

        public void ShowDialogLine(NPCDialogContentAsset setDialog, int setLineIndex)
        {
            dialog = setDialog;
            lineIndex = setLineIndex;
            ShowDialogLine();
        }

        public void ShowDialogLine(int setLineIndex) => ShowDialogLine(dialog, setLineIndex);

        public void ShowDialogLine()
        {
            isReacting = false;
            if (lineIndex < 0)
            {
                HideDialog();
            }
            NPCDialogContent.DynamicLine currentLine = dialog != null ? dialog.GetLine(lineIndex) : NPCDialogContent.DynamicLine.None;
            if (dialog == null || lineIndex >= dialog.LineCount)
            {
                EndDialog();
            }
            else
            {
                ShowDialog();
                ShowText(currentLine.text);
                SetNextButtonActive(currentLine.AnswerCount == 0);
                ShowAnswerButtons(currentLine.answers);
            }
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
            IsShowingDialog = true;
            isReacting = true;
            HideAllButtons();
            ShowText(dialog.GetReaction(lineIndex, reactionIndex));
        }

        public void ShowDialog()
        {
            if (IsShowingDialog == true) return;
            if (animatedGUI)
            {
                animatedGUI.clip = showAnimation;
                animatedGUI.Play();
            }
            IsShowingDialog = true;
        }

        public void HideDialog()
        {
            ClearText();
            HideAllButtons();
            if (IsShowingDialog == false) return;
            if (animatedGUI)
            {
                animatedGUI.clip = hideAnimation;
                animatedGUI.Play();
            }
            IsShowingDialog = false;
        }

        public void EndDialog()
        {
            if (messageDialogToAdmin && messageOnDialogEnd != "" && Application.isPlaying)
                MessengerAdmin.Send(messageOnDialogEnd);
            HideDialog();
            lineIndex = dialog != null ? dialog.LineCount : -1;
            isReacting = false;
            reactionIndex = -1;
            onDialogEnd.Invoke();
        }
    }
}
