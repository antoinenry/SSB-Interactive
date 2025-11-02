using System;
using System.Collections.Generic;
using UnityEngine;
using Pokefanf;

namespace NPC
{
    [Serializable]
    public class NPCDialogInjector
    {
        public NPCDialogInjectorConfig config;

        private Dictionary<string, string> injectionDictionary;

        public void UpdateDictionary()
        {
            if (NPCDialogConfigAsset.Current != null) config = NPCDialogConfigAsset.Current.Data.injector;
            injectionDictionary = new Dictionary<string, string>();
            if (ConcertAdmin.Current != null)
            {
                injectionDictionary.TryAdd(config.key_ConcertName, ConcertAdmin.Current.info.name);
                injectionDictionary.TryAdd(config.key_VenueName, ConcertAdmin.Current.info.location);
            }
            injectionDictionary.TryAdd(config.key_PokeAllyName, PokeConfig.Current.Ally.pokeName);
            injectionDictionary.TryAdd(config.key_PokeEnemyName, PokeConfig.Current.Ennemy.pokeName);
        }

        public string Inject(string text)
        {
            if (text == null || injectionDictionary == null) return text;
            foreach (KeyValuePair<string, string> pair in injectionDictionary)
            {
                if (pair.Key == null || pair.Key == string.Empty) continue;
                if (pair.Value == null || pair.Value == string.Empty) continue;
                text = text.Replace(pair.Key, pair.Value);
            }
            return text;
        }
    }

    [Serializable]
    public struct NPCDialogInjectorConfig
    {
        public string key_ConcertName;
        public string key_VenueName;
        public string key_PokeAllyName;
        public string key_PokeEnemyName;
    }
}