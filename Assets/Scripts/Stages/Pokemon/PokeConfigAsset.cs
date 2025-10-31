using System;
using UnityEngine;

namespace Pokefanf
{
    [CreateAssetMenu(fileName = "PokeConfig", menuName = "Config/PokeConfig")]
    public class PokeConfigAsset : JsonAsset<PokeConfig>
    {
        public override PokeConfig Data { get => data; set => data = value; }
        [SerializeField] private PokeConfig data;

        public void SetBattleConfig(Pokefanf ally)
        {
            data.SetBattleConfig(ally);
            Save();
        }
    }

    [Serializable]
    public struct PokeConfig
    {
        public Pokefanf[] pokeFanf_starters;
        public Pokefanf pokeFanf_ally;
        public Pokefanf pokeFanf_ennemy;
        public string[] musician_priority;

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

        public void SetBattleConfig(Pokefanf ally)
        {
            pokeFanf_ally = ally;
            pokeFanf_ennemy = Pokefanf.None;
            int priority_index = 0, priority_max = musician_priority != null ? musician_priority.Length - 1 : 0;
            string priority_musician = null;

            while (priority_index < priority_max)
            {
                priority_musician = priority_index <= priority_max ? musician_priority[priority_index] : null;
                if (ally.musicianName != priority_musician)
                {
                    pokeFanf_ennemy = GetStarterByMusicianName(priority_musician);
                    break;
                }
                else
                    priority_index++;
            }
        }
    }
}