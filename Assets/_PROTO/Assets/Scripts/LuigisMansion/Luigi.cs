using System.Collections;
using UnityEngine;

public class Luigi : MonoBehaviour
{
    public float walkSpeed;
    public float walkDuration;
    public Transform torch;
    public float torchRotationSpeed;
    [Range(0f, 90f)] public float torchAngleMax;
    [Range(0f, 90f)] public float lookUpAngleMin;
    public SpriteRenderer spriteRenderer;
    public Sprite idleSprite;
    public Sprite[] walkSprites;
    public Sprite fallSprite;
    public float animationRate;
    public int floorFall;
    public float fallDuration;

    private Rigidbody2D body;
    private HauntedHouse house;
    private LuigiHitBox hitBox;
    private float torchAngle;
    private int walkingDirection;
    private bool falling;

    public int CurrentStageIndex { get; private set; }
    public HouseStage CurrentStage { get; private set; }
    public bool LookingUp => torchAngle > lookUpAngleMin;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        house = FindObjectOfType<HauntedHouse>();
        hitBox = GetComponentInChildren<LuigiHitBox>();
    }

    private void OnEnable()
    {
        if (MiniGameConfig.Current != null)
        {
            MiniGameConfigData.LuigisMansionConfig config = MiniGameConfig.Current.Data.luigisMansion;
            walkSpeed = config.luigiSpeed;
            torchRotationSpeed = config.torchSpeed;
            floorFall = config.fallFloors;
        }
        hitBox.onHit.AddListener(OnHit);
    }

    private void OnDisable()
    {
        hitBox.onHit.RemoveListener(OnHit);
    }

    private void Update()
    {
        if (falling)
        {
            spriteRenderer.sprite = fallSprite;
        }
        else if (walkingDirection != 0)
        {
            int animationIndex = Mathf.FloorToInt(Time.time * animationRate * walkSpeed) % walkSprites.Length;
            spriteRenderer.sprite = walkSprites[animationIndex];
        }
        else
        {
            spriteRenderer.sprite = idleSprite;
        }
    }

    private void FixedUpdate()
    {
        if (falling) return;

        CurrentStageIndex = house.GetFloorIndex(torch.position.y);
        house.currentFloor = CurrentStageIndex;
        CurrentStage = house.GetFloor(CurrentStageIndex);

        float horizontalInput = AudienceInputSource.Current.GetHorizontalAxis().deltaPresses;
        if (horizontalInput == 0f)
        {
            walkingDirection = 0;
            //walkTimer += Time.fixedDeltaTime;
            //if (walkTimer > walkDuration) walkingDirection = 0;
        }
        else if (horizontalInput > 0f)
        {
            walkingDirection = 1;
            //walkTimer = 0f;
        }
        else
        {
            walkingDirection = -1;
            //walkTimer = 0f;
        }

        if (walkingDirection > 0
            && (CurrentStage == null || body.position.x < CurrentStage.transform.position.x + CurrentStage.size.x / 2f))
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            body.velocity = walkSpeed * Vector2.right;
        }
        else if (walkingDirection < 0
            && (CurrentStage == null || body.position.x > CurrentStage.transform.position.x - CurrentStage.size.x / 2f))
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            body.velocity = walkSpeed * Vector2.left;
        }
        else
        {
            body.velocity = Vector2.zero;
        }
        torchAngle = torch.localRotation.eulerAngles.z - 90f;

        float verticalInput = AudienceInputSource.Current.GetVerticalAxis().deltaPresses;
        if (torchAngle < torchAngleMax && verticalInput > 0f) torch.localRotation *= Quaternion.AngleAxis(torchRotationSpeed * Time.fixedDeltaTime, Vector3.forward);
        else if (torchAngle > -torchAngleMax && verticalInput < 0f) torch.localRotation *= Quaternion.AngleAxis(-torchRotationSpeed * Time.fixedDeltaTime, Vector3.forward);
    }

    private void OnHit()
    {
        if (falling == false) StartCoroutine(FallCoroutine());
    }

    private IEnumerator FallCoroutine()
    {
        walkingDirection = 0;
        falling = true;
        body.simulated = false;
        yield return new WaitForSeconds(fallDuration);
        CurrentStageIndex = Mathf.Max(0, CurrentStageIndex - floorFall);
        transform.position = house.GetPositionOnFloor(CurrentStageIndex);
        yield return new WaitForFixedUpdate();
        body.simulated = true;
        falling = false;
    }
}
