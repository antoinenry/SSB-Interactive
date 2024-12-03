using UnityEngine;
using TMPro;

namespace Megalovania
{
    public class RoundManager : MonoBehaviour
    {
        public HazardSpawner[] roundPrefabs;
        public bool changeRoundOnDeath = true;
        public Coin coinPrefab;
        [Range(0f, 1f)] public float coinChance = .5f;
        public float coinSpawnDelay = 5f;
        public float speed = .5f;
        public float speedIncrement = .01f;
        public TMP_Text roundsField;
        public string roundsFieldPrefix = "round ";

        private HazardSpawner currentRound;
        private int currentRoundPrefabIndex;

        public int RoundCount { get; private set; }

        private void OnDisable()
        {
            if (currentRound) Destroy(currentRound.gameObject);
        }

        private void Update()
        {
            HazardSpawner.SpeedScale = speed;
            if (currentRound != null) return;
            HazardSpawner roundPrefab = PickARound();
            if (roundPrefab == null) return;
            SpawnRound(roundPrefab);
            if (coinChance > 0f && Random.Range(0f, 1f) <= coinChance) Invoke("SpawnCoin", coinSpawnDelay);
            roundsField.text = roundsFieldPrefix + RoundCount;
        }

        private HazardSpawner PickARound()
        {
            if (roundPrefabs == null || roundPrefabs.Length == 0) return null;
            int i = Random.Range(0, roundPrefabs.Length - 1);
            if (i == currentRoundPrefabIndex) i = (i + 1) % roundPrefabs.Length;
            currentRoundPrefabIndex = i;
            return roundPrefabs[i];
        }

        private void SpawnRound(HazardSpawner roundPrefab)
        {
            currentRound = Instantiate(roundPrefab, transform);
            currentRound.onEnd.AddListener(OnRoundEnd);
            currentRound.onKillsPlayer.AddListener(OnPlayerDeath);
        }

        private void SpawnCoin()
        {
            if (currentRound == null) return;
            Frame frame = FindObjectOfType<Frame>();
            Instantiate(coinPrefab, currentRound.transform).transform.position = frame.RandomPoint();
        }

        private void OnRoundEnd()
        {
            Destroy(currentRound.gameObject);
            currentRound = null;
            RoundCount++;
            if (speed < 1f) speed += speedIncrement;
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