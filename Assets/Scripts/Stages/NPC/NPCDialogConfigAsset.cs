using UnityEngine;

namespace NPC
{
    [CreateAssetMenu(fileName = "NPCDialogConfig", menuName = "Config/NPCDialogConfig")]
    public class NPCDialogConfigAsset : JsonAsset<NPCDialogConfig>
    {
        [CurrentToggle] public bool isCurrent;
        public static NPCDialogConfigAsset Current => CurrentAssetsManager.GetCurrent<NPCDialogConfigAsset>();
    }
}