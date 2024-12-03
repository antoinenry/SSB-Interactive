using System;

public class FloralFuryStage : Stage
{
    [Serializable]
    public struct Phase
    {
        public enum Type { MiniGame, SoloChoice, Solo }

        public Type type;
        public float[] dropChances;
    }

    public Phase currentPhase;
    public Phase[] phases;

    public override int MomentCount => phases.Length;

    private Cuphead cup;
    private ChoicePanel choices;
    private ObjectSpawner spawner;
    private Messenger messenger;

    protected override void OnMomentChange(int momentValue)
    {
        base.OnMomentChange(momentValue);
        if (momentValue < MomentCount) SwitchToPhase(phases[momentValue]);
    }

    protected override bool HasAllComponents()
    {        
        if (base.HasAllComponents() && cup && choices && spawner && messenger) return true;
        cup = FindObjectOfType<Cuphead>(true);
        choices = FindObjectOfType<ChoicePanel>(true);
        spawner = FindObjectOfType<ObjectSpawner>(true);
        messenger = FindObjectOfType<Messenger>(true);
        return base.HasAllComponents() && cup && choices && spawner && messenger;
    }

    private void SwitchToPhase(Phase phase)
    {
        if (!HasAllComponents()) return;
        currentPhase = phase;
        switch (phase.type)
        {
            case Phase.Type.MiniGame:
                choices.enabled = false;
                cup.enabled = true;
                spawner.enabled = true;
                break;
            case Phase.Type.SoloChoice:
                if (choices.enabled) choices.UnlockChoice();
                else choices.enabled = true;
                cup.enabled = false;
                spawner.enabled = false;
                break;
            case Phase.Type.Solo:
                choices.LockChoice();
                cup.enabled = true;
                spawner.enabled = true;
                messenger.Send("Go solo de " + choices.SelectedChoice);
                break;
        }
        for (int i = 0, iend = currentPhase.dropChances.Length; i < iend; i++)
        {
            if (i >= spawner.objects.Length) break;
            else
            {
                ObjectSpawner.ObjectChance o = spawner.objects[i];
                o.chance = currentPhase.dropChances[i];
                spawner.objects[i] = o;
            }
        }
    }
}