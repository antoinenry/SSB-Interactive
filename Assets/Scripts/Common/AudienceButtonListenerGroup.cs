using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;

public class AudienceButtonListenerGroup : MonoBehaviour
{
    [Header("Buttons")]
    public AudienceButtonListener[] buttons;
    public bool applyButtonConfiguration = true;
    public AudienceButtonListener.ButtonConfiguration buttonConfiguration;
    [Header("Animations")]
    public Animation[] buttonAnimations;
    public AnimationClip idleAnimation;
    public AnimationClip selectedAnimation;
    public AnimationClip winnerAnimation;
    public AnimationClip loserAnimation;
    [Header("Auto press")]
    public AutoPressMode autoPress = AutoPressMode.Leader;
    public float autoPressDelay = 3f;
    public float autoPressBaseSpeed = 1f;
    public float autoPressAcceleration = 1f;
    [Header("Events")]
    public UnityEvent onRankingChange;
    public UnityEvent onButtonMaxed;

    private float autoPressTimer;

    public enum AutoPressMode { Off, Random, Leader }

    public AudienceButtonListener[] RankedButtons { get; private set; }

    private void Reset()
    {
        buttons = GetComponentsInChildren<AudienceButtonListener>(true);
        buttonAnimations = GetComponentsInChildren<Animation>(true);
        ApplyButtonConfiguration();
    }

    private void OnEnable()
    {
        ResetButtons();
        SetListenersActive(true);
    }

    private void OnDisable()
    {
        SetListenersActive(false);
        ResetButtons();
    }

    private void OnValidate()
    {
        ApplyButtonConfiguration();
    }

    private void Update()
    {
        AnimationUpdate();
        if (autoPress != AutoPressMode.Off) AutoPressUpdate(Time.deltaTime);
    }

    private void SetListenersActive(bool active)
    {
        if (ButtonCount == 0) return;
        foreach (AudienceButtonListener b in buttons)
        {
            if (b == null) continue;
            if (active)
            {
                b.onValueChange.AddListener(OnButtonValueChange);
                b.onValueMaxed.AddListener(OnButtonValueMaxed);
            }
            else
            {
                b.onValueChange.RemoveListener(OnButtonValueChange);
                b.onValueMaxed.RemoveListener(OnButtonValueMaxed);
            }
        }
    }

    private void OnButtonValueChange(float value, float maxValue)
    {
        UpdateButtonRanking();
    }

    private void OnButtonValueMaxed()
    {
        onButtonMaxed.Invoke();
    }

    private void ApplyButtonConfiguration()
    {
        if (ButtonCount == 0 || applyButtonConfiguration == false) return;
        foreach (AudienceButtonListener b in buttons)
        {
            if (b == null) continue;
            b.configuration = buttonConfiguration;
        }
    }

    public int ButtonCount => buttons != null ? buttons.Length : 0;

    public AudienceButtonListener GetLeaderButton()
    {
        if (RankedButtons == null) UpdateButtonRanking();
        return RankedButtons.Length > 0 ? RankedButtons[0] : null;
    }

    public int GetLeaderIndex()
    {
        if (buttons == null) return -1;
        AudienceButtonListener leader = GetLeaderButton();
        return Array.IndexOf(buttons, leader);
    }

    public AudienceButtonListener GetRandomButton()
    {
        if (ButtonCount == 0) return null;
        else return buttons[UnityEngine.Random.Range(0, ButtonCount)];
    }

    public void UpdateButtonRanking()
    {
        if (ButtonCount == 0) return;
        if (RankedButtons == null || RankedButtons.Length != buttons.Length)
        {
            RankedButtons = new AudienceButtonListener[buttons.Length];
            Array.Copy(buttons, RankedButtons, buttons.Length);
        }
        bool mustUpdateRanking = false;
        for (int i = 0, iend = ButtonCount - 1; i < iend; i++)
        {
            if (RankedButtons[i] == null || RankedButtons[i + 1] == null) continue;
            if (RankedButtons[i].OutputValue < RankedButtons[i + 1].OutputValue)
            {
                mustUpdateRanking = true;
                break;
            }
        }
        if (mustUpdateRanking)
        {
            Array.Sort(RankedButtons, new ButtonValueComparer());
            onRankingChange.Invoke();
        }
    }

    public class ButtonValueComparer : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            float x_value = (x != null && x is AudienceButtonListener) ? (x as AudienceButtonListener).OutputValue : int.MinValue;
            float y_value = (y != null && y is AudienceButtonListener) ? (y as AudienceButtonListener).OutputValue : int.MinValue;
            if (x_value == y_value) return 0;
            else if (x_value < y_value) return 1;
            else return -1;
        }
    }

    private void AutoPressUpdate(float deltaTime)
    {
        AudienceButtonListener targetButton;
        autoPressTimer += deltaTime;
        if (autoPressTimer < autoPressDelay)
        {
            targetButton = null;
        }
        else switch (autoPress)
            {
                case AutoPressMode.Random:
                    targetButton = GetRandomButton();
                    break;
                case AutoPressMode.Leader:
                    targetButton = GetLeaderButton();
                break;
            default:
                targetButton = null;
                break;
        }
        if (targetButton == null) return;
        float autoPressSpeed = autoPressBaseSpeed + (autoPressTimer - autoPressDelay) * autoPressAcceleration;
        targetButton.PressButton(autoPressSpeed * deltaTime);
    }

    private void AnimationUpdate()
    {
        if (buttonAnimations == null) return;
        int animationCount = buttonAnimations != null ? buttonAnimations.Length : 0;
        int leaderIndex = GetLeaderIndex();
        bool leaderWins = HasWinner();
        AnimationClip leaderAnimation = leaderWins ? winnerAnimation : selectedAnimation;
        AnimationClip otherAnimation = leaderWins ? loserAnimation : idleAnimation;

        for (int i = 0; i < animationCount; i++)
        {
            if (buttonAnimations[i] == null) continue;
            buttonAnimations[i].clip = i == leaderIndex ? leaderAnimation : otherAnimation;
            buttonAnimations[i].Play();
                
        }
    }

    public bool HasWinner()
    {
        AudienceButtonListener leader = GetLeaderButton();
        if (leader == null) return false;
        return leader.IsMaxed;
    }
    public void ResetButtons()
    {
        autoPressTimer = 0f;
        if (buttons == null) return;
        foreach (AudienceButtonListener b in buttons) if (b) b.ResetButton();
    }

    public void SetButtonsEnabled(bool setEnabled)
    {
        if (buttons == null) return;
        foreach (AudienceButtonListener b in buttons) if (b) b.enabled = setEnabled;
    }
}
