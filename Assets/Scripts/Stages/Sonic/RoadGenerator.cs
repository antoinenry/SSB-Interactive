using System.Collections.Generic;
using UnityEngine;

namespace Sonic
{
    public class RoadGenerator : MonoBehaviour
    {
        [System.Serializable]
        public struct DifficultySetting
        {
            public float checkpointDistance;
            public FloatRange carSpeed;
            public FloatRange carSpacing;
            public float minGapDuration;
            [Range(0f, 1f)] public float reverseChance;
            [Range(0f, 1f)] public float itemChance;
        }

        public Transform roadCrosser;
        public float generateRadius;
        public CarSpawner roadPrefab;
        public FloatRangeDiscrete roadSpacing;
        public float roadLength;
        public Transform checkPointPrefab;
        public float checkpointSafeSpace;
        public Coin coinPrefab;
        public FloatRange coinPositionY;
        public DifficultySetting currentDifficulty;
        public DifficultySetting[] difficultyCurve;
        public float difficultyStep;

        private List<CarSpawner> roads;
        private float lastCheckpointX;

        private void Awake()
        {
            roads = new List<CarSpawner>();
            GetComponentsInChildren(roads);
        }

        private void OnEnable()
        {
            if (MiniGameConfig.Current != null)
            {
                StageConfig config = MiniGameConfig.Current.Data.sonic;
                difficultyStep = config.difficultyStepLength;
                difficultyCurve = config.difficulty;
            }
        }

        private void Update()
        {
            UpdateDifficulty();
            ClearRoads();
            GenerateRoads();
        }

        private void ClearRoads()
        {
            List<CarSpawner> toClear = roads.FindAll(r => r != null && roadCrosser.position.x - r.transform.position.x > generateRadius);
            foreach (CarSpawner r in toClear) Destroy(r.gameObject);
            roads.RemoveAll(r => r == null);
        }

        private void GenerateRoads()
        {
            float lastRoadX = 0f;
            foreach (CarSpawner r in roads) if (r.transform.position.x > lastRoadX) lastRoadX = r.transform.position.x;
            if (lastRoadX < roadCrosser.position.x + generateRadius)
            {
                bool addCheckpoint = lastRoadX - lastCheckpointX >= currentDifficulty.checkpointDistance;
                CarSpawner newRoad = Instantiate(roadPrefab, transform, false);
                newRoad.transform.position = (lastRoadX + (addCheckpoint ? checkpointSafeSpace : roadSpacing.RandomValue)) * Vector3.right;
                newRoad.travelDistance = roadLength;
                newRoad.carSpeed = currentDifficulty.carSpeed.RandomValue;
                newRoad.SetCarEvery(currentDifficulty.carSpacing.RandomValue, currentDifficulty.minGapDuration);
                newRoad.reverseDirection = (currentDifficulty.reverseChance > 0f && Random.Range(0f, 1f) < currentDifficulty.reverseChance);
                if (currentDifficulty.itemChance > 0f && Random.Range(0f, 1f) < currentDifficulty.itemChance)
                {
                    Coin newCoin = Instantiate(coinPrefab, newRoad.transform);
                    newCoin.transform.localPosition = new Vector3(0f, coinPositionY.RandomValue, 0f);
                }
                roads.Add(newRoad);
                if (addCheckpoint)
                {
                    Transform checkpoint = Instantiate(checkPointPrefab, transform, false);
                    lastCheckpointX = .5f * (newRoad.transform.position.x + lastRoadX);
                    checkpoint.position = lastCheckpointX * Vector2.right;
                }
            }
        }

        private void UpdateDifficulty()
        {
            int index = Mathf.FloorToInt((roadCrosser.position.x - transform.position.x) / difficultyStep);
            index = Mathf.Clamp(index, 0, difficultyCurve.Length - 1);
            if (index >= 0 && index < difficultyCurve.Length) currentDifficulty = difficultyCurve[index];
        }
    }
}