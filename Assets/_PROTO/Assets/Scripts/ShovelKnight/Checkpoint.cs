using UnityEngine;

namespace ShovelKnight
{
    public class Checkpoint : MonoBehaviour
    {
        public Color baseColor = Color.white;
        [Range(0f, 1f)] public float darkMultiplier = .5f;
        public bool dark;

        private SpriteRenderer render;

        private void Awake()
        {
            render = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            SetColor(dark ? darkMultiplier * baseColor : baseColor);
        }

        public Vector2 Size
        {
            get => render.size;
            set => render.size = value;
        }

        private void SetColor(Color c, bool setAlpha = false)
        {
            if (setAlpha) render.color = c;
            else render.color = new(c.r, c.g, c.b, render.color.a);
        }
    }
}
