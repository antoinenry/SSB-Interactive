using UnityEngine;

public class FloralBomb : MonoBehaviour
{
    public float oscillationAmplitude;
    public float oscilationRate;
    public Sprite explosionSprite;
    public float explosionDuration;
    public float explosionBlinkRate;
    public float destroyY;

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private float spawnTime;
    private bool exploded;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnTime = Time.time;
    }

    private void Update()
    {
        if (exploded)
        {
            spriteRenderer.sprite = explosionSprite;
            spriteRenderer.enabled = Mathf.FloorToInt(explosionBlinkRate * Time.time) % 2 == 0;
        }
    }

    private void FixedUpdate()
    {
        float angle = .5f * oscillationAmplitude * Mathf.Sin(oscilationRate * (Time.fixedTime - spawnTime) * Mathf.PI * 2f);
        body.MoveRotation(angle);
        if (body.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Cuphead cup = collision.GetComponent<Cuphead>();
        if (cup != null && cup.enabled && cup.health == Cuphead.Health.Alive)
        {
            cup.Die();
            Explode(cup.transform.position);
        }
    }

    private void Explode(Vector2 position)
    {
        exploded = true;
        body.simulated = false;
        body.velocity = Vector2.zero;
        transform.position = position;
        transform.rotation = Quaternion.identity;
        Destroy(gameObject, explosionDuration);
    }
}
