using System.Collections;
using UnityEngine;

namespace Megalovania
{
    public class Laser : Hazard
    {
        public bool shoot = true;
        public float length = 15f;
        public float spreadSpeed = 5f;
        public float linearSpeed = 1f;
        public float linearAmplitude = 2f;
        public float angularSpeed = 30f;
        public float angularAmplitude = 90f;

        private LineRenderer line;
        private BoxCollider2D boxCollider;
        private float startTime;
        private Vector2 centerPosition;
        private float linearOffset;
        private float centerRotation;
        private float angularOffset;
        private float animatedLength;

        private void Awake()
        {
            line = GetComponent<LineRenderer>();
            line.enabled = false;
            boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.enabled = false;
        }

        private void FixedUpdate()
        {
            SpreadAnimation();
            Movement();
        }

        public void SetStartPosition(Vector2 position, float positionOffset, float rotation, float rotationOffset)
        {
            startTime = Time.fixedTime;
            centerRotation = rotation;
            angularOffset = rotationOffset;
            transform.localRotation = Quaternion.Euler(0f, 0f, rotation + rotationOffset);
            centerPosition = position;
            linearOffset = positionOffset;
            transform.localPosition = position + linearOffset * (Vector2)(transform.localRotation * Vector3.right);
        }

        public void SetMovement(LaserSpawner.Burst burstInfo)
        {
            linearSpeed = burstInfo.linearSpeed;
            linearAmplitude = burstInfo.linearMovement;
            angularSpeed = burstInfo.angularSpeed;
            angularAmplitude = burstInfo.angularMovement;
        }

        private void SpreadAnimation()
        {
            float animatedPos;
            if (shoot)
            {
                animatedLength = Mathf.MoveTowards(animatedLength, length, spreadSpeed * Time.fixedDeltaTime);
                animatedPos = 0f;
            }
            else
            {
                animatedLength = Mathf.MoveTowards(animatedLength, 0f, spreadSpeed * Time.fixedDeltaTime);
                animatedPos = length - animatedLength;
            }
            if (animatedLength > 0f)
            {
                boxCollider.enabled = true;
                line.enabled = true;
                boxCollider.size = new(boxCollider.size.x, animatedLength);
                boxCollider.offset = new(boxCollider.offset.x, animatedPos + .5f * animatedLength);
                line.SetPosition(0, animatedPos * Vector2.up);
                line.SetPosition(1, (animatedPos + animatedLength) * Vector3.up);
            }
            else
            {
                boxCollider.enabled = false;
                line.enabled = false;
            }
        }

        private void Movement()
        {
            float t = Time.fixedTime - startTime;
            float angularMovement = angularOffset + Mathf.PingPong(t * angularSpeed, angularAmplitude) - .5f * angularAmplitude;
            if (!float.IsNaN(angularMovement)) transform.localRotation = Quaternion.Euler(0f, 0f, centerRotation + angularMovement);
            float linearMovement = linearOffset + Mathf.PingPong(t * linearSpeed, linearAmplitude) - .5f * linearAmplitude;
            if (!float.IsNaN(linearMovement)) transform.localPosition = centerPosition + linearMovement * (Vector2)(Quaternion.Euler(0f, 0f, centerRotation) * Vector3.right);
        }

        public void Destroy() => StartCoroutine(DestroyCoroutine());

        private IEnumerator DestroyCoroutine()
        {
            while (animatedLength > 0f)
            {
                shoot = false;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
