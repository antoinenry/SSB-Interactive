using System.Collections.Generic;
using UnityEngine;

namespace Sonic
{
    public class CarSpawner : MonoBehaviour
    {
        public Car carPrefab;
        public List<Car> cars;
        public float carSpeed;
        public bool reverseDirection;
        public List<float> spawnPositions;
        public float travelDistance;
        public float edgeTravelDistance;
        public float edgeTravelPerspective;
        public int frontSortingOrder;
        public int backSortingOrder;
        public float zSpacing;

        private float scroll;

        private void FixedUpdate()
        {
            if (cars.Count != spawnPositions.Count) SpawnCars();
            if (reverseDirection)
                scroll = Mathf.Repeat(scroll - carSpeed * Time.fixedDeltaTime, travelDistance);
            else
                scroll = Mathf.Repeat(scroll + carSpeed * Time.fixedDeltaTime, travelDistance);
            MoveCars();
        }

        private void SpawnCars()
        {
            for (int i = spawnPositions.Count; i < cars.Count; i++)
            {
                if (cars[i] != null) Destroy(cars[i].gameObject);
            }
            for (int i = cars.Count; i < spawnPositions.Count; i++)
                cars.Add(Instantiate(carPrefab, transform, false));
        }

        private void MoveCars()
        {
            for (int i = 0, iend = spawnPositions.Count; i < iend; i++)
            {
                Car car = cars[i];
                if (car == null || car.HasCrashed) continue;
                car.FlipDirection = reverseDirection;
                float carTravel = (spawnPositions[i] + scroll) % travelDistance;
                if (carTravel < travelDistance - edgeTravelDistance)
                {
                    car.Position = transform.position + carTravel * Vector3.down;
                    car.SortingOrder = frontSortingOrder;
                    car.Collision = true;
                }
                else
                {
                    float distanceToEdge = travelDistance - carTravel;
                    car.Position = transform.position + edgeTravelPerspective * (distanceToEdge / edgeTravelDistance) * Vector3.down;
                    car.SortingOrder = backSortingOrder;
                    car.Collision = false;
                }
                car.SpriteZ = -zSpacing * carTravel;
                car.ColorIndex = i;
            }
        }

        public void SetCarEvery(float spacing, float minimumGapDuration)
        {
            int positionCount = Mathf.FloorToInt(travelDistance / spacing);
            spawnPositions = new List<float>(positionCount);
            for (int i = 0; i < positionCount; i++) spawnPositions.Add(i * spacing);
            float minimumGapSpace = minimumGapDuration * carSpeed;
            if (spacing < minimumGapSpace && travelDistance % spacing < minimumGapSpace)
            {
                int removePositions = Mathf.CeilToInt(minimumGapSpace / spacing);
                int removePositionAt = Random.Range(0, positionCount);
                for (int i = 0; i < removePositions; i++)
                {
                    spawnPositions.RemoveAt(removePositionAt);
                    positionCount--;
                    if (positionCount == 0) break;
                    removePositionAt = removePositionAt % positionCount;
                }
            }
        }
    }
}