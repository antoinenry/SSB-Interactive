namespace Megalovania
{
    public class MegalovaniaStage : Stage
    {
        public enum Phase { BulletHell, Solo }
        public Phase currentPhase;
        public Phase[] phases;

        public override int MomentCount => phases.Length;

        private RoundManager bulletHellRounds;
        private SoloChoiceSpawner soloChoices;

        protected override void OnMomentChange(int momentValue)
        {
            base.OnMomentChange(momentValue);
            if (momentValue < MomentCount) SwitchToPhase(phases[momentValue]);
        }

        protected override bool HasAllComponents()
        {
            if (base.HasAllComponents() && bulletHellRounds && soloChoices) return true;
            bulletHellRounds = GetComponentInChildren<RoundManager>(true);
            soloChoices = GetComponentInChildren<SoloChoiceSpawner>(true);
            return base.HasAllComponents() && bulletHellRounds && soloChoices;
        }

        private void SwitchToPhase(Phase phase)
        {
            if (!HasAllComponents()) return;
            currentPhase = phase;
            switch (phase)
            {
                case Phase.BulletHell:
                    bulletHellRounds.enabled = true;
                    soloChoices.enabled = false;
                    break;
                case Phase.Solo:
                    bulletHellRounds.enabled = false;
                    soloChoices.enabled = true;
                    break;
            }
        }
    }
}
