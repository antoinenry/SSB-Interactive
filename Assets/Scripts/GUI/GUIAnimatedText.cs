using UnityEngine;
using System.Collections;
using TMPro;

[ExecuteAlways]
public class GUIAnimatedText : MonoBehaviour
{
    public TMP_Text textField;
    public string text = "Label Text";
    public bool visible = true;
    public bool animated = true;
    public float inAnimationSpeed = 10f;
    public float outAnimationSpeed = 20f;

    private Coroutine animationCoroutine;

    public string AnimatedText { get; private set; }

    private void Reset()
    {
        textField = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (!Application.isPlaying || animated == false) StaticUpdate();
        else AnimatedUpdate();
    }

    private void AnimatedUpdate()
    {
        if (animationCoroutine == null) animationCoroutine = StartCoroutine(AnimateLabelCoroutine());
        textField?.SetText(AnimatedText);
    }

    private void StaticUpdate()
    {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = null;
        textField?.SetText(text);
    }

    public IEnumerator AnimateLabelCoroutine()
    {
        while (animated)
        {
            if (textField == null) yield return null;
            else
            {
                if (AnimatedText == null) AnimatedText = "";
                if (text == null) text = "";
                else
                {
                    if (ShouldResetAnimatedText()) AnimatedText = "";
                    int animatedLength = AnimatedText.Length, textLength = text.Length;
                    if (visible && inAnimationSpeed > 0f)
                    {
                        if (animatedLength < textLength) AnimatedText += text[animatedLength];
                        yield return new WaitForSeconds(1f / inAnimationSpeed);
                    }
                    else if (!visible && outAnimationSpeed > 0f)
                    {
                        if (animatedLength > 0) AnimatedText = text.Remove(animatedLength - 1);
                        yield return new WaitForSeconds(1f / outAnimationSpeed);
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }
        }
        animationCoroutine = null;
    }

    private bool ShouldResetAnimatedText()
    {
        if (text == null || text.Length == 0) return AnimatedText != null && AnimatedText.Length > 0;
        if (text == AnimatedText) return false;
        if (AnimatedText.Length > text.Length) return true;
        for (int i = 0, iend = AnimatedText.Length; i < iend; i++) if (AnimatedText[i] != text[i]) return true;
        return false;
    }
}
