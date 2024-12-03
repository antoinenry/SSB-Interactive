using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Megalovania
{
    public class SoloChoice : MonoBehaviour
    {
        public string text = "PLUS FORT";
        public float scaleAnimationAmplitude = .1f;
        public float scaleAnimationFrequency = 1f;
        public float catchAnimationDuration = 2f;
        public float catchAnimationScale = 1f;

        public UnityEvent onCaught;

        public bool Caught { get; private set; }

        private Messenger adminMessenger;
        private TMP_Text textField;
        private SpriteRenderer sprite;
        private float catchTime;

        private void Awake()
        {
            adminMessenger = FindObjectOfType<Messenger>(true);
            textField = GetComponentInChildren<TMP_Text>(true);
            sprite = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            float t = Time.time;
            textField.text = text;
            if (Caught)
            {
                transform.localScale = Vector3.one * (1f + catchAnimationScale * (t - catchTime));
                textField.color = new Color(1f, 1f, 1f, 1f - (t - catchTime) / catchAnimationDuration);
                if (t > catchTime + catchAnimationDuration) Destroy(gameObject);
            }
            else
            {
                transform.localScale = Vector3.one * (1f + scaleAnimationAmplitude * Mathf.Sin(scaleAnimationFrequency * Mathf.PI * 2f * t));
                textField.color = Color.white;
            }
            sprite.color = textField.color;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (Caught) return;
            Caught = true;
            catchTime = Time.time;
            adminMessenger.Send(text);
            onCaught.Invoke();
        }
    }
}
