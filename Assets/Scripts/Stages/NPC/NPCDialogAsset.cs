using UnityEngine;

[CreateAssetMenu(fileName = "NPCDialog", menuName = "Config/NPCDialog")]
public class NPCDialogAsset : JsonAsset<NPCDialogContent>
{
    public override NPCDialogContent Data { get => data; set => data = value; }
    [SerializeField] private NPCDialogContent data;

    public NPCDialogContent.DynamicLine GetLine(int index) => data.GetLine(index);
}