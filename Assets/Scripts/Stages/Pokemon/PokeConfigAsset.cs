using System;
using UnityEngine;

namespace Pokefanf
{
    [Serializable]
    public struct PokeConfig
    {
        public Pokefanf[] pokeFanf_starters;
        public string[] musician_priority;
    }

    [CreateAssetMenu(fileName = "PokeConfig", menuName = "Config/PokeConfig")]
    public class PokeConfigAsset : JsonAsset<PokeConfig>
    {
        [CurrentToggle] public bool isCurrent;
        public Pokefanf GetStarterByPokeName(string name)
        {
            if (Data.pokeFanf_starters == null) return Pokefanf.None;
            return Array.Find(Data.pokeFanf_starters, p => p.pokeName == name);
        }
        public Pokefanf GetStarterByMusicianName(string name)
        {
            if (Data.pokeFanf_starters == null) return Pokefanf.None;
            return Array.Find(Data.pokeFanf_starters, p => p.musicianName == name);
        }

        public Pokefanf GetEnemyMusician(string ally_musician)
        {
            string ennemy_musician = null;
            int priority_index = 0, priority_max = Data.musician_priority != null ? Data.musician_priority.Length - 1 : 0;
            string priority_musician;

            while (priority_index < priority_max)
            {
                priority_musician = priority_index <= priority_max ? Data.musician_priority[priority_index] : null;
                if (ally_musician != priority_musician)
                {
                    ennemy_musician = priority_musician;
                    break;
                }
                else
                    priority_index++;
            }
            return GetStarterByMusicianName(ennemy_musician);
        }
    }
}