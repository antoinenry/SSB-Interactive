using System;

namespace Sonic
{
    [Serializable]
    public struct StageConfig
    {
        public float walkSpeed;
        public float invicibleWalkSpeed;
        public float verticalStep;
        public float difficultyStepLength;
        public RoadGenerator.DifficultySetting[] difficulty;
    }
}