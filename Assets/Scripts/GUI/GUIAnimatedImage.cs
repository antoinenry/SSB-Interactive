using UnityEngine;
using UnityEngine.UI;

public class GUIAnimatedImage : MonoBehaviour
{
    public Image image;
    public bool hide = false;
    public float blinkFrequency = 0f;

    private void Update()
    {
        if (image == null) return;
        if (hide == true)
        {
            image.enabled = false;
            return;
        }
        else image.enabled = true;
        float time = Time.time;
        BlinkAnimationUpdate(time);
    }

    private void BlinkAnimationUpdate(float t)
    {
        if (blinkFrequency <= 0f) return;
        image.enabled = (int)(t * blinkFrequency) % 2 == 0;
    }
}
