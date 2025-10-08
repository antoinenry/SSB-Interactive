using TMPro;
using UnityEngine;

namespace Shop
{
    [ExecuteAlways]
    public class ShopShelfDisplay : MonoBehaviour
    {
        [Header("Components")]
        public TMP_Text labelField;
        public SpriteRenderer iconField;
        public TMP_Text priceField;
        [Header("Content")]
        public string buttonID = "a";
        public ShopItem item;

        private void Update()
        {
            if (labelField) labelField.text = item.name;
            if (iconField) iconField.sprite = item.icon;
            if (priceField) priceField.text = item.price.ToString();
        }

        public void EmptyShelf()
        {
            if (labelField) labelField.text = "";
            if (iconField) iconField.sprite = null;
            if (priceField) priceField.text = "";
        }
    }
}
