using System;
using UnityEngine;

namespace Pokefanf
{
    [CreateAssetMenu(fileName = "PokeConfig", menuName = "Config/PokeConfig")]
    public class PokeConfigAsset : JsonAsset<PokeConfig>
    {
        public override PokeConfig Data { get => data; set => data = value; }
        [SerializeField] private PokeConfig data;
    }

    [Serializable]
    public struct PokeConfig
    {
        public Pokefanf pokeFanf_left;
        public Pokefanf pokeFanf_middle;
        public Pokefanf pokeFanf_right;
        public Pokefanf pokeFanf_ally;
        public Pokefanf pokeFanf_ennemy;
    }
}