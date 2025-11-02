using System;
using System.Collections.Generic;

namespace NPC
{
    public abstract class NPCDialogInjector
    {
        protected Dictionary<string, string> injectionDictionary;

        public void UpdateDictionary(string key, string value)
        {
            if (injectionDictionary == null) injectionDictionary = new Dictionary<string, string>();
            if (injectionDictionary.TryAdd(key, value)) return;
            else injectionDictionary[key] = value;
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

        public static string InjectAll(string text)
        {
            NPCDialogInjector[] injectors = NPCDialogConfigAsset.Current != null ? NPCDialogConfigAsset.Current.Data.injectors.GetAllInjectors() : null;
            if (injectors == null) return text;
            foreach (NPCDialogInjector injector in injectors)
            {
                if (injector == null) continue;
                text = injector.Inject(text);
            }
            return text;
        }
    }


    [Serializable]
    public struct NPCDialogInjectorConfig
    {
        public NPCDialogInjector_ConcertAdmin concertAdmin;
        public NPCDialogInjector_Pokefanf pokeFanf;

        public static NPCDialogInjectorConfig Current => NPCDialogConfigAsset.Current != null ? NPCDialogConfigAsset.Current.Data.injectors : new NPCDialogInjectorConfig();

        public NPCDialogInjector[] GetAllInjectors() => new NPCDialogInjector[]
        {
            concertAdmin, pokeFanf
        };

    }

    [Serializable]
    public class NPCDialogInjector_ConcertAdmin : NPCDialogInjector
    {
        public string key_ConcertName;
        public string key_ConcertLocation;
    }

    [Serializable]
    public class NPCDialogInjector_Pokefanf : NPCDialogInjector
    {
        public string key_AllyPoke;
        public string key_EnemyPoke;
        public string key_CurrentAttack;
    }
}