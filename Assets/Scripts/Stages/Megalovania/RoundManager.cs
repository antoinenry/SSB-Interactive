using UnityEngine;
using UnityEngine.Events;

namespace Megalovania
{
    public class RoundManager : MegalovaniaPhase
    {
        public HazardSpawner[] roundPrefabs;
        public bool changeRoundOnDeath = true;
        public float timeBetweenRounds = 2f;
        public Coin coinPrefab;
        [Range(0f, 1f)] public float coinChance = .5f;
        public float coinSpawnDelay = 5f;
        public float speed = .5f;
        public float speedIncrement = .01f;
        public UnityEvent onFinishRound;

        private HazardSpawner currentRound;
        private Coin spawnedCoin;
        private Frame frame;
        private float roundTimer;
        private int currentRoundPrefabIndex;

        private void Awake()
        {
            frame = FindObjectOfType<Frame>(true);
        }

        private void OnEnable()
        {
            roundTimer = timeBetweenRounds;
        }

        private void OnDisable()
        {
            roundTimer = 0f;
            if (currentRound) Destroy(currentRound.gameObject);
        }

        private void Update()
        {
            if (roundTimer < timeBetweenRounds)
            {
                roundTimer += Time.deltaTime;
                return;
            }
            HazardSpawner.SpeedScale = speed;
            if (currentRound != null) return;
            HazardSpawner roundPrefab = PickARound();
            SpawnRound(roundPrefab);
            if (spawnedCoin == null && coinChance > 0f && Random.Range(0f, 1f) <= coinChance) Invoke("SpawnCoin", coinSpawnDelay);
        }

        private HazardSpawner PickARound()
        {
            if (roundPrefabs == null || roundPrefabs.Length == 0) return null;
            int i = Random.Range(0, roundPrefabs.Length - 1);
            if (i == currentRoundPrefabIndex) i = (i + 1) % roundPrefabs.Length;
            currentRoundPrefabIndex = i;
            return roundPrefabs[i];
        }

        public void SpawnRound(HazardSpawner roundPrefab)
        {
            roundTimer = 0f;
            if (roundPrefab == null) return;
            currentRound = Instantiate(roundPrefab, transform);
            currentRound.onEnd.AddListener(OnRoundEnd);
            currentRound.onKillsPlayer.AddListener(OnPlayerDeath);
        }

        public void SpawnRound(int roundIndex)
        {
            if (roundPrefabs == null || roundPrefabs.Length == 0) return;
            SpawnRound(roundPrefabs[roundIndex]);
        }

        private void SpawnCoin()
        {
            if (coinPrefab == null) return;
            if (frame == null) frame = FindObjectOfType<Frame>();
            if (frame)
            {
                spawnedCoin = Instantiate(coinPrefab, frame.transform);
                spawnedCoin.transform.position = frame.RandomPoint();
            }
        }

        private void OnRoundEnd()
        {
            roundTimer = 0f;
            if (currentRound) Destroy(currentRound.gameObject);
            currentRound = null;
            if (speed < 1f) speed += speedIncrement;
            onFinishRound.Invoke();
        }

        private void OnPlayerDeath()
        {
            if (currentRound == null) return;       
            Destroy(currentRound.gameObject);
            if (!changeRoundOnDeath)
            {
                HazardSpawner roundPrefab = roundPrefabs[currentRoundPrefabIndex];
                SpawnRound(roundPrefab);
            }
        }
    }
}