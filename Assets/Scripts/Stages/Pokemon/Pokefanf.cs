using System;

namespace Pokefanf
{
    [Serializable]
    public struct Pokefanf
    {
        public string pokeName;
        public string musicianName;
        public string[] attacks;

        public static Pokefanf None => new Pokefanf()
        {
            pokeName = null,
            musicianName = null
        };

        public bool HasMusician => musicianName != null && musicianName != "";
        public int AttackCount => attacks != null ? attacks.Length : 0;

        public string GetAttack()
        {
            if (AttackCount > 0) return attacks[0];
            else return null;
        }
    }
}