using UnityEngine;

namespace Pokefanf
{
    [ExecuteAlways]
    public class PokeSelectorGroup : SelectorButtonGroup<Pokefanf>
    {
        public PokeConfig config;

        private void Update()
        {
            SetItems(config.pokeFanf_left, config.pokeFanf_middle, config.pokeFanf_right);
        }
    }
}