using System;
using System.Collections;
using UnityEngine;

namespace Megalovania
{
    public class LaserSpawner : HazardSpawner
    {
        [Serializable]
        public struct Burst
        {
            public float startTime;
            public float duration;
            public float laserDuration;
            public float timeBetweenLasers;
            public EdgePosition edge;
            public float linearOffset;
            public float linearMovement;
            public float linearSpeed;
            public float angularOffset;
            public float angularMovement;
            public float angularSpeed;

            public Burst Scaled
            {
                get
                {
                    Burst scaled = this;
                    scaled.startTime = startTime / SpeedScale;
                    scaled.duration = duration / SpeedScale;
                    scaled.laserDuration = laserDuration / SpeedScale;
                    scaled.timeBetweenLasers = timeBetweenLasers / SpeedScale;
                    scaled.linearSpeed = linearSpeed * SpeedScale;
                    scaled.angularSpeed = angularSpeed * SpeedScale;
                    return scaled;
                }
            }
        }

        public Laser laserPrefab;
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
            GetSpawnPosition(burst.edge, out Vector2 position, out float rotation);
            Laser l = SpawnLaser(position, burst.linearOffset, rotation, burst.angularOffset);
            l.SetMovement(burst);
            while (Time.time < startTime + burst.duration)
            {
                l.shoot = true;
                yield return new WaitForSeconds(burst.laserDuration);
                l.shoot = false;
                yield return new WaitForSeconds(burst.timeBetweenLasers);
            }
            l.Destroy();
            onEnd.Invoke();
        }

        public Laser SpawnLaser(Vector2 position, float positionOffset, float rotation, float rotationOffset)
        {
            Laser l = Instantiate(laserPrefab, transform);
            l.SetStartPosition(position, positionOffset, rotation, rotationOffset);
            spawnedHazards.Add(l);
            l.onKillsPlayer.AddListener(OnKillsPlayer);
            return l;
        }

        private void GetSpawnPosition(EdgePosition edge, out Vector2 position, out float rotation)
        {
            position = Vector2.zero;
            rotation = 0f;
            if (frame == null) return;
            Vector2 offset = frame.size / 2 + distanceFromFrame * Vector2.one;
            switch (edge)
            {
                case EdgePosition.Bottom:
                    position = new Vector2(0f, -offset.y);
                    rotation = 0f;
                    break;
                case EdgePosition.Top:
                    position = new Vector2(0f, offset.y);
                    rotation = 180f;
                    break;
                case EdgePosition.Left:
                    position = new Vector2(-offset.x, 0f);
                    rotation = -90f;
                    break;
                case EdgePosition.Right:
                    position = new Vector2(offset.x, 0f);
                    rotation = 90f;
                    break;
            }
        }
    }
}
