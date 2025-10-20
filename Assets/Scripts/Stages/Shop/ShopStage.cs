using UnityEngine;

namespace Shop
{
    public class ShopStage : Stage
    {
        [Header("Moments")]
        public int welcomeDialogueMoment = 0;
        public int shoppingMoment = 1;
        public int goodbyeDialogueMoment = 2;

        private Shop shop;

        public override int MomentCount => 3;

        protected override bool HasAllComponents()
        {
            if (shop == null) shop = GetComponentInChildren<Shop>(true);
            return base.HasAllComponents() && shop != null;
        }

        protected override void OnMomentChange(int value)
        {
            base.OnMomentChange(value);
        }
    }
}