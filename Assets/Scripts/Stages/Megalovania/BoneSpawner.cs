using System;
using System.Collections;
using UnityEngine;

namespace Megalovania
{
    public class BoneSpawner : HazardSpawner
    {
        [Serializable]
        public struct Burst
        {
            public float startTime;
            public float duration;
            public float timeBetweenBursts;
            public EdgePosition edge;
            public int boneCount;
            public float boneSize;
            public float boneSpeed;
            public float boneSpacing;
            public float sizeVariationAmplitude;
            public float sizeVariationFrequency;

            public float GetBoneSize(int boneIndex)
            {
                if (sizeVariationAmplitude <= 0f) return boneSize;
                if (sizeVariationFrequency == 0f) return boneSize + .5f * sizeVariationAmplitude * UnityEngine.Random.Range(-1f, 1f);
                else return boneSize + .5f * sizeVariationAmplitude * Mathf.Cos(2f * Mathf.PI * boneIndex * sizeVariationFrequency / boneCount);
            }

            public Burst Scaled
            {
                get
                {
                    if (SpeedScale == 0f) return this;
                    Burst scaled = this;
                    scaled.startTime = startTime / SpeedScale;
                    scaled.duration = duration / SpeedScale;
                    scaled.timeBetweenBursts = timeBetweenBursts / SpeedScale;
                    scaled.boneSpeed = boneSpeed * SpeedScale;
                    return scaled;
                }
            }
        }

        public Bone bonePrefab;
        public Burst[] bursts;

        public override void StartSpanwing()
        {
            foreach (Burst b in bursts) StartCoroutine(SpawnCoroutine(b.Scaled));
        }

        private IEnumerator SpawnCoroutine(Burst burst)
        {
            yield return new WaitForSeconds(burst.startTime);
            float startTime = Time.time;
            int boneCount = 0;
            while (Time.time < startTime + burst.duration)
            {
                for (int i = 0; i < burst.boneCount; i++)
                {
                    SpawnBone(burst.edge, burst.GetBoneSize(boneCount++), burst.boneSpeed);
                    if (burst.boneSpacing > 0f && burst.boneSpeed != 0f) yield return new WaitForSeconds(Mathf.Abs(burst.boneSpacing / burst.boneSpeed));
                    else break;
                }
                yield return new WaitForSeconds(burst.timeBetweenBursts);
            }
            while (spawnedHazards != null && spawnedHazards.Find(h => h != null)) yield return null;
            onEnd.Invoke();
        }

        private void GetSpawnPosition(EdgePosition edge, bool direction, out Vector2 position, out float rotation)
        {
            position = Vector2.zero;
            rotation = 0f;
            if (frame == null) return;
            float side = direction ? -1f : 1f;
            Vector2 offset = frame.size / 2;
            switch (edge)
            {
                case EdgePosition.Bottom: 
                    position = new Vector2(side * offset.x, -offset.y);
                    rotation = 0f;
                    break;
                case EdgePosition.Top:
                    position = new Vector2(-side * offset.x, offset.y);
                    rotation = 180f;
                    break;
                case EdgePosition.Left:
                    position = new Vector2(-offset.x, -side * offset.y);
                    rotation = -90f;
                    break;
                case EdgePosition.Right:
                    position = new Vector2(offset.x, side * offset.y);
                    rotation = 90f;
                    break;
            }
        }

        public void SpawnBone(EdgePosition edge, float size, float speed)
        {
            Bone b = Instantiate(bonePrefab, transform);
            GetSpawnPosition(edge, speed > 0f, out Vector2 pos, out float r);
            b.SetStartPosition(pos, r);
            b.SetSize(size);
            b.speed = speed;
            spawnedHazards.Add(b);
            b.onKillsPlayer.AddListener(OnKillsPlayer);
        }
    }
}
