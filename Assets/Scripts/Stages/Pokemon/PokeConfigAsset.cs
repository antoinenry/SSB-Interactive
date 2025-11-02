using System;
using UnityEngine;

namespace Pokefanf
{
    [CreateAssetMenu(fileName = "PokeConfig", menuName = "Config/PokeConfig")]
    public class PokeConfigAsset : JsonAsset<PokeConfig>
    {
        public override PokeConfig Data { get => data; set => data = value; }
        [SerializeField] private PokeConfig data;

        [CurrentToggle] public bool isCurrent;
        public static PokeConfigAsset Current => CurrentAssetsManager.GetCurrent<PokeConfigAsset>();
    }

    [Serializable]
    public struct PokeConfig
    {
        public Pokefanf[] pokeFanf_starters;
        public string ally_musician;
        public string ennemy_musician;
        public string[] musician_priority;

        public static PokeConfig Current
        {
            get
            {
                PokeConfigAsset asset = PokeConfigAsset.Current;
                return asset != null ? asset.Data : default;
            }
        }

        public Pokefanf GetStarterByPokeName(string name)
        {
            if (pokeFanf_starters == null) return Pokefanf.None;
            return Array.Find(pokeFanf_starters, p => p.pokeName == name);
        }

        public Pokefanf GetStarterByMusicianName(string name)
        {
            if (pokeFanf_starters == null) return Pokefanf.None;
            return Array.Find(pokeFanf_starters, p => p.musicianName == name);
        }

        public void SetBattleConfig(string allyMusicianName)
        {
            ally_musician = allyMusicianName;
            ennemy_musician = null;
            int priority_index = 0, priority_max = musician_priority != null ? musician_priority.Length - 1 : 0;
            string priority_musician = null;

            while (priority_index < priority_max)
            {
                priority_musician = priority_index <= priority_max ? musician_priority[priority_index] : null;
                if (ally_musician != priority_musician)
                {
                    ennemy_musician = priority_musician;
                    break;
                }
                else
                    priority_index++;
            }
        }

        public Pokefanf Ally => GetStarterByMusicianName(ally_musician);
        public Pokefanf Ennemy => GetStarterByMusicianName(ennemy_musician);
    }
}