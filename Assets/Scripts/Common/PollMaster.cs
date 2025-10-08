using System;
using UnityEngine;
using UnityEngine.Events;

public class PollMaster : MonoBehaviour
{
    [Serializable]
    public class Candidate
    {
        public string buttonID = "a";
        public int votes = 0;
        public int maxVotes = 10;
        public GUIVoteGauge voteGUI;

        public void SetVotes(int value, int maxValue)
        {
            votes = value;
            maxVotes = maxValue;
        }

        public void UpdateGUI()
        {
            if (voteGUI == null) return;
            voteGUI.votes = votes;
            voteGUI.maxVotes = maxVotes;
        }
    }

    public Candidate[] candidates;
    public int voteGoal = 100;
    public UnityEvent<Candidate> onCandidateWins;

    private void Awake()
    {
        ResetVotes();
    }

    private void OnEnable()
    {
        AudienceInput.OnAudienceInput.AddListener(OnAudienceInput);
    }

    private void OnDisable()
    {
        AudienceInput.OnAudienceInput.RemoveListener(OnAudienceInput);
    }

    private void Update()
    {
        if (candidates == null) return;
        foreach (Candidate c in candidates)
        {
            if (c == null) continue;
            c.UpdateGUI();
            if (c.votes >= voteGoal) onCandidateWins.Invoke(c);
        }
    }

    private void ResetVotes()
    {
        if (candidates == null) return;
        foreach (Candidate c in candidates)
        {
            if (c == null) continue;
            c.SetVotes(0, voteGoal);
        }
    }

    private void UpdateVotes()
    {
        if (candidates == null) return;
        int voteInput = 0;
        foreach (Candidate c in candidates)
        {
            if (c == null) continue;
            voteInput = (int)AudienceInput.GetButton(c.buttonID, ButtonValueType.RawDelta);
            c.SetVotes(c.votes + voteInput, voteGoal);            
        }
    }

    private void OnAudienceInput()
    {
        UpdateVotes();
    }
}
