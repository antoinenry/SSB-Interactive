using UnityEngine;

public class Cuphead : MonoBehaviour
{
    public enum Mode { FreeMove, FollowPanelChoice }
    public enum Health { Alive, Dead, Ressucitating }

    public Mode movementMode;
    public Health health;
    public float walkSpeed;
    public float walkDuration;
    public float deathDuration;
    public float ressucitateDuration;
    public float bodyWidth;
    public Sprite idleSprite;
    public Sprite[] walkSprites;
    public Sprite deadSprite;
    public float walkAnimationRate;
    public float ressucitateBlinkRate;
    public int coinLossOnDamage;

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private CoinCatcher coins;
    private float walkTimer;
    private float walkDirection;
    private ChoicePanel choicePanel;
    private float cameraX;
    private float cameraHalfWidth;
    private float deathTimer;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        choicePanel = FindObjectOfType<ChoicePanel>(true);
        spriteRenderer = GetComponent<SpriteRenderer>();
        coins = GetComponent<CoinCatcher>();
    }

    private void OnEnable()
    {
        spriteRenderer.enabled = true;        
    }

    private void OnDisable()
    {
        spriteRenderer.enabled = false;
    }

    private void Update()
    {
        Camera cam = Camera.main;
        cameraX = cam.transform.position.x;
        cameraHalfWidth = cam.aspect * cam.orthographicSize;
        if (health == Health.Dead)
        {
            spriteRenderer.sprite = deadSprite;
            deathTimer += Time.deltaTime;
            if (deathTimer > deathDuration)
            {
                deathTimer = 0f;
                health = Health.Ressucitating;
            }
        }
        else
        {
            if (walkDirection == 0)
            {
                spriteRenderer.sprite = idleSprite;
            }
            else
            {
                int animationIndex = Mathf.FloorToInt(Time.time * walkAnimationRate * walkSpeed) % walkSprites.Length;
                spriteRenderer.sprite = walkSprites[animationIndex];
            }
            if (health == Health.Ressucitating)
            {
                spriteRenderer.enabled = Mathf.FloorToInt(ressucitateBlinkRate * Time.time) % 2 == 0;
                deathTimer += Time.deltaTime;
                if (deathTimer > ressucitateDuration)
                {
                    deathTimer = 0f;
                    health = Health.Alive;
                    spriteRenderer.enabled = true;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        movementMode = choicePanel.enabled && !choicePanel.Locked ? Mode.FollowPanelChoice : Mode.FreeMove;
        if (health == Health.Dead)
        {
            walkDirection = 0;
            body.velocity = Vector2.zero;
        }
        else
        {
            switch (movementMode)
            {
                case Mode.FreeMove:
                    FreeMove();
                    break;
                case Mode.FollowPanelChoice:
                    FollowPanelChoice();
                    break;
            }
            if (body.position.x > cameraX + cameraHalfWidth + .5f * bodyWidth) body.position = new Vector2(cameraX - cameraHalfWidth - .5f * bodyWidth, body.position.y);
            else if (body.position.x < cameraX - cameraHalfWidth - .5f * bodyWidth) body.position = new Vector2(cameraX + cameraHalfWidth + .5f * bodyWidth, body.position.y);
        }   
    }

    private void FreeMove()
    {
        float input = AudienceInputSource.Current.GetHorizontalAxis().deltaPresses;
        if (input > 0f) WalkRight();
        else if (input < 0f) WalkLeft();
        else StayIdle();
        //if (input == 0f)
        //{
        //    walkTimer += Time.fixedDeltaTime;
        //    if (walkTimer > walkDuration) StayIdle();
        //}
        //else if (input > 0f)
        //{
        //    WalkRight();
        //    walkTimer = 0f;
        //}
        //else
        //{
        //    WalkLeft();
        //    walkTimer = 0f;
        //}
    }

    private void FollowPanelChoice()
    {
        float choiceX = 0f;
        if (choicePanel.selected == ChoicePanel.Choice.Left) choiceX = cameraX - cameraHalfWidth / 2f - bodyWidth;
        if (choicePanel.selected == ChoicePanel.Choice.Right) choiceX = cameraX + cameraHalfWidth / 2f + bodyWidth;
        if (body.position.x < choiceX - .5f * bodyWidth) WalkRight();
        else if (body.position.x > choiceX + .5f * bodyWidth) WalkLeft();
        else StayIdle();
    }

    private void WalkRight()
    {
        walkDirection = 1;
        transform.localScale = new Vector3(1f, 1f, 1f);
        body.velocity = walkSpeed * Vector2.right;
    }

    private void WalkLeft()
    {
        walkDirection = -1;
        transform.localScale = new Vector3(-1f, 1f, 1f);
        body.velocity = walkSpeed * Vector2.left;
    }

    private void StayIdle()
    {
        walkDirection = 0;
        body.velocity = Vector2.zero;
    }

    public void Die()
    {
        health = Health.Dead;
        walkDirection = 0;
        body.velocity = Vector2.zero;
        coins.LoseCoins(coinLossOnDamage);
    }
}
