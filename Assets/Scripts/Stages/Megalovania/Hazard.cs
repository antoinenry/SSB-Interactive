using UnityEngine;
using UnityEngine.Events;

namespace Megalovania
{
    public class Hazard : MonoBehaviour
    {
        public UnityEvent onKillsPlayer;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            HeartPlayer player = collision.transform.parent.GetComponent<HeartPlayer>();
            if (player)
            {
                player.Die();
                onKillsPlayer.Invoke();
            }
        }
    }

}