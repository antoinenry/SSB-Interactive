using System;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlatformsGenerator : ScrollerPathGenerator
{
    [Serializable]
    public struct DifficultySetting
    {
        public float minimumPlatformWidth;
        public float maximumPlatformWidth;
        [Range(0f, 1f)] public float movingChance;
        public float movingSpeed;
        [Range(0f, 1f)] public float blinkingChance;
        public int blinkingRate;
        [Range(0f, 1f)] public float itemChance;
    }

    [Header("Platforms")]
    public ShovelPlatform platformPrefab;
    public DifficultySetting currentDifficulty;
    public float difficultyStep = 20f;
    public DifficultySetting[] difficultyCurve;

    private List<ShovelPlatform> platforms;

    private void OnEnable()
    {
        if (MiniGameConfig.Current != null)
        {
            MiniGameConfigData.ShovelKnightConfig config = MiniGameConfig.Current.Data.shovelKnight;
            difficultyStep = config.difficultyStepHeight;
            difficultyCurve = config.difficulty;
        }
    }

    protected override void Update()
    {
        vertical = true;
        base.Update();
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
        if (platforms == null) platforms = new List<ShovelPlatform>();
        if (Path == null) return;
        List<Vector2> newPositions = new(Path);
        foreach (ShovelPlatform p in platforms)
        {
            if (p == null) continue;
            Vector2 pos = p.transform.localPosition;
            if (Array.IndexOf(Path, pos) == -1)
                Destroy(p.gameObject);
            else
                newPositions.Remove(pos);
        }
        platforms.RemoveAll(ob => ob == null);
        if (platformPrefab != null)
        {
            foreach (Vector2 pt in newPositions)
            {
                ShovelPlatform newPlatform = Instantiate(platformPrefab, transform, false);
                newPlatform.transform.localPosition = pt;
                float width = UnityEngine.Random.Range(currentDifficulty.minimumPlatformWidth, currentDifficulty.maximumPlatformWidth);
                newPlatform.SetWidth(width);
                if (currentDifficulty.movingChance > 0f && UnityEngine.Random.Range(0f, 1f) <= currentDifficulty.movingChance)
                    newPlatform.movingSpeed = UnityEngine.Random.Range(0, 2) == 1 ? currentDifficulty.movingSpeed : -currentDifficulty.movingSpeed; ;
                if (currentDifficulty.blinkingChance > 0f && UnityEngine.Random.Range(0f, 1f) <= currentDifficulty.blinkingChance)
                {
                    newPlatform.blinkEveryBounce = currentDifficulty.blinkingRate;
                    newPlatform.blinkOffset = PathLength;
                }
                bool item = currentDifficulty.itemChance > 0f && UnityEngine.Random.Range(0f, 1f) <= currentDifficulty.itemChance;
                newPlatform.EnableItem(item);
                platforms.Add(newPlatform);
            }
        }
    }

    private void UpdateDifficulty()
    {
        if (difficultyCurve != null && difficultyStep != 0f)
        {
            int difficultyIndex = Mathf.FloorToInt(Center.y / difficultyStep);
            if (difficultyIndex < 0) difficultyIndex = -difficultyIndex;
            if (difficultyIndex < difficultyCurve.Length)
            {
                currentDifficulty = difficultyCurve[difficultyIndex];
            }
        }
    }
}
