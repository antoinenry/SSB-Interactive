using UnityEngine;
using NPC;

namespace Shop
{
    public class ShopStage : Stage
    {
        public NPCDialogContentAsset introDialog;
        public NPCDialogContentAsset purchaseDialog;
        public NPCDialogContentAsset outroDialog;

        private Shop shop;
        private NPCDialog npc;

        public override int MomentCount => 3;

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
                case 0: ShowIntroDialog(); break;
                case 1: ShowShop(); break;
                case 2: ShowOutroDialog(); break;
            }
        }

        private void OnBuyItem(ShopItem item)
        {
            if (shop == null) return;
            if (shop.CartIsFull)
            {
                HideShop();
            }
            else
            {
                ShowPurchaseDialog();
            }
        }

        private void OnPurchaseDialogEnd()
        {
            if (npc != null) npc.onDialogEnd.RemoveListener(OnPurchaseDialogEnd);
            ShowShop();
        }

        private void ShowIntroDialog()
        {
            HideShop();
            if (npc != null) npc.ShowDialogLine(introDialog, 0);
        }

        private void ShowShop()
        {
            HideDialog();
            if (shop == null) return;            
            shop.Open();
            shop.onBuyItem.AddListener(OnBuyItem);
        }

        private void ShowPurchaseDialog()
        {
            HideShop();
            if (npc == null) return;
            npc.ShowDialogLine(purchaseDialog, 0);
            npc.onDialogEnd.AddListener(OnPurchaseDialogEnd);
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

        private void HideShop()
        {
            if (shop == null) return;
            shop.Close();
            shop.onBuyItem.RemoveListener(OnBuyItem);
        }
    }
}