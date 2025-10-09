using System;
using System.Collections;
using UnityEngine;

namespace Sonic
{
    public class Hedgehog : MonoBehaviour
    {
        [Serializable]
        public class SpriteSheet
        {
            public Sprite idleSprite;
            public Sprite[] walkSprites;
            public Sprite deadSprite;
        }

        public float walkSpeed;
        public float invicibleWalkSpeed;
        public float yStep;
        public float minY;
        public float maxY;
        public string hazardTag = "Hazard";
        public string checkpointTag = "Checkpoint";
        public float animationRate;
        public Transform cemetary;
        public float respawnDelay = 1f;
        public float respawnIdleDuration = 2f;
        public float respawnBlinkRate = 2f;
        public bool invincible;
        public SpriteSheet normalSprites;
        public SpriteSheet invincibleSprites;

        private Rigidbody2D body;
        private ParticleSystem invicibleParticles;
        private MiniGameScore score;
        private Vector2 respawnPosition;
        private SpriteRenderer spriteRenderer;
        private float walkDirection;
        private bool disableMovement;
        private SpriteSheet currentSprites;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            invicibleParticles = GetComponentInChildren<ParticleSystem>(true);
            score = GetComponent<MiniGameScore>();
            respawnPosition = body.position;
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            currentSprites = normalSprites;
        }

        private void OnEnable()
        {
            if (MiniGameConfig.Current != null)
            {
                StageConfig config = MiniGameConfig.Current.Data.sonic;
                walkSpeed = config.walkSpeed;
                invicibleWalkSpeed = config.invicibleWalkSpeed;
                //walkDuration = config.walkDuration;
                yStep = config.verticalStep;
            }
        }

        public Color invincibleColor = Color.yellow;
        public float invinciblieColorRate = 10f;

        private void Update()
        {
            float time = Time.time;
            if (invincible)
            {
                if (!invicibleParticles.isPlaying) invicibleParticles.Play();
                currentSprites = invincibleSprites;
                spriteRenderer.color = Color.Lerp(Color.white, invincibleColor, .5f + .5f * Mathf.Cos(time * invinciblieColorRate * 2f * Mathf.PI));
            }
            else
            {
                if (invicibleParticles.isPlaying) invicibleParticles.Stop();
                currentSprites = normalSprites;
                spriteRenderer.color = Color.white;
            }
            if (walkDirection == 0f)
            {
                spriteRenderer.sprite = currentSprites.idleSprite;
            }
            else
            {
                int animationIndex = Mathf.FloorToInt(time * animationRate * walkSpeed) % currentSprites.walkSprites.Length;
                spriteRenderer.sprite = currentSprites.walkSprites[animationIndex];
            }
            score.unitValue = transform.position.x;
        }

        private void FixedUpdate()
        {
            if (disableMovement)
            {
                walkDirection = 0f;
                body.velocity = Vector2.zero;
                return;
            }
            walkDirection = AudienceInput.GetAxis(AudienceInputConfiguration.Axis.Direction.Horizontal);
            if (walkDirection > 0f)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                body.velocity = (invincible ? invicibleWalkSpeed : walkSpeed) * Vector2.right;
            }
            else if (walkDirection < 0f)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
                body.velocity = (invincible ? invicibleWalkSpeed : walkSpeed) * Vector2.left;
            }
            else
            {
                body.velocity = Vector2.zero;
            }
            float bodyY = body.position.y;
            float verticalInput = AudienceInput.GetAxis(AudienceInputConfiguration.Axis.Direction.Vertical);
            if (verticalInput > 0f) bodyY += yStep;
            else if (verticalInput < 0f) bodyY -= yStep;
            bodyY = Mathf.Clamp(bodyY, minY, maxY);
            body.position = new(body.position.x, bodyY);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(hazardTag))
            {
                if (invincible)
                {
                    Car car = collision.GetComponent<Car>();
                    car?.Crash();
                }
                else
                {
                    DropDead();
                    StartCoroutine(RespawnCoroutine());
                }
            }
            if (collision.CompareTag(checkpointTag))
            {
                respawnPosition = body.position;
            }
        }

        private void DropDead()
        {
            GameObject deadObject = new GameObject("dead");
            deadObject.transform.parent = cemetary;
            deadObject.transform.position = transform.position;
            SpriteRenderer deadRenderer = deadObject.AddComponent<SpriteRenderer>();
            deadRenderer.sprite = currentSprites.deadSprite;
            deadRenderer.sortingOrder = spriteRenderer.sortingOrder;
        }

        private IEnumerator RespawnCoroutine()
        {
            disableMovement = true;
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(respawnDelay);
            body.position = respawnPosition;
            float idleTimer = 0f;
            while (idleTimer < respawnIdleDuration)
            {
                idleTimer += Time.deltaTime;
                spriteRenderer.enabled = (idleTimer * respawnBlinkRate) % 1f > .5f;
                yield return null;
            }
            spriteRenderer.enabled = true;
            disableMovement = false;
        }
    }
}