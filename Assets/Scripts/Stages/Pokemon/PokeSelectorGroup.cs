using UnityEngine;

namespace Pokefanf
{
    [ExecuteAlways]
    public class PokeSelectorGroup : SelectorButtonGroup<Pokefanf>
    {
        public PokeConfig config;

        private void Update()
        {
            SetItems(config.pokeFanf_starters);
        }
    }
}