using System;

[Serializable]
public struct NPCDialogContent
{
    [Serializable]
    public struct DynamicLine
    {
        public string text;
        public string[] answers;

        public static DynamicLine None => new DynamicLine() { text = null, answers = null };

        public int AnswerCount => answers != null ? answers.Length : 0;
    }

    [Serializable]
    public struct ReactionLine
    {
        public int fromLine;
        public string[] responses;

        public static ReactionLine None => new ReactionLine() { fromLine = -1, responses = null };

        public int ResponseCount => responses != null ? responses.Length : 0;

        public string GetResponse(int index) => index >= 0 && index < ResponseCount ? responses[index] : null;
    }

    public DynamicLine[] lines;
    public ReactionLine[] reactions;

    public int LineCount => lines != null ? lines.Length : 0;
    public int ReactionCount => reactions != null ? reactions.Length : 0;

    public DynamicLine GetLine(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= LineCount) return DynamicLine.None;
        else return lines[lineIndex];
    }

    public string GetReaction(int lineIndex, int answerIndex)
    {
        if (ReactionCount == 0) return null;
        else return Array.Find(reactions, r => r.fromLine == lineIndex).GetResponse(answerIndex);
    }
}
