using System;

[Serializable]
public struct MiniGameConfigData
{
    public DonkeyKongConfig donkeyKong;
    public LuigisMansionConfig luigisMansion;
    public RaymanConfig rayman;
    public ShovelKnightConfig shovelKnight;
    public Sonic.StageConfig sonic;

    [Serializable]
    public struct DonkeyKongConfig
    {
        public float horizontalSpeed;
        public float rotateSpeed;
        public PropTower.DifficultySetting[] difficulty;
    }

    [Serializable]
    public struct LuigisMansionConfig
    {
        public float luigiSpeed;
        public float torchSpeed;
        public int fallFloors;
        public HauntedHouse.Difficulty[] difficulty;
    }

    [Serializable]
    public struct RaymanConfig
    {
        public float horizontalSpeed;
        public float jumpForce;
        public float flyForce;
        public float fallGravity;
        public float glideGravity;
        public float difficultyStepLength;
        public CaveGenerator.DifficultySetting[] difficulty;
    }

    [Serializable]
    public struct ShovelKnightConfig
    {
        public float bounceForce;
        public float moveForce;
        public float difficultyStepHeight;
        public VerticalPlatformsGenerator.DifficultySetting[] difficulty;
    }    
}