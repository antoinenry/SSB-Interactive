using UnityEngine;

namespace Megalovania
{
    public class Bullet : Hazard
    {
        public float speed = 1f;
        public float lifeDistance = 10f;

        private Rigidbody2D body;
        private Vector2 startPosition;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            body.velocity = speed * (transform.rotation * Vector2.up);
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
            transform.localScale = value * Vector3.one;
        }
    }
}
