using NPC;

namespace Prophecy
{
    public class ProphecyStage : Stage
    {
        public NPCDialog npc;

        public override int MomentCount => 2;

        protected override void OnMomentChange(int value)
        {
            base.OnMomentChange(value);
            if (npc == null) return;
            if (value == 1)
            {
                npc.gameObject.SetActive(true);
                npc.ShowDialogLine(0);
            }
            else
                npc.gameObject.SetActive(false);
        }
    }
}
