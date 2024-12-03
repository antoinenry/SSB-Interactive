using UnityEngine;

public class ShovelPlatform : MonoBehaviour
{
    static public int BounceCount;

    public float movingSpeed;
    public int blinkEveryBounce;
    public int blinkOffset;
    public Transform item;
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        body = GetComponentInChildren<Rigidbody2D>();
        boxCollider = GetComponentInChildren<BoxCollider2D>();
    }

    private void Start()
    {
        if (movingSpeed != 0f) spriteRenderer.sprite = sprites[1];
        else if (blinkEveryBounce != 0f) spriteRenderer.sprite = sprites[2];
        else spriteRenderer.sprite = sprites[0];
    }

    private void FixedUpdate()
    {
        if (movingSpeed != 0)
        {
            body.bodyType = RigidbodyType2D.Kinematic;
            if (Mathf.Abs(body.position.x - transform.position.x) > body.transform.localScale.x) movingSpeed = -movingSpeed;
            body.velocity = new Vector2(movingSpeed, 0f);
        }
        if (blinkEveryBounce > 0)
        {
            body.gameObject.SetActive((blinkOffset + BounceCount) % blinkEveryBounce != 0);
        }

    }

    public void SetWidth(float w)
    {
        spriteRenderer.size = new(w, spriteRenderer.size.y);
        boxCollider.size = new(w, boxCollider.size.y);
    }

    public void EnableItem(bool enable)
    {
        item.gameObject.SetActive(enable);
    }
}
