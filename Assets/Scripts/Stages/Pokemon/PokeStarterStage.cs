using Shop;
using UnityEngine;

namespace Pokefanf
{
    public class PokeStarterStage : Stage
    {
        [Header("Components")]
        public NPCDialog npc;
        public PokeSelectorGroup selector;
        [Header("Contents")]
        public NPCDialogAsset introDialog;
        public NPCDialogAsset outroDialog;
        public PokeConfigAsset config;

        public override int MomentCount => 3;

        protected override bool HasAllComponents()
        {
            if (base.HasAllComponents() && npc != null && selector != null) return true;
            if (npc == null) npc = GetComponentInChildren<NPCDialog>(true);
            if (selector == null) selector = GetComponentInChildren<PokeSelectorGroup>(true);
            return (base.HasAllComponents() && npc != null && selector != null);
        }

        protected override void OnMomentChange(int value)
        {
            base.OnMomentChange(value);
            base.OnMomentChange(value);
            switch (value)
            {
                case 0: ShowIntroDialog(); break;
                case 1: ShowSelector(); break;
                case 2: ShowOutroDialog(); break;
            }
        }

        private void ShowIntroDialog()
        {
            HideSelector();
            if (npc != null) npc.ShowDialogLine(introDialog, 0);
        }

        private void ShowOutroDialog()
        {
            HideSelector();
            if (npc != null) npc.ShowDialogLine(outroDialog, 0);
        }

        private void HideDialog()
        {
            if (npc != null) npc.EndDialog();
        }

        private void ShowSelector()
        {
            HideDialog();
            if (selector == null) return;
            if (config != null) selector.config = config.Data;
            selector.gameObject.SetActive(true);
        }

        private void HideSelector()
        {
            if (selector == null) return;
            selector.gameObject.SetActive(false);
        }
    }
}
