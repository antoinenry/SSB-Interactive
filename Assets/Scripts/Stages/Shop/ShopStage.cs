using UnityEngine;

namespace Shop
{
    public class ShopStage : Stage
    {
        public enum Phase { NPC_Welcome, Shopping, NPC_Purchase, NPC_Goodbye }

        public Phase currentPhase;
        public string welcomeDialog = "Bienvenue étranger !";
        public string purchaseDialog = "Excellent choix !";
        public string goodbyeDialog = "Héhé, un plaisir de faire affaire avec vous, étranger.";
        public float dialogDuration = 2f;

        private Shop shop;
        private NPCDialog dialog;
        private float dialogTimer;

        protected override bool HasAllComponents()
        {
            if (shop == null) shop = GetComponentInChildren<Shop>(true);
            if (dialog == null) dialog = GetComponentInChildren<NPCDialog>(true);
            return base.HasAllComponents() && shop != null && dialog != null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            shop.onBuyItem.AddListener(OnBuyItem);
            currentPhase = Phase.NPC_Welcome;
        }

        protected override void OnDisable()
        {
            shop.onBuyItem.RemoveListener(OnBuyItem);
            base.OnDisable();
        }

        private void Update()
        {
            if (Application.isPlaying == false) dialogTimer = 0f;
            switch(currentPhase)
            {
                case Phase.NPC_Welcome:
                    dialog.text = welcomeDialog;
                    ShowNPC();
                    dialogTimer += Time.deltaTime;
                    if (dialogTimer > dialogDuration) currentPhase = Phase.Shopping;
                    break;
                case Phase.Shopping:
                    ShowShop();
                    dialogTimer = 0f;
                    break;
                case Phase.NPC_Purchase:
                    dialog.text = purchaseDialog;
                    ShowNPC();
                    dialogTimer += Time.deltaTime;
                    if (dialogTimer > dialogDuration) currentPhase = shop.CartIsFull ? Phase.NPC_Goodbye : Phase.Shopping;
                    break;
                case Phase.NPC_Goodbye:
                    dialog.text = goodbyeDialog;
                    ShowNPC();
                    break;
            }
        }

        private void ShowNPC()
        {
            dialog.gameObject.SetActive(true);
            shop.Close();

        }

        private void ShowShop()
        {
            dialog.gameObject.SetActive(false);
            shop.Open();
        }

        private void OnBuyItem(ShopItem shopItem)
        {
            if (shop.CartIsFull)
            {
                currentPhase = Phase.NPC_Goodbye;
            }
            else
            {
                currentPhase = Phase.NPC_Purchase;
            }
        }
    }
}