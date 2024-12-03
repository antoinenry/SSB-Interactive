using UnityEngine;

namespace Sonic
{
    public class SonicStage : Stage
    {
        public int invincibleMoment = 1;

        private Hedgehog sonic;

        public override int MomentCount => 3;

        protected override void Awake()
        {
            base.Awake();
            sonic = GetComponentInChildren<Hedgehog>(true);
        }

        protected override void OnMomentChange(int value)
        {
            base.OnMomentChange(value);
            if (sonic != null) sonic.invincible = (value == invincibleMoment);
        }
    }
}
