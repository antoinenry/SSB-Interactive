using System;
using System.Collections;
using UnityEngine;

namespace Megalovania
{
    public class BulletSpawner : HazardSpawner
    {
        [Serializable]
        public struct Burst
        {
            public float startTime;
            public float duration;
            public float timeBetweenBursts;
            public float angle;
            public int bulletCount;
            public float bulletSize;
            public float bulletSpeed;
            public float timeBetweenBullets;
            public float linearSpread;
            public float angularSpread;

            public Burst Scaled
            {
                get
                {
                    if (SpeedScale == 0f) return this;
                    Burst scaled = this;
                    scaled.startTime = startTime / SpeedScale;
                    scaled.duration = duration / SpeedScale;
                    scaled.timeBetweenBursts = timeBetweenBursts / SpeedScale;
                    scaled.bulletSpeed = bulletSpeed * SpeedScale;
                    scaled.timeBetweenBullets = timeBetweenBullets / SpeedScale;
                    return scaled;
                }
            }
        }

        public Bullet bulletPrefab;
        public Burst[] bursts;
        public float distanceFromFrame = 1f;

        public override void StartSpanwing()
        {
            foreach (Burst b in bursts) StartCoroutine(SpawnCoroutine(b.Scaled));
        }

        private IEnumerator SpawnCoroutine(Burst burst)
        {
            yield return new WaitForSeconds(burst.startTime);
            float startTime = Time.time;
            while (Time.time < startTime + burst.duration)
            {
                for (int i = 0; i < burst.bulletCount; i++)
                {
                    SpawnBullet(burst.angle,
                        ((float)i / (burst.bulletCount - 1) - .5f) * burst.linearSpread,
                        ((float)i / (burst.bulletCount - 1) - .5f) * burst.angularSpread,
                        burst.bulletSize, burst.bulletSpeed);
                    if (burst.timeBetweenBullets > 0f) yield return new WaitForSeconds(burst.timeBetweenBullets);
                }
                yield return new WaitForSeconds(burst.timeBetweenBursts);
            }
            while (spawnedHazards != null && spawnedHazards.Find(h => h != null)) yield return null;
            onEnd.Invoke();
        }

        private void GetSpawnPosition(float angle, float linearOffset, float angularOffset, out Vector2 position, out float rotation)
        {
            position = Vector2.zero;
            rotation = 0f;
            if (frame == null) return;
            angle = Mathf.Repeat(angle, 360f);
            position = Quaternion.Euler(0f, 0f, angle) * Vector2.down;
            position = Vector2.Scale(position, frame.size / 2f + distanceFromFrame * Vector2.one);
            position += linearOffset * (Vector2)(Quaternion.Euler(0f, 0f, angle) * Vector2.right);
            rotation = angle - angularOffset; 
        }

        public void SpawnBullet(float angle, float linearOffset, float angularOffset, float size, float speed)
        {
            Bullet b = Instantiate(bulletPrefab, transform);
            GetSpawnPosition(angle, linearOffset, angularOffset, out Vector2 pos, out float r);
            b.SetStartPosition(pos, r);
            b.SetSize(size);
            b.speed = speed;
            spawnedHazards.Add(b);
            b.onKillsPlayer.AddListener(OnKillsPlayer);
        }
    }
}
