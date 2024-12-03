using UnityEngine;

namespace Megalovania
{
    public class SoloChoiceSpawner : MonoBehaviour
    {
        public SoloChoice choicePrefab;
        public string[] choices;
        public Vector2[] spawnPositions;
        public float timeBetweenChoices;

        private SoloChoice[] spawnedChoices;
        private int choiceIndex;
        private float lastChoiceTime;
        private HeartPlayer player;

        private void Awake()
        {
            player = FindObjectOfType<HeartPlayer>(true);
        }

        private void OnEnable()
        {
            lastChoiceTime = Time.time;
        }

        private void OnDisable()
        {
            if (spawnedChoices == null) return;
            foreach (SoloChoice c in spawnedChoices) Destroy(c.gameObject);
            spawnedChoices = null;
        }

        private void Update()
        {
            if (spawnedChoices == null)
            {
                if (Time.time < lastChoiceTime + timeBetweenChoices) return;
                SpawnChoices();
            }
        }

        private void SpawnChoices()
        {
            int spawnCount = spawnPositions.Length;
            int newChoiceIndex = choiceIndex;
            spawnedChoices = new SoloChoice[spawnCount];
            for (int i = 0; i < spawnCount; i++)
            {
                SoloChoice c = Instantiate(choicePrefab, transform);
                c.transform.localPosition = spawnPositions[i];
                c.text = choices[(choiceIndex + i) % choices.Length];
                c.onCaught.AddListener(OnChoice);
                spawnedChoices[i] = c;
            }
            choiceIndex += spawnCount;
            player.transform.localPosition = Vector3.zero;
        }

        private void OnChoice()
        {
            if (spawnedChoices == null) return;
            foreach(SoloChoice c in spawnedChoices)
            {
                if (c == null || c.Caught) continue;
                Destroy(c.gameObject);
            }
            spawnedChoices = null;
            lastChoiceTime = Time.time;
        }
    }
}
