using UnityEngine;
using System.Collections.Generic;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace Megalovania
{
    [ExecuteAlways]
    public class Frame : MonoBehaviour
    {
        public Vector2 size;

        private SpriteRenderer sprite;
        private EdgeCollider2D box;
        private SpriteMask mask;

        private void Update()
        {
            if (sprite)
            {
                if (sprite.size == size) return;
                sprite.size = size;
            }
            else sprite = GetComponent<SpriteRenderer>();
            if (box)
            {
                float xHalf = .5f * size.x, yHalf = .5f * size.y;
                List<Vector2> points = new List<Vector2>(5)
                {
                    new(xHalf, yHalf),
                    new(xHalf, -yHalf),
                    new(-xHalf, -yHalf),
                    new(-xHalf, yHalf),
                    new(xHalf, yHalf)
                };
                box.SetPoints(points);
            }
            else box = GetComponent<EdgeCollider2D>();
            if (mask) mask.transform.localScale = (Vector3)size;
            else mask = GetComponentInChildren<SpriteMask>(true);
        }

        public Vector3 RandomPoint()
        {
            float x = Random.Range(-.5f, .5f) * size.x;
            float y = Random.Range(-.5f, .5f) * size.y;
            return transform.position + new Vector3(x, y);
        }
    }
}
