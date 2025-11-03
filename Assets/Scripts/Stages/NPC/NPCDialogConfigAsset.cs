using UnityEngine;

namespace NPC
{
    [CreateAssetMenu(fileName = "NPCDialogConfig", menuName = "Config/NPCDialogConfig")]
    public class NPCDialogConfigAsset : JsonAsset<NPCDialogConfig>
    {
        public override NPCDialogConfig Data { get => data; set => data = value; }
        [SerializeField] private NPCDialogConfig data;
        
        [CurrentToggle] public bool isCurrent;
        public static NPCDialogConfigAsset Current => CurrentAssetsManager.GetCurrent<NPCDialogConfigAsset>();
    }
}