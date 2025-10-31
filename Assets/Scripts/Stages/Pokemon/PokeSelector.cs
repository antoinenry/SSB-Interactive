using TMPro;
using UnityEngine;

namespace Pokefanf
{
    [ExecuteAlways]
    public class PokeSelector : SelectorButton<Pokefanf>
    {
        public TMP_Text label;

        private void Reset()
        {
            label = GetComponentInChildren<TMP_Text>(true);
        }

        private void Update()
        {
            if (label) label.text = item.pokeName;
        }
    }
}