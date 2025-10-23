using System;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class GUIAdaptiveTextBackground : MonoBehaviour
{
    [Serializable]
    public struct Margins
    {
        public float left, right, top, bottom;
    };

    [Header("Components")]
    public RectTransform adaptiveTransform;
    public TMP_Text textField;
    [Header("Aspect")]
    public Vector2 minimumSize;
    public Margins margins;

    private void Update()
    {
        if (textField == null) return;
        AdaptiveSize = new Vector2()
        {
            x = Mathf.Max(minimumSize.x, textField.renderedWidth + margins.left + margins.right),
            y = Mathf.Max(minimumSize.y, textField.renderedHeight + margins.top + margins.bottom)
        };
        if (adaptiveTransform == null) return;
        adaptiveTransform.sizeDelta = AdaptiveSize;
        adaptiveTransform.position = textField.rectTransform.position - .5f * new Vector3(margins.left - margins.right, margins.bottom - margins.top, 0f);
    }

    public Vector2 AdaptiveSize { get; private set; }
}
