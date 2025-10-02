using UnityEngine;

public class Rayman : MonoBehaviour
{
    public Transform spawnPoint;
    public string floorTag = "Floor";
    public string hazardTag = "Hazard";
    public float horizontalSpeed = 1f;
    public float jumpForce = 10f;
    public float flyForce = 1f;
    public float fallGravityScale = 1f;
    public float glideGravityScale = .5f;
    public float respawnDelay = 2f;
    //public float directionDuration;
    [Header("Animation")]
    public Sprite onFloorSprite;
    public Sprite jumpSprite;
    public Sprite[] glideSprites;
    public float framePerSeconds;

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private bool onFloor;
    //private bool gliding;
    //private float directionTimer;
    private MiniGameScore score;
    private CaveGenerator cave;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        score = GetComponent<MiniGameScore>();
        cave = FindObjectOfType<CaveGenerator>(true);
    }

    private void OnEnable()
    {
        if (MiniGameConfig.Current != null)
        {
            MiniGameConfigData.RaymanConfig config = MiniGameConfig.Current.Data.rayman;
            horizontalSpeed = config.horizontalSpeed;
            jumpForce = config.jumpForce;
            flyForce = config.flyForce;
            fallGravityScale = config.fallGravity;
            glideGravityScale = config.glideGravity;
        }
    }

    private void Update()
    {
        if (onFloor) spriteRenderer.sprite = onFloorSprite;
        else if (body.gravityScale == glideGravityScale)
        {
            int animationIndex = Mathf.FloorToInt(Time.time * framePerSeconds) % glideSprites.Length;
            spriteRenderer.sprite = glideSprites[animationIndex];
        }
        else spriteRenderer.sprite = jumpSprite;
        score.unitValue = Mathf.Max(0f, transform.position.x);
    }

    public int direction;
    private void FixedUpdate()
    {
        if (transform.position.x < cave.rangeMin || transform.position.x > cave.rangeMax)
        {
            body.velocity = Vector2.zero;
            body.gravityScale = 0f;
            return;
        }
        float horizontalInput = AudienceInput.GetAxis(AudienceInputConfiguration.Axis.Direction.Horizontal);
        float verticalInput = AudienceInput.GetAxis(AudienceInputConfiguration.Axis.Direction.Vertical);
        if (onFloor)
        {
            if (verticalInput > 0f)
            {
                body.velocity = jumpForce * Vector2.up;
                body.gravityScale = glideGravityScale;
                onFloor = false;
            }
        }
        else
        {
            if (verticalInput > 0f && body.velocity.y < flyForce)
            {
                body.velocity = new(body.velocity.x, flyForce);
                body.gravityScale = glideGravityScale;
                //gliding = true;
            }

            //int direction = 0;
            if (horizontalInput == 0f)
            {
                direction = 0;
                //directionTimer += Time.fixedDeltaTime;
                //if (directionTimer > directionDuration) direction = 0;
            }
            else if (horizontalInput > 0f)
            {
                direction = 1;
                //directionTimer = 0f;
            }
            else
            {
                direction = -1;
                //directionTimer = 0f;
            }
            if (direction == 1)
                body.velocity = new(horizontalInput * horizontalSpeed, body.velocity.y);
            else if (direction == -1)
                body.velocity = new(horizontalInput * horizontalSpeed, body.velocity.y);
            if (verticalInput < 0f)
            {
                body.gravityScale = fallGravityScale;
                //gliding = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        body.gravityScale = fallGravityScale;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(floorTag))
        {
            onFloor = true;
            //score.value = Mathf.Max(0f, transform.position.x);
            //spawnPoint.position = transform.position;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(floorTag)) onFloor = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(hazardTag)) Die();
    }

    private void Die()
    {
        body.simulated = false;
        float respawnX = cave.GetLastFreeSpot(transform.position.x);
        if (!float.IsNaN(respawnX)) spawnPoint.position = new(respawnX, spawnPoint.position.y, spawnPoint.position.z);
        Invoke("Respawn", respawnDelay);
    }

    private void Respawn()
    {
        transform.position = spawnPoint.position;
        body.simulated = true;
    }
}
