using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScroller : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Camera cam = Camera.main;
        float cameraX = cam.transform.position.x;
        float cameraWidth = cam.orthographicSize * cam.aspect * 2f;
        Sprite sprite = spriteRenderer.sprite;
        float spriteWidth = sprite.texture.width / sprite.pixelsPerUnit;
        spriteRenderer.size = new(cameraWidth + spriteWidth * 2f, spriteRenderer.size.y);
        transform.position = new(cameraX - (cameraX % spriteWidth), transform.position.y);
    }
}
