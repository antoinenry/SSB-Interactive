using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PropTower : MonoBehaviour
{
    [Serializable]
    public struct DifficultySetting
    {
        public float floorHeight;
        public float floorWidth;
        public int[] propChances;
        [Range(0f, 1f)] public float itemChance;
    }

    public Rigidbody2D currentFloor;
    public ColliderUtility nextFloor;
    public List<Rigidbody2D> props;
    public Coin coinPrefab;
    public float stableWaitMin;
    public float stableWaitMax;
    public float stableVelocity;
    public float yDestroy;
    public float rumbleDuration;
    public float rumbleAmplitude;
    public float rumbleFrequency;
    public float lockTowerDelay;
    public int currentFloorIndex;
    public DifficultySetting currentDifficulty;
    public DifficultySetting[] difficulties;

    private PropDrop propDrop;
    private float stableTimerMin;
    private float stableTimerMax;
    private float towerLockTimer;
    private Vector2 currentFloorPosition;
    private float currentTowerHeight;
    private MiniGameScore score;

    public UnityEvent onPassFloor;

    private void Awake()
    {
        propDrop = FindObjectOfType<PropDrop>();
        score = FindObjectOfType<MiniGameScore>();
    }

    private void OnEnable()
    {
        propDrop.onDropProp.AddListener(AddProp);
        if (MiniGameConfig.Current != null)
        {
            MiniGameConfigData.DonkeyKongConfig config = MiniGameConfig.Current.Data.donkeyKong;
            difficulties = config.difficulty;
        }
    }

    private void OnDisable()
    {
        propDrop.onDropProp.RemoveListener(AddProp);
    }

    private void Start()
    {
        UpdateDifficulty();
        currentFloorPosition = currentFloor.position;
        nextFloor.transform.position = currentFloorPosition + currentDifficulty.floorHeight * Vector2.up;
        SetFloorWidth(nextFloor.transform, currentDifficulty.floorWidth);
    }

    private void FixedUpdate()
    {
        float dropHeight = Mathf.Max(currentTowerHeight, currentFloorPosition.y);
        propDrop.MoveSpawnHeight(dropHeight);
        if (towerLockTimer == 0f)
        {
            if (TowerIsStable())
            {
                if (TryLockTower() == false)
                    propDrop.enabled = true;
            }
            else
            {
                currentTowerHeight = GetTowerHeight();
                score.unitValue = currentTowerHeight;
                propDrop.enabled = false;
            }
            if (DestroyFallenProps() > 0)
            {
                stableTimerMin = 0f;
                stableTimerMax = 0f;
                StartCoroutine(RumbleCoroutine());
            }
        }
        else
        {
            propDrop.enabled = false;
            TryLockTower();
        }
    }

    private void AddProp(Rigidbody2D newProp)
    {
        if (newProp != null)
        {
            if (props == null) props = new List<Rigidbody2D>() { newProp };
            else props.Add(newProp);
            stableTimerMin = 0f;
            stableTimerMax = 0f;
            propDrop.enabled = false;
        }
    }

    private float GetTowerHeight()
    {
        float h = 0;
        if (props != null)
        {
            props.RemoveAll(p => p == null);
            foreach (Rigidbody2D prop in props)
            {
                if (prop.position.y > h) h = prop.position.y;
            }
        }
        return Mathf.Max(h, currentFloor.position.y);
    }

    private int DestroyFallenProps()
    {
        int destroyed = 0;
        if (props != null)
        {
            foreach (Rigidbody2D prop in props)
            {
                if (prop != null && prop.position.y < currentFloor.position.y + yDestroy)
                {
                    Destroy(prop.gameObject);
                    stableTimerMin = 0f;
                    stableTimerMax = 0f;
                    destroyed++;
                }
            }
        }
        return destroyed;
    }

    private bool TowerIsStable()
    {
        if (props == null || props.Count == 0
            || props.FindIndex(prop => prop != null && prop.velocity.magnitude > stableVelocity) == -1)
        {
            if (stableTimerMin >= stableWaitMin || stableTimerMax >= stableWaitMax) return true;
            else
            {
                float deltaTime = Time.fixedDeltaTime;
                stableTimerMin += deltaTime;
                stableTimerMax += deltaTime;
            }
        }
        else
        {
            stableTimerMin = 0f;
        }
        return false;
    }

    private bool TryLockTower()
    {
        if (nextFloor.triggerEnterCount == 0)
        {
            towerLockTimer = 0f;
            return false;
        }
        else
        {
            towerLockTimer += Time.fixedDeltaTime;
            if (towerLockTimer < lockTowerDelay)
            {
                return false;
            }
            else
            {
                towerLockTimer = 0f;
                currentFloorIndex++;
                onPassFloor.Invoke();
                //UpdateDifficulty();
                StartCoroutine(LockTowerCoroutine());
                return true;
            }
        }
    }

    private void UpdateDifficulty()
    {
        if (difficulties != null)
        {
            if (currentFloorIndex >= 0 && currentFloorIndex < difficulties.Length)
            {
                currentDifficulty = difficulties[currentFloorIndex];
                propDrop.chances = currentDifficulty.propChances;
            }
        }
    }

    private IEnumerator RumbleCoroutine()
    {
        float timer = 0f;
        while (timer < rumbleDuration)
        {
            Vector3 randomMove = rumbleAmplitude * Vector3.up;
            randomMove = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.forward) * randomMove;
            currentFloor.MovePosition(currentFloorPosition + (Vector2)randomMove);
            timer += Time.fixedDeltaTime;
            yield return new WaitForSeconds(1f / rumbleFrequency);
            yield return new WaitForFixedUpdate();
        }
        currentFloor.MovePosition(currentFloorPosition);
    }

    private IEnumerator LockTowerCoroutine()
    {
        foreach (Rigidbody2D prop in props)
        {
            if (prop != null)
            {
                prop.bodyType = RigidbodyType2D.Static;
            }
        }
        props.Clear();
        towerLockTimer = 0f;
        yield return new WaitForFixedUpdate();
        Rigidbody2D newFloor = Instantiate(currentFloor, transform, false);
        currentFloor.bodyType = RigidbodyType2D.Static;
        newFloor.position = nextFloor.transform.position;
        SetFloorWidth(newFloor.transform, currentDifficulty.floorWidth);
        currentFloor = newFloor;
        currentFloorPosition = currentFloor.position;
        RandomAddItem();
        UpdateDifficulty();
        nextFloor.transform.position = currentFloor.position + currentDifficulty.floorHeight * Vector2.up;
        SetFloorWidth(nextFloor.transform, currentDifficulty.floorWidth);
    }

    private void SetFloorWidth(Transform floor, float width)
    {
        BoxCollider2D floorCollider = floor.GetComponent<BoxCollider2D>();
        Vector2 colliderSize = floorCollider.size;
        colliderSize.x = width;
        floorCollider.size = colliderSize;
        SpriteRenderer floorSprite = floor.GetComponent<SpriteRenderer>();
        Vector2 spriteSize = floorSprite.size;
        spriteSize.x = width;
        floorSprite.size = spriteSize;
    }

    private void RandomAddItem()
    {
        if (currentDifficulty.itemChance == 0f || UnityEngine.Random.Range(0f, 1f) > currentDifficulty.itemChance) return;
        Coin item = Instantiate(coinPrefab, transform);
        item.transform.position = new Vector2(
            currentFloor.position.x + currentDifficulty.floorWidth * UnityEngine.Random.Range(-.5f, .5f),
            currentFloor.position.y + .5f * currentDifficulty.floorHeight);
    }
}
