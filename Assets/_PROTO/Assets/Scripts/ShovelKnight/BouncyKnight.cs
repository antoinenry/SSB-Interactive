using UnityEngine;

public class BouncyKnight : MonoBehaviour
{
    public string floorTag = "Floor";
    public float bounceForce;
    public float moveForce;
    public RectTransform scoreGUI;

    private Rigidbody2D body;
    private MiniGameScore score;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        score = GetComponent<MiniGameScore>();
    }
    private void OnEnable()
    {
        if (MiniGameConfig.Current != null)
        {
            MiniGameConfigData.ShovelKnightConfig config = MiniGameConfig.Current.Data.shovelKnight;
            bounceForce = config.bounceForce;
            moveForce = config.moveForce;
        }
    }

    private void FixedUpdate()
    {
        //if (InputSource.Current != null)
        //{
        //    body.AddForce(InputSource.Current.RightNormalized * moveForce * Vector2.right);
        //    body.AddForce(InputSource.Current.LeftNormalized * moveForce * Vector2.left);
        //}
        float horizontalInput = AudienceInputSource.Current.GetHorizontalAxis().deltaPresses;
        body.AddForce(horizontalInput * moveForce * Vector2.right);
        Camera cam = Camera.main;
        float cameraX = cam.transform.position.x;
        float cameraHalfWidth = cam.aspect * cam.orthographicSize;
        if (body.position.x > cameraX + cameraHalfWidth) body.position = new Vector2(cameraX - cameraHalfWidth, body.position.y);
        else if (body.position.x < cameraX - cameraHalfWidth) body.position = new Vector2(cameraX + cameraHalfWidth, body.position.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (body.velocity.y <= 0f && collision.gameObject.CompareTag(floorTag))
        {
            ShovelPlatform.BounceCount++;
            body.velocity = new Vector2(body.velocity.x, bounceForce);
            score.unitValue = Mathf.Max(score.unitValue, transform.position.y);
            scoreGUI.position = new Vector3(scoreGUI.position.x, score.unitValue, 0f);
        }
    }
}
