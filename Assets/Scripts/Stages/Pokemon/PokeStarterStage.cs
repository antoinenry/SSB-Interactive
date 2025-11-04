using UnityEngine;
using NPC;

namespace Pokefanf
{
    public class PokeStarterStage : Stage
    {
        [Header("Components")]
        public NPCDialog npc;
        public PokeSelectorGroup selector;
        [Header("Contents")]
        public NPCDialogContentAsset introDialog;
        public NPCDialogContentAsset outroDialog;
        public PokeConfigAsset config;
        public InventoryTracker playerInventory;
        [Header("Web")]
        public string selectionMessage = "Curseur : ";
        public string validationMessage = "VALIDE : ";

        public override int MomentCount => 3;

        protected override bool HasAllComponents()
        {
            if (base.HasAllComponents() && npc != null && selector != null) return true;
            if (npc == null) npc = GetComponentInChildren<NPCDialog>(true);
            if (selector == null) selector = GetComponentInChildren<PokeSelectorGroup>(true);
            if (playerInventory == null)
            {
                playerInventory = CurrentAssetsManager.GetCurrent<InventoryTracker>();
            }
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

        private void OnChangeSelection()
        {
            if (selector == null) return;
            MessengerAdmin.Send(selectionMessage + selector.GetLeader().musicianName);
        }

        private void OnValidateSelection(Pokefanf selected)
        {
            MessengerAdmin.Send(validationMessage + selected.musicianName);
            if (playerInventory != null)
            {
                playerInventory.SetStarter(selected.musicianName);
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
            selector.onRankingChange.AddListener(OnChangeSelection);
            selector.onSelectorMaxed.AddListener(OnValidateSelection);
        }

        private void HideSelector()
        {
            if (selector == null) return;
            selector.onRankingChange.RemoveListener(OnChangeSelection);
            selector.onSelectorMaxed.RemoveListener(OnValidateSelection);
            selector.gameObject.SetActive(false);
        }
    }
}
