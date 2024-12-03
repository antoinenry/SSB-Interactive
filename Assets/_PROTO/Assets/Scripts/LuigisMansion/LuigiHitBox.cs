using UnityEngine;
using UnityEngine.Events;

public class LuigiHitBox : MonoBehaviour
{
    public UnityEvent onHit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Ghost ghost = collision.GetComponent<Ghost>();
        if (ghost != null ) onHit.Invoke();
    }
}