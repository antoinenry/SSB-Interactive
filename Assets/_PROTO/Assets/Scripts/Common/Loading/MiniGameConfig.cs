using UnityEngine;

[CreateAssetMenu(fileName = "Minigames", menuName = "Config/MiniGames")]
public class MiniGameConfig : JsonAsset<MiniGameConfigData>
{
    [CurrentToggle] public bool isCurrent;
    static public MiniGameConfig Current => CurrentAssetsManager.GetCurrent<MiniGameConfig>();
}
