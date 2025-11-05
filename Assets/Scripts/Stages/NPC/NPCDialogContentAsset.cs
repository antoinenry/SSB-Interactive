using UnityEngine;

[CreateAssetMenu(fileName = "NPCDialog", menuName = "Config/NPCDialog")]
public class NPCDialogContentAsset : JsonAsset<NPCDialogContent>
{
    public int LineCount => Data.LineCount;
    public NPCDialogContent.DynamicLine GetLine(int index) => Data.GetLine(index);
    public bool HasReaction(int lineIndex, int answerIndex) => Data.HasReaction(lineIndex, answerIndex);
    public string GetReaction(int lineIndex, int answerIndex) => Data.GetReaction(lineIndex, answerIndex);
}