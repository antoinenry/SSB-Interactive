using UnityEngine;
using NPC;

namespace Shop
{
    public class ShopStage : Stage
    {
        public NPCDialogContentAsset[] introDialogs;
        public NPCDialogContentAsset purchaseDialog;
        public NPCDialogContentAsset outroDialog;

        private Shop shop;
        private NPCDialog npc;

        public override int MomentCount => 4;

        protected override bool HasAllComponents()
        {
            if (base.HasAllComponents() && shop != null && npc != null) return true;
            if (shop == null) shop = GetComponentInChildren<Shop>(true);
            if (npc == null) npc = GetComponentInChildren<NPCDialog>(true);
            return base.HasAllComponents() && shop != null && npc != null;
        }

        protected override void OnMomentChange(int value)
        {
            base.OnMomentChange(value);
            switch (value)
            {
                case 0: HideDialog(); break;
                case 1: ShowIntroDialog(); break;
                case 2: ShowShop(); break;
                case 3: ShowOutroDialog(); break;
            }
        }

        private void ShowIntroDialog()
        {
            HideShop();
            if (npc != null) npc.ShowDialogLine(GetCurrentIntroDialogAsset(), 0);
        }

        private void ShowShop()
        {
            HideDialog();
            if (shop == null) return;            
            shop.Open();
            shop.onBuyItem.AddListener(OnBuyItem);
        }

        private void OnBuyItem(ShopItem item)
        {
            if (shop == null) return;
            ShowPurchaseDialog();
        }

        private void ShowPurchaseDialog()
        {
            HideShop();
            if (npc == null) return;
            npc.dialog = purchaseDialog;
            npc.ShowRandomLine();
            npc.onPressNext.AddListener(OnPurchaseDialogEnd);
        }

        private void HideShop()
        {
            if (shop == null) return;
            shop.Close();
            shop.onBuyItem.RemoveListener(OnBuyItem);
        }

        private void OnPurchaseDialogEnd()
        {
            HideDialog();
            if (npc != null) npc.onDialogEnd.RemoveListener(OnPurchaseDialogEnd);
            if (shop.CartIsFull) HideShop();
            else ShowShop();
        }

        private void ShowOutroDialog()
        {
            HideShop();
            if (npc != null) npc.ShowDialogLine(outroDialog, 0);
        }

        private void HideDialog()
        {
            if (npc != null) npc.EndDialog();
        }

        private NPCDialogContentAsset GetCurrentIntroDialogAsset()
        {
            int dialogOptions = introDialogs != null ? introDialogs.Length : 0;
            if (dialogOptions == 0) return null;
            // Count previous shop song occurences in setlist
            int shopIteration = 0;
            SetlistInfo currentSetlist = ConcertAdmin.Current.state.setlist;
            SongInfo currentSong = ConcertAdmin.Current.state.song;
            int currentSongPosition = ConcertAdmin.Current.state.songPosition;
            for (int i = 0; i < currentSongPosition && i < currentSetlist.Length; i++)
                if (currentSetlist.GetSong(i) == currentSong) shopIteration++;
            // Get corresponding dialog (clamped index)
            return introDialogs[Mathf.Clamp(shopIteration, 0, dialogOptions)];
        }
    }
}