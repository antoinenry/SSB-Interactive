using UnityEngine;

[ExecuteAlways]
public class BouncingLogo : MonoBehaviour
{
    public float bodyRadius;
    public float inputForce;
    public Vector2 startVelocity;
    public float horizontalInput;
    public float verticalInput;

    private Rigidbody2D body;
    private Rect screenRect;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, bodyRadius);
        Gizmos.DrawWireCube(screenRect.center, screenRect.size);
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        body.velocity = startVelocity;
    }

    private void Update()
    {
        Camera cam = Camera.main;
        float screenHeight = cam.orthographicSize * 2f;
        screenRect.size = new(cam.aspect * screenHeight, screenHeight);
        screenRect.center = cam.transform.position;
        if (Application.isPlaying)
        {
            
            //horizontalInput = InputSystem.Current.rightNormalized - InputSystem.Current.leftNormalized;
            //verticalInput = InputSystem.Current.upNormalized - InputSystem.Current.downNormalized;
        }
    }

    private void FixedUpdate()
    {
        Vector2 velocity = body.velocity;
        if ((velocity.x > 0f && body.position.x + bodyRadius > screenRect.xMax)
            || (velocity.x < 0f && body.position.x - bodyRadius < screenRect.xMin))
        {
            velocity.x = -body.velocity.x;
        }
        if ((velocity.y > 0f && body.position.y + bodyRadius > screenRect.yMax)
            || (velocity.y < 0f && body.position.y - bodyRadius < screenRect.yMin))
        {
            velocity.y = -body.velocity.y;
        }
        body.velocity = velocity;
        body.AddForce(new Vector2(horizontalInput, verticalInput) * inputForce);
    }
}
