using System;
using System.Collections.Generic;
using UnityEngine;

public class HauntedHouse : MonoBehaviour
{
    [Serializable]
    public struct Difficulty
    {
        public int floorCount;
        public float minFloorWidth;
        public float maxFloorWidth;
        public int ghostCount;
        public float ghostSpeed;
        public int minCoinsPerFloor;
        public int maxCoinsPerFloor;
    }

    public int currentFloor;
    public HouseStage stagePrefab;
    [Range(0f, 1f)] public float minStairDistance;
    public int fullDarkSortingOrder;
    public int semiDarkSortingOrder;
    public Ghost ghostPrefab;
    public Difficulty currentDifficulty;
    public Difficulty[] difficultyCurve;

    private MiniGameScore score;
    private List<HouseStage> stages;
    private List<Ghost> ghosts;

    private void Awake()
    {
        score = GetComponent<MiniGameScore>();
        stages = new(GetComponentsInChildren<HouseStage>(true));
        ghosts = new(GetComponentsInChildren<Ghost>(true));
        
    }

    private void OnEnable()
    {
        if (MiniGameConfig.Current != null)
        {
            MiniGameConfigData.LuigisMansionConfig config = MiniGameConfig.Current.Data.luigisMansion;
            difficultyCurve = config.difficulty;
        }
    }

    private void Update()
    {
        for (int f = 0, floorCount = stages.Count; f <= currentFloor || f < floorCount; f++)
        {
            if (f < stages.Count)
            {
                if (f == currentFloor) stages[f].SetDarkness(semiDarkSortingOrder);
                else stages[f].SetDarkness(fullDarkSortingOrder);
            }
            else
            {
                AddFloor();
            }
        }
        score.unitValue = currentFloor;
    }

    private void AddFloor()
    {
        UpdateDifficulty();
        HouseStage lastFloor = stages[stages.Count - 1];
        HouseStage newFloor = Instantiate(stagePrefab, transform, false);
        float width = UnityEngine.Random.Range(currentDifficulty.minFloorWidth, currentDifficulty.maxFloorWidth);
        newFloor.SetWidth(width);
        float lastStairPos = lastFloor.stairs.transform.position.x;
        float stairWidth = lastFloor.stairs.Size.x;
        Vector2 pos = new Vector2()
        {
            x = lastStairPos + UnityEngine.Random.Range(-width / 2f + stairWidth, width / 2f - stairWidth),
            y = lastFloor.transform.position.y + lastFloor.size.y
        };
        newFloor.transform.position = pos;
        float lastStairPosRelative = .5f + (lastStairPos - pos.x ) / width;
        float stairDistance = UnityEngine.Random.Range(minStairDistance, 1f);
        if (lastStairPosRelative + stairDistance > 1f) newFloor.PlaceStairs(lastStairPosRelative - stairDistance);
        else if (lastStairPosRelative - stairDistance < 0f) newFloor.PlaceStairs(lastStairPosRelative + stairDistance);
        else
        {
            if (UnityEngine.Random.Range(0, 2) == 0) newFloor.PlaceStairs(lastStairPosRelative + stairDistance);
            else newFloor.PlaceStairs(lastStairPosRelative - stairDistance);
        }
        newFloor.PlaceCoins(UnityEngine.Random.Range(currentDifficulty.minCoinsPerFloor, currentDifficulty.maxCoinsPerFloor));
        stages.Add(newFloor);
    }

    public int GetFloorIndex(float y)
    {
        return Mathf.RoundToInt((y - transform.position.y) / stagePrefab.size.y);
    }

    public HouseStage GetFloor(int index)
    {
        if (index >= 0 && index < stages.Count) return stages[index];
        else return null;

    }

    public HouseStage GetFloor(float y)
    {
        int index = GetFloorIndex(y);
        return GetFloor(index);
}

    public HouseStage GetFloor(Vector2 position)
    {
        int index = GetFloorIndex(position.y);
        HouseStage stage = GetFloor(index);
        if (stage != null 
            && position.x > stage.transform.position.x - stage.size.x / 2f 
            && position.x < stage.transform.position.x + stage.size.x / 2f)
        {
            return stage;
        }
        return null;
    }

    public Vector2 GetPositionOnFloor(int stageIndex)
    {
        return stages[stageIndex].transform.position + .5f * stages[stageIndex].size.y * Vector3.down;
    }

    private void UpdateDifficulty()
    {
        if (difficultyCurve != null)
        {
            int difficultyIndex = Mathf.FloorToInt((float)(currentFloor - 1) / currentDifficulty.floorCount);
            if (difficultyIndex >= 0 && difficultyIndex < difficultyCurve.Length)
            {
                currentDifficulty = difficultyCurve[difficultyIndex];
            }
        }
        for (int i = ghosts.Count; i < currentDifficulty.ghostCount; i++)
        {
            Ghost newGhost = Instantiate(ghostPrefab, transform, true);
            newGhost.currentAction = Ghost.Action.Teleport;
            newGhost.stalkSpeed = currentDifficulty.ghostSpeed;
            ghosts.Add(newGhost);
        }
        for (int i = currentDifficulty.ghostCount; i < ghosts.Count; i++)
        {
            Destroy(ghosts[i].gameObject);
        }
        ghosts.RemoveAll(g => g == null);
    }
}
