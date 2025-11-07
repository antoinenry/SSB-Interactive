using TMPro;

namespace Megalovania
{
    public class MegalovaniaStage : Stage
    {
        public MegalovaniaPhase[] phases;
        public GUIAnimatedText roundsField;
        public string roundsFieldPrefix = "round ";
        public int roundCounter;
        public string soloText = "SOLO !";

        public override int MomentCount => phases.Length;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (roundsField) roundsField.text = "";
            if (phases != null)
                foreach (MegalovaniaPhase phase in phases)
                    if (phase != null && phase is RoundManager)
                        (phase as RoundManager).onFinishRound.AddListener(OnFinishRound);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (phases != null)
                foreach (MegalovaniaPhase phase in phases)
                    if (phase != null && phase is RoundManager)
                        (phase as RoundManager).onFinishRound.RemoveListener(OnFinishRound);
        }

        protected override void OnMomentChange(int momentValue)
        {
            base.OnMomentChange(momentValue);
            if (momentValue >= 0 && momentValue < MomentCount) SwitchToPhase(momentValue);
        }

        private void OnFinishRound()
        {
            roundCounter++;
            if (roundsField) roundsField.text = roundsFieldPrefix + roundCounter;
        }

        private void SwitchToPhase(int phaseIndex)
        {
            for (int i = 0, iend = phases != null ? phases.Length : 0; i < iend; i++)
            {
                if (phases[i] == null) continue;
                else if (i == phaseIndex)
                {
                    phases[i].SetActive(true);
                    if (phases[i] is RoundManager)
                    {
                        roundCounter++;
                        if (roundsField) roundsField.text = roundsFieldPrefix + roundCounter;
                    }
                    else if (phases[i] is SoloChoiceSpawner)
                    {
                        if (roundsField) roundsField.text = soloText;
                    }
                    else
                    {
                        if (roundsField) roundsField.text = "";
                    }
                }
                else
                {
                    phases[i].SetActive(false);
                }
            }
        }
    }
}
