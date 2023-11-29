// Data structure representing a dynamic sequence of live events (chapters)

using System;

[Serializable] public struct SetlistData
{
    // Data structure representing an event in a dynamic setlist
    [Serializable] public struct Chapter
    {
        public string act;      // What the band is doing (can be a song, a speech, or any live feature)
        public string game;     // What the screen is doing (can be a minigame, a visual or any interactive display)
        public string input;    // Can be used to feed the game some data (like a list of selectable songs)
        public string output;   // Can be used to set a reference to the game output (for later use in the setlist)
    }

    // Data structure representing a selection of songs for dynamic selection
    [Serializable] public struct SongPool
    {
        public string name;     // The name used for reference in the setlist
        public string songs;    // Songs by their reference name in the setlist
    }

    public Chapter[] chapters;
    public SongPool[] pools;
}
