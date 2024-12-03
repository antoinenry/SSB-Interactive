using System.Collections;
using UnityEngine;

namespace Sonic
{
    public class Car : MonoBehaviour
    {
        public Rigidbody2D body;
        public SpriteRenderer carRenderer;
        public SpriteRenderer fillRenderer;
        public SpriteRenderer wheelsRenderer;
        public float layerZSpacing;
        public Color[] colors;
        public Sprite frontSprite;
        public Sprite backSprite;
        public Sprite frontFill;
        public Sprite backFill;
        public float oscillationAmplitude;
        public float oscillationSpeed;
        public float crashOutSpeed = 10f;
        public float crashSpinSpeed = 1000f;
        public float crashDuration = 3f;

        private Collider2D[] colliders;
        private float fillOffsetY;
        private float spriteOffsetY;

        public bool HasCrashed { get; private set; }

        private void Awake()
        {
            colliders = new Collider2D[body.attachedColliderCount];
            body.GetAttachedColliders(colliders);
            fillOffsetY = fillRenderer.transform.localPosition.y;
            spriteOffsetY = carRenderer.transform.localPosition.y;
        }

        private void Update()
        {
            if (HasCrashed) return;
            float oscillate = oscillationAmplitude * Mathf.Sin(oscillationSpeed * transform.position.y);
            fillRenderer.transform.localPosition = new(fillRenderer.transform.localPosition.x, fillOffsetY + oscillate, fillRenderer.transform.localPosition.z);
            carRenderer.transform.localPosition = new(carRenderer.transform.localPosition.x, spriteOffsetY + oscillate, carRenderer.transform.localPosition.z);
        }

        public bool Collision
        {
            set
            {
                foreach (Collider2D c in colliders)
                    c.enabled = value;
            }
        }

        public Vector2 Position
        {
            get => body.position;
            set => body.MovePosition(value);
        }

        public int SortingOrder
        {
            set
            {
                carRenderer.sortingOrder = value;
                fillRenderer.sortingOrder = value;
                wheelsRenderer.sortingOrder = value;
            }
        }

        public float SpriteZ
        {
            set
            {
                Vector3 pos = carRenderer.transform.position;
                pos.z = value;
                carRenderer.transform.position = pos;

                pos = fillRenderer.transform.position;
                pos.z = value + layerZSpacing;
                fillRenderer.transform.position = pos;

                pos = wheelsRenderer.transform.position;
                pos.z = value + 2f * layerZSpacing;
                wheelsRenderer.transform.position = pos;
            }
        }

        public int ColorIndex
        {
            set
            {
                if (colors.Length > 0)
                    fillRenderer.color = colors[value % colors.Length];
            }
        }

        public bool FlipDirection
        {
            set
            {
                if (value == true)
                {
                    carRenderer.sprite = backSprite;
                    fillRenderer.sprite = backFill;
                }
                else
                {
                    carRenderer.sprite = frontSprite;
                    fillRenderer.sprite = frontFill;
                }
            }
        }

        public void Crash()
        {
            if (HasCrashed) return;
            StartCoroutine(CrashCoroutine());
        }

        private IEnumerator CrashCoroutine()
        {
            HasCrashed = true;
            Collision = false;
            float timer = 0f;
            float deltaTime;
            Vector2 direction = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)) * Vector3.right;
            while (timer < crashDuration)
            {
                deltaTime = Time.deltaTime;
                body.position += direction * crashOutSpeed * deltaTime;
                body.rotation += crashSpinSpeed * deltaTime;
                yield return null;
                timer += deltaTime;
            }
            Destroy(gameObject);
        }
    }
}