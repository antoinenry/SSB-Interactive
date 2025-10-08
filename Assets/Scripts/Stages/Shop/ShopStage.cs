using System;
using UnityEngine;

namespace Shop
{
    [Serializable]
    public struct ShopItem
    {
        public string name;
        public Sprite icon;
        public int price;
    }

    public class ShopStage : Stage
    {
        public ShopItem[] inventory;
        public ShopShelfDisplay[] shelfDisplays;

        private PollMaster poll;

        protected override bool HasAllComponents()
        {
            if (base.HasAllComponents() && poll) return true;
            poll = GetComponentInChildren<PollMaster>(true);
            return base.HasAllComponents() && poll;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (poll) poll.onCandidateWins.AddListener(OnPollWinner);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (poll) poll.onCandidateWins.RemoveListener(OnPollWinner);
        }

        private void Update()
        {
            int shelfCount = shelfDisplays != null ? shelfDisplays.Length : 0,
                inventoryCount = inventory != null ? inventory.Length : 0;
            if (shelfCount == 0) return;
            int inventory_index = 0;
            foreach (ShopShelfDisplay shelf in shelfDisplays)
            {
                if (shelf == null) continue;
                if (inventory_index < inventoryCount) shelf.item = inventory[inventory_index];
                else shelf.EmptyShelf();
                inventory_index++;
            }
        }

        private void OnPollWinner(PollMaster.Candidate candidate)
        {
            if (shelfDisplays == null) return;
            string buttonID = candidate?.buttonID;
            int shelfIndex = Array.FindIndex(shelfDisplays, s => s.buttonID == buttonID);
            if (shelfIndex != -1) BuyItem(shelfDisplays[shelfIndex].item);           
        }

        private void BuyItem(ShopItem item)
        {
            Debug.Log("BOUGHT A " + item.name + " !");
        }
    }
}