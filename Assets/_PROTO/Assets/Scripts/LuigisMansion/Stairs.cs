using UnityEngine;

public class Stairs : MonoBehaviour
{
    public int stepCount;
    public Vector2 stepSize;
    public Rigidbody2D user;
    public Vector2 sizeMargin;

    public Vector2 Size => stepSize * stepCount + sizeMargin;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < stepCount; i++)
        {
            Vector3 pt1 = new Vector2(i * stepSize.x, i * stepSize.y);
            Vector3 pt2 = new Vector2(i * stepSize.x, (i + 1) * stepSize.y);
            Vector3 pt3 = new Vector2((i + 1) * stepSize.x, (i + 1) * stepSize.y);
            Gizmos.DrawLine(transform.position + pt1, transform.position + pt2);
            Gizmos.DrawLine(transform.position + pt2, transform.position + pt3);
        }
    }

    private void FixedUpdate()
    {
        if (user == null) return;
        Vector2 relativeBodyPos = user.position - (Vector2)transform.position;
        int step = Mathf.CeilToInt(relativeBodyPos.x / stepSize.x);
        if (   step < 0 
           || step > stepCount
           || (relativeBodyPos.y < stepSize.y / 2f && relativeBodyPos.x > stepSize.x)
           )
        {
            user = null;
        }
        else
        {
            relativeBodyPos.y = stepSize.y * step;
            user.position = (Vector2)transform.position + relativeBodyPos;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Luigi luser = collision.GetComponent<Luigi>();
        if (luser != null && luser.LookingUp) user = collision.attachedRigidbody;
    }
}
