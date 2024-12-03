using UnityEngine;

public class Ghost : MonoBehaviour
{
    public enum Action { Idle, Stalk, Retreat, Teleport }

    public Action currentAction;
    public float stalkSpeed;
    public float retreatSpeed;
    public float retreatDuration;
    public float idleDuration;
    public Vector2 speedScale;
    public Vector2 targetOffset;
    [Range(0f, 1f)] public float teleportChance;
    public float teleportRadius;
    public float floatingAmplitude;
    public float floatingRate;
    public string lightTag = "Weapon";

    private Rigidbody2D body;
    private Transform target;
    private HauntedHouse house;
    private float actionTimer;
    private HouseStage currentFloor;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        house = FindObjectOfType<HauntedHouse>();
    }

    private void OnEnable()
    {
        Luigi findLuigi = FindObjectOfType<Luigi>();
        if (findLuigi != null) target = findLuigi.transform;
    }

    private void FixedUpdate()
    {
        currentFloor = house.GetFloor(body.position);
        switch (currentAction)
        {
            case Action.Idle: StayIdle();
                break;
            case Action.Stalk: Stalk();
                break;
            case Action.Retreat: Retreat();
                break;
            case Action.Teleport: Teleport();
                break;
        }
        body.velocity += Mathf.Sin(Time.fixedTime * floatingRate * Mathf.PI * 2f) * floatingAmplitude * Vector2.up;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(lightTag) && currentFloor == house.GetFloor(collision.transform.position))
        {
            actionTimer = 0f;
            currentAction = Action.Retreat;
        }
    }

    public void StayIdle()
    {
        if (actionTimer < idleDuration)
        {
            body.velocity = Vector2.zero;
            actionTimer += Time.fixedDeltaTime;
        }
        else
        {
            actionTimer = 0f;
            currentAction = Action.Stalk;
        }
    }

    private void Stalk()
    {
        if (Vector2.Distance(body.position, (Vector2)target.position + targetOffset) < teleportRadius)
            body.velocity = speedScale * stalkSpeed * ((Vector2)target.position + targetOffset - (Vector2)transform.position).normalized;
        else
        {
            actionTimer = 0f;
            currentAction = Action.Teleport;
        }
    }

    private void Retreat()
    {
        if (actionTimer < retreatDuration)
        {
            actionTimer += Time.fixedDeltaTime;
            body.velocity = retreatSpeed * (transform.position - target.position).normalized;
        }
        else
        {
            actionTimer = 0f;
            if (teleportChance > 0f && Random.Range(0f, 1f) <= teleportChance)
                currentAction = Action.Teleport;
            else
                currentAction = Action.Idle;
        }
    }

    public void Teleport()
    {
        float randomAngle = Random.Range(0f, 2F * Mathf.PI);
        Vector3 randomPosition = target.position + teleportRadius * new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
        body.position = randomPosition;
        actionTimer = 0f;
        currentAction = Action.Idle;
    }
}
