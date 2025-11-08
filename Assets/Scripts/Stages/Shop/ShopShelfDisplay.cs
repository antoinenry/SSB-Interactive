using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Shop
{
    [ExecuteAlways]
    public class ShopShelfDisplay : MonoBehaviour
    {
        [Header("Components")]
        public TMP_Text labelField;
        public TMP_Text priceField;
        public AudienceButtonListener button;
        public Image songToken;
        [Header("Content")]
        public ShopItem item;

        public UnityEvent<ShopItem> onChoseItem;

        private void Reset()
        {
            button = GetComponentInChildren<AudienceButtonListener>(true);
        }

        private void OnEnable()
        {
            button?.ResetButton();
            SetButtonListenersActive(true);
        }

        private void OnDisable()
        {
            SetButtonListenersActive(false);
            button?.ResetButton();
        }

        private void Update()
        {
            if (labelField) labelField.text = item.song.Title;
            if (priceField) priceField.text = item.price.ToString();
        }

        private void SetButtonListenersActive(bool active)
        {
            if (button == null) return;
            if (active)
            {
                button.onValueMaxed.AddListener(OnButtonMax);
                button.onValueChange.AddListener(OnButtonPress);
            }
            else
            {
                button.onValueMaxed.RemoveListener(OnButtonMax);
                button.onValueChange.RemoveListener(OnButtonPress);
            }
        }

        private void OnButtonPress(float value, float maxValue)
        {

        }

        private void OnButtonMax()
        {
            onChoseItem.Invoke(item);
            button?.ResetButton();
        }

        public void ResetShelf()
        {
            button?.ResetButton();
            EmptyShelf();
        }

        public void EmptyShelf()
        {
            if (labelField) labelField.text = "";
            if (priceField) priceField.text = "";
        }
    }
}
