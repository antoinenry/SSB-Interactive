using UnityEngine;
using UnityEngine.UI;

public class GlowingGUIImage : MonoBehaviour
{
    public float rate = 1f;
    public float alphaMax = 1f;
    public float alphaMin = 0f;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (image == null) return;
        float t = Time.time * rate * 2f * Mathf.PI;
        Color c = image.color;
        c.a = alphaMin + .5f * (1f + Mathf.Sin(t)) * (alphaMax - alphaMin);
        image.color = c;
    }
}
