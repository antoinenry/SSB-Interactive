using UnityEngine;

public class DestroyOnPositionRange : MonoBehaviour
{
    public Rect range;
    public bool destroyOnExit;

    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (body != null) return;
        if (IsInsideRange(transform.position)) Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (body == null) return;
        if (IsInsideRange(body.position)) Destroy(gameObject);
    }

    private bool IsInsideRange(Vector2 pos)
        => pos.x >= range.xMin && pos.x <= range.xMax && pos.y >= range.yMin && pos.y <= range.yMax;
}
