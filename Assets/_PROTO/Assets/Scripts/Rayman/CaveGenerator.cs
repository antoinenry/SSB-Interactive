using System;
using System.Collections.Generic;
using UnityEngine;

public class CaveGenerator : ScrollerPathGenerator
{
    [Serializable]
    public struct DifficultySetting
    {
        public float minimumPassageHeight;
        public float maximumPassageHeight;
        [Range(0f, 1f)] public float stalagtiteChance;
        [Range(0f, 1f)] public float stalagmiteChance;
        [Range(0f, 1f)] public float itemChance;
    }

    public MeshRenderer floor;
    public MeshRenderer ceiling;
    public float floorOffset;
    public float ceilingOffset;
    public float floorScrollSpeed;
    [Header("Obstacles")]
    public CaveObstacle obstaclePrefab;
    public DifficultySetting currentDifficulty;
    public float difficultyStep = 20f;
    public DifficultySetting[] difficultyCurve;

    private List<CaveObstacle> obstacles;

    protected override void Update()
    {
        vertical = false;
        base.Update();
        UpdateBorders();
    }

    private void OnEnable()
    {
        if (MiniGameConfig.Current != null)
        {
            MiniGameConfigData.RaymanConfig config = MiniGameConfig.Current.Data.rayman;
            difficultyStep = config.difficultyStepLength;
            difficultyCurve = config.difficulty;
        }
    }

    protected override void OnPathChange()
    {
        if (Application.isPlaying)
        {
            UpdateDifficulty();
            UpdateObstacles();
        }
    }

    private void UpdateObstacles()
    {
        if (obstacles == null) obstacles = new List<CaveObstacle>();
        if (Path == null) return;
        List<Vector2> newPositions = new(Path);
        foreach(CaveObstacle ob in obstacles)
        {
            if (ob == null) continue;
            Vector2 pos = ob.transform.localPosition;
            if (Array.IndexOf(Path, pos) == -1)
                Destroy(ob.gameObject);
            else
                newPositions.Remove(pos);
        }
        obstacles.RemoveAll(ob => ob == null);
        if (obstaclePrefab != null)
        {
            foreach (Vector2 pt in newPositions)
            {
                CaveObstacle newObstacle = Instantiate(obstaclePrefab, transform, false);
                newObstacle.transform.localPosition = pt;
                newObstacle.Randomize(currentDifficulty.minimumPassageHeight, currentDifficulty.maximumPassageHeight, currentDifficulty.stalagtiteChance, currentDifficulty.stalagmiteChance, currentDifficulty.itemChance);
                obstacles.Add(newObstacle);
            }
        }
    }

    private void UpdateBorders()
    {
        floor.transform.localPosition = Center + floorOffset * Vector2.up;
        if (Application.isPlaying) floor.material.mainTextureOffset = new Vector2(Center.x * floorScrollSpeed, 0f);
        ceiling.transform.localPosition = Center + ceilingOffset * Vector2.up;
        if (Application.isPlaying) ceiling.material.mainTextureOffset = new Vector2(Center.x * floorScrollSpeed, 0f);
    }

    private void UpdateDifficulty()
    {
        if (difficultyCurve != null && difficultyStep != 0f)
        {
            int difficultyIndex = Mathf.FloorToInt(Center.x / difficultyStep);
            if (difficultyIndex < 0) difficultyIndex = -difficultyIndex;
            if (difficultyIndex < difficultyCurve.Length)
            {
                currentDifficulty = difficultyCurve[difficultyIndex];
            }
        }
    }

    public float GetLastFreeSpot(float max)
    {
        int obstacleCount = obstacles != null ? obstacles.Count : 0;
        if (obstacleCount == 0) return float.NaN;
        if (obstacles.Count == 1) return obstacles[0].transform.position.x;
        List<float> obstacleXPositions = obstacles.ConvertAll(o => o != null && o.transform.position.x <= max ? o.transform.position.x : float.NegativeInfinity);
        obstacleXPositions.Sort();
        return (obstacleXPositions[obstacleCount - 2] + obstacleXPositions[obstacleCount - 1]) / 2f;
    }
}
