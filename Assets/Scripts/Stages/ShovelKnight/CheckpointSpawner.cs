using UnityEngine;

namespace ShovelKnight
{    
    public class CheckpointSpawner : MonoBehaviour
    {
        public Checkpoint checkpointModel;
        public Transform player;
        public float checkpointHeight = 30f;
        public float spawnAdvance = 10f;
        public Color[] colorCycle;

        private float currentHeight;
        private Checkpoint lastCheckPoint;
        private Checkpoint currentCheckPoint;
        private VerticalPlatformsGenerator levelGenerator;

        private void Awake()
        {
            levelGenerator = FindObjectOfType<VerticalPlatformsGenerator>(true);
        }

        private void Update()
        {
            // Spawning in advance
            if (player.position.y + spawnAdvance >= currentHeight)
            {
                if (lastCheckPoint != null) Destroy(lastCheckPoint.gameObject);
                lastCheckPoint = currentCheckPoint;
                currentCheckPoint = Instantiate(checkpointModel, transform);
                currentCheckPoint.transform.position += currentHeight * Vector3.up;
                currentCheckPoint.Size = new(checkpointModel.Size.x, checkpointHeight);
                currentCheckPoint.baseColor = colorCycle[(int)(currentHeight / checkpointHeight) % colorCycle.Length];
                currentCheckPoint.dark = true;
                currentHeight += checkpointHeight;
                levelGenerator.rangeMax = currentHeight;
            }
            // Passing checkpoint
            if (currentCheckPoint != null && player.position.y >= currentCheckPoint.transform.position.y)
            {
                if (lastCheckPoint) lastCheckPoint.dark = true;
                currentCheckPoint.dark = false;
                levelGenerator.rangeMin = currentHeight - 2 * checkpointHeight;
            }
        }
    }
}
