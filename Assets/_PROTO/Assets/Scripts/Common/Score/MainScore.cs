using UnityEngine;

public class MainScore : MonoBehaviour
{
    public GUIMainScorePanel scoreDisplay;
    public string postScoreUri = "concert/today/score";
    public string publicName = "le public";
    public int totalScore = 0;

    private HttpRequest scorePost;
    private HttpClientScriptable httpClient;

    public bool DisplayScore
    {
        get => scoreDisplay ? scoreDisplay.gameObject.activeInHierarchy : false;
        set => scoreDisplay?.gameObject?.SetActive(value);
    }

    private void Awake()
    {
        httpClient = CurrentAssetsManager.GetCurrent<HttpClientScriptable>();
    }

    private void Start()
    {
        DisplayScore = false;
    }

    private void InitScorePostRequest()
    {
        scorePost = new();
        scorePost.type = HttpRequest.RequestType.POST;
        scorePost.requestUri = postScoreUri;
    }

    public void RegisterStageScore()
    {
        Stage currentStage = FindObjectOfType<Stage>(true);
        if (currentStage == null) return;
        string stageName = currentStage.name;
        int pointsFromMiniGame, pointsFromCoins;
        MiniGameScore miniScore = currentStage.GetComponentInChildren<MiniGameScore>(true);
        if (miniScore)
        {
            pointsFromMiniGame = miniScore.PointsValue;
            scoreDisplay.miniScoreUnits = miniScore.RoundedUnitValue;
            scoreDisplay.miniScoreSuffix = miniScore.unit;
        }
        else
        {
            pointsFromMiniGame = 0;
            scoreDisplay.miniScoreUnits = 0;
            scoreDisplay.miniScoreSuffix = "";
        }
        CoinCatcher coins = currentStage.GetComponentInChildren<CoinCatcher>(true);
        if (coins)
        {
            pointsFromCoins = coins.PointsCount;
            scoreDisplay.coinCount = coins.count;
        }
        else
        {
            pointsFromCoins = 0;
            scoreDisplay.coinCount = 0;
        }
        PostStageScore(pointsFromMiniGame + pointsFromCoins, stageName);
        if (DisplayScore)
        {
            scoreDisplay.hideMiniScore = miniScore == null;
            scoreDisplay.hideCoins = coins == null;
            scoreDisplay.PlayAddingUpAnimation(pointsFromMiniGame, pointsFromCoins);
        }
    }

    public void PostStageScore(int score, string stageName)
    {
        if (scorePost == null) InitScorePostRequest();
        ScoreData data = new()
        {
            stageScore = score,
            stage = stageName
        };
        scorePost.SerializeBody(data);
        if (httpClient != null) httpClient.SendRequest(scorePost);
    }

    public void RequestScoreTotal()
    {

    }
}
