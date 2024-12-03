using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    public Sprite[] sprites;
    public float animationFrameDuration = .1f;
    public float floatAnimationSpeed = 10f;
    public float floatAnimationAmplitude = .25f;
    public float catchAnimationDuration = 1f;
    public float catchAnimationFrameDuration = .05f;
    public float catchAnimationUpwardSpeed = 1f;
    public int catchAnimationSortingOrder;

    private SpriteRenderer spriteRenderer;

    public bool Caught { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine(AnimationCoroutine());   
    }

    private IEnumerator AnimationCoroutine()
    {
        int spriteIndex = 0;
        float t = 0f;
        while(enabled && sprites != null && sprites.Length > 0)
        {
            spriteRenderer.sprite = sprites[spriteIndex];
            spriteRenderer.transform.localPosition = Vector3.up * floatAnimationAmplitude * Mathf.Sin(t * floatAnimationSpeed);
            yield return new WaitForSeconds(animationFrameDuration);
            spriteIndex = (spriteIndex + 1) % sprites.Length;
            t += animationFrameDuration;
        }
    }

    public void CatchAndDestroy()
    {
        Caught = true;
        StopAllCoroutines();
        StartCoroutine(CatchCoroutine());
    }

    private IEnumerator CatchCoroutine()
    {
        int spriteIndex = 0;
        float timer = 0f;
        while (enabled && sprites != null && sprites.Length > 0 && timer < catchAnimationDuration)
        {
            spriteRenderer.sortingOrder = catchAnimationSortingOrder;
            spriteRenderer.sprite = sprites[spriteIndex];
            spriteRenderer.color = new(1f, 1f, 1f, 1f - timer / catchAnimationDuration);
            transform.position += (catchAnimationUpwardSpeed * catchAnimationFrameDuration) * Vector3.up;
            yield return new WaitForSeconds(catchAnimationFrameDuration);
            spriteIndex = (spriteIndex + 1) % sprites.Length;
            timer += catchAnimationFrameDuration;
        }
        Destroy(gameObject);
    }
}
