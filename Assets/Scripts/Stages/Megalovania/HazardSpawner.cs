using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Megalovania
{
    public abstract class HazardSpawner : MonoBehaviour
    {
        public enum EdgePosition { Bottom, Top, Left, Right }

        static public float SpeedScale;

        public UnityEvent onEnd;
        public UnityEvent onKillsPlayer;

        protected Frame frame;
        protected List<Hazard> spawnedHazards;

        protected void Awake()
        {
            frame = FindObjectOfType<Frame>(true);
            spawnedHazards = new();
        }

        protected void OnEnable()
        {
            StartSpanwing();
        }

        protected void OnDisable()
        {
            StopSpawning();
        }

        public abstract void StartSpanwing();

        public virtual void StopSpawning()
        {
            StopAllCoroutines();
            if (spawnedHazards != null)
            {
                foreach (Hazard h in spawnedHazards)
                {
                    if (h != null) Destroy(h.gameObject);
                }
            }
        }

        protected void OnKillsPlayer()
        {
            onKillsPlayer.Invoke();
            Destroy(gameObject);
        }
    }
}
