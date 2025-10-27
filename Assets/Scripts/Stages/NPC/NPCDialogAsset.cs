using UnityEngine;

[CreateAssetMenu(fileName = "NPCDialog", menuName = "Config/NPCDialog")]
public class NPCDialogAsset : JsonAsset<NPCDialogContent>
{
    public override NPCDialogContent Data { get => data; set => data = value; }
    [SerializeField] private NPCDialogContent data;

    public int LineCount => data.LineCount;
    public NPCDialogContent.DynamicLine GetLine(int index) => data.GetLine(index);
    public bool HasReaction(int lineIndex, int answerIndex) => data.HasReaction(lineIndex, answerIndex);
    public string GetReaction(int lineIndex, int answerIndex) => data.GetReaction(lineIndex, answerIndex);
}