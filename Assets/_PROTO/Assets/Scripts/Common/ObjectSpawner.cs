using UnityEngine;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct ObjectChance
    {
        public Transform prefab;
        [Min(0f)] public float chance;
    }

    public ObjectChance[] objects;
    public Rect spawnZone;
    public float secondsPerSpawn;

    private float spawnTimer;
    private List<Transform> spawnedObjects;

    public Transform[] Spawns => spawnedObjects != null ? spawnedObjects.ToArray() : new Transform[0];

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + (Vector3)spawnZone.center, spawnZone.size);
    }

    private void Update()
    {
        if (spawnedObjects == null) spawnedObjects = new();
        else spawnedObjects.RemoveAll(o => o == null);
        if (secondsPerSpawn > 0)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer > secondsPerSpawn)
            {
                spawnedObjects.Add(Spawn());
                spawnTimer = 0f;
            }
        }
    }

    public Transform Spawn()
    {
        int prefabCount = objects != null ? objects.Length : 0;
        if (prefabCount == 0) return null;
        Transform selectPrefab = objects[0].prefab;
        if (prefabCount > 1)
        {
            float chanceSum = 0;
            foreach (ObjectChance o in objects) chanceSum += o.chance;
            float randomFloat = Random.Range(0, chanceSum);
            chanceSum = 0;
            foreach (ObjectChance o in objects)
            {
                if (randomFloat < o.chance + chanceSum)
                {
                    selectPrefab = o.prefab;
                    break;
                }
                else
                    chanceSum += o.chance;
            }
        }
        Transform spawn = Instantiate(selectPrefab, transform);
        spawn.localPosition = new(Random.Range(spawnZone.xMin, spawnZone.xMax), Random.Range(spawnZone.yMin, spawnZone.yMax));
        return spawn;
    }
}
