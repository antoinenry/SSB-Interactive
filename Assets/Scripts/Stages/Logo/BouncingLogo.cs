using UnityEngine;

[ExecuteAlways]
public class BouncingLogo : MonoBehaviour
{
    public float bodyRadius;
    public float inputForce;
    public Vector2 startVelocity;
    public float minimumSpeed;
    public float noInputDrag;

    private Rigidbody2D body;
    private Rect screenRect;
    private float horizontalInput;
    private float verticalInput;

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
            horizontalInput = AudienceInputSource.Current.GetHorizontalAxis().deltaPresses;
            if (horizontalInput != 0f) horizontalInput = Mathf.Sign(horizontalInput);
            verticalInput = AudienceInputSource.Current.GetVerticalAxis().deltaPresses;
            if (verticalInput != 0f) verticalInput = Mathf.Sign(verticalInput);
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
        Vector2 movementForce = new Vector2(horizontalInput, verticalInput) * inputForce;
        body.drag = (movementForce == Vector2.zero && velocity.magnitude > minimumSpeed) ? noInputDrag : 0f;
        body.AddForce(movementForce);
    }
}
