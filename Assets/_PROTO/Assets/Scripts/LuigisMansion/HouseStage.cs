using UnityEngine;

public class HouseStage : MonoBehaviour
{
    public SpriteRenderer frame;
    public Transform background;
    public SpriteRenderer darkness;
    public Stairs stairs;
    public Vector2 size;
    public float frameThickness;
    [Range(0f, 1f)] public float stairPosition;
    public Coin coinPrefab;
    public float itemHeight;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, size);
    }

    public void SetWidth(float w)
    {
        size.x = Mathf.Max(0f, w);
        if (frame != null)
        {
            frame.size = size + 2f * frameThickness * Vector2.one;
            frame.transform.localPosition = Vector2.zero;
        }
        if (background != null)
        {
            background.localScale = size;
        }
        if (darkness != null)
        {
            darkness.size = size;
        }
    }

    public void PlaceStairs(float relativeX)
    {
        stairPosition = Mathf.Repeat(relativeX, 1f);
        if (stairs != null)
        {
            float stairsX = (stairPosition - .5f) * size.x;
            stairsX = Mathf.Clamp(stairsX, -.5f * size.x + stairs.Size.x, .5f * size.x - stairs.Size.x);
            float stairsY = -size.y / 2f;
            stairs.transform.localPosition = new Vector3(stairsX, stairsY, 0f);
        }
    }

    public void SetDarkness(int sortingOrder)
    {
        darkness.sortingOrder = sortingOrder;
    }

    public void PlaceCoins(int count)
    {
        if (count <= 0) return;
        float spacing = 1f / count;
        float relativeX = Random.Range(0f, 1f);
        for (int i = 0; i < count; i++)
        {
            Coin item = Instantiate(coinPrefab, transform);
            item.transform.localPosition = new Vector3((relativeX - .5f) * size.x, -.5f * size.y + itemHeight, 0f);
            relativeX = Mathf.Repeat(relativeX + spacing, 1f);
        }
    }
}
