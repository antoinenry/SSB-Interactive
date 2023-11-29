// Unity Asset representing a dynamic setlist (for local use)

using UnityEngine;

[CreateAssetMenu(fileName = "NewSetlist", menuName = "SSBI/Setlist")]
public class Setlist : ScriptableObject
{
    public SetlistData data;
}
