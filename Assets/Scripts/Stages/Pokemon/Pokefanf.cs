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
    }
}