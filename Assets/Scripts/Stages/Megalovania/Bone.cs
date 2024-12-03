using UnityEngine;

namespace Megalovania
{
    public class Bone : Hazard
    {
        public float speed = 1f;
        public float lifeDistance = 10f;
        public float spriteHeightOffset = .5f;

        private Rigidbody2D body;
        private Vector2 startPosition;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();        
        }

        private void FixedUpdate()
        {
            body.velocity = speed * (transform.rotation * Vector2.right);
            if (Vector2.Distance(startPosition, transform.localPosition) > lifeDistance) Destroy(gameObject);
        }

        public void SetStartPosition(Vector2 position, float rotation)
        {
            startPosition = position;
            transform.localPosition = position;
            transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
        }

        public void SetSize(float value)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            sprite.size = new(sprite.size.x, value + spriteHeightOffset);
            boxCollider.size = sprite.size;
            boxCollider.offset = new(0f, value / 2f - spriteHeightOffset);
        }
    }
}
