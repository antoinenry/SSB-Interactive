using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutroStage : Stage
{
    [Header("Components")]
    public GameObject scorePanel;
    public GUIAnimatedText scoreTextField;
    public Image logo;
    public GameObject leaderboardPanel;
    public GUIAnimatedText leaderBoardTextField;
    public GameObject setlistPanel;
    public GUIAnimatedText setlistTextField;
    [Header("Contents")]
    public string replacedWithAudienceName = "nom_du_public";
    public string replacedWithScore = "score_du_public";
    public string[] ignoredSetlistTitles;
    public float delayBetweenAppartions = 1f;

    private GUIMainScorePanel mainScoreGUI;

    protected override void Awake()
    {
        base.Awake();
        mainScoreGUI = FindObjectOfType<GUIMainScorePanel>(true);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(DisplayCoroutine());
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
        HideAll();
    }

    public void HideAll()
    {
        if (scorePanel) scorePanel.SetActive(false);
        if (leaderboardPanel) leaderboardPanel.SetActive(false);
        if (logo) logo.gameObject.SetActive(false);
        if (setlistPanel) setlistPanel.SetActive(false);
    }

    private IEnumerator DisplayCoroutine()
    {
        HideAll();
        yield return new WaitForSeconds(delayBetweenAppartions);

        if (scorePanel) scorePanel.SetActive(true);
        if (scoreTextField) scoreTextField.text = GetScoreText();
        yield return new WaitForSeconds(delayBetweenAppartions);

        if (logo) logo.gameObject.SetActive(true);
        yield return new WaitForSeconds(delayBetweenAppartions);

        if (leaderboardPanel) leaderboardPanel.SetActive(true);
        if (leaderBoardTextField) leaderBoardTextField.text = GetLeaderboardText();
        yield return new WaitForSeconds(delayBetweenAppartions);

        if (setlistPanel) setlistPanel.SetActive(true);
        if (setlistTextField) setlistTextField.text = GetSetlistText();
    }

    private string GetScoreText()
    {
        if (scoreTextField == null) return string.Empty;
        string text = scoreTextField.text;
        string scoreString = mainScoreGUI != null ? mainScoreGUI.totalScore.ToString("000") : "plein de ";
        return text.Replace(replacedWithScore, scoreString);
    }

    private string GetLeaderboardText()
    {
        if (leaderBoardTextField == null) return string.Empty;
        string text = leaderBoardTextField.text;
        string scoreString = mainScoreGUI != null ? mainScoreGUI.totalScore.ToString("000") : "plein de ";
        string audienceName = ConcertAdmin.Current != null ? ConcertAdmin.Current.info.location : "vous";
        text = text.Replace(replacedWithScore, scoreString);
        text = text.Replace(replacedWithAudienceName, audienceName);
        return text;
    }

    private string GetSetlistText()
    {
        string text = "";
        SetlistInfo setlist = ConcertAdmin.Current != null ? ConcertAdmin.Current.state.setlist : SetlistInfo.None;
        foreach (SongInfo song in setlist.GetSongs())
        {
            if (ignoredSetlistTitles != null && Array.IndexOf(ignoredSetlistTitles, song.Title) != -1) continue;
            text += song.Title + "\n";
        }
        return text;
    }
}
