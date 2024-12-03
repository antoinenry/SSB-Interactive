using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GUIMainScorePanel : MonoBehaviour
{
    public bool hideMiniScore;
    public TMP_Text miniScoreField;
    public float miniScoreUnits;
    public string miniScoreSuffix;
    public bool hideCoins;
    public TMP_Text coinsField;
    public string coinFieldPrefix = "x ";
    public float coinCount;
    public Image coinIcon;
    public TMP_Text congratsField;
    public string congratsPrefix = "Bravo ";
    public string publicName;
    public TMP_Text totalField;
    public float totalScore;
    public string totalSuffix = " points";
    [Header("Animation")]
    public float preAnimationPause = 2f;
    public float pointsPerSecond = 100f;
    public float maxAnimationDuration = 5f;

    public void PlayAddingUpAnimation(int pointsFromMiniScore, int pointsFromCoinScore)
    {
        StopAllCoroutines();
        StartCoroutine(AddingAnimationCoroutine(pointsFromMiniScore, pointsFromCoinScore));
    }

    private IEnumerator AddingAnimationCoroutine(int pointsFromMiniScore, int pointsFromCoins)
    {
        SetMiniScoreValue(miniScoreUnits, miniScoreSuffix);
        SetCoinCount(coinCount);
        SetPublicName(publicName);
        SetTotalValue(totalScore);
        // Animation
        yield return new WaitForSecondsRealtime(preAnimationPause);
        float pps = pointsPerSecond;
        if (pps <= 0f || (pointsFromMiniScore + pointsFromCoins) / pps > maxAnimationDuration)
            pps = (pointsFromMiniScore + pointsFromCoins) / maxAnimationDuration;
        float deltaTime;
        float startingTotal = totalScore;
        // Mini game
        float miniUnitsPerSecond = pps * miniScoreUnits / pointsFromMiniScore;
        while (miniScoreUnits > 0)
        {
            deltaTime = Time.unscaledDeltaTime;
            SetMiniScoreValue(miniScoreUnits - deltaTime * miniUnitsPerSecond, miniScoreSuffix);
            SetTotalValue(totalScore + deltaTime * pps);
            yield return null;
        }
        miniScoreUnits = 0;
        SetMiniScoreValue(0, miniScoreSuffix);
        totalScore = startingTotal + pointsFromMiniScore;
        SetTotalValue(totalScore);
        // Coins
        float coinsPerSecond = pps * coinCount / pointsFromCoins;
        while (coinCount > 0)
        {
            deltaTime = Time.unscaledDeltaTime;
            SetCoinCount(coinCount - deltaTime * coinsPerSecond);
            SetTotalValue(totalScore + deltaTime * pps);
            yield return null;
        }
        coinCount = 0;
        SetCoinCount(0);
        totalScore = startingTotal + pointsFromMiniScore + pointsFromCoins;
        SetTotalValue(totalScore);
    }

    public void SetMiniScoreValue(float value, string unit)
    {
        miniScoreUnits = value;
        if (miniScoreField)
        {
            miniScoreField.enabled = !hideMiniScore;
            miniScoreField.text = (int)value + unit;
        }
    }

    public void SetCoinCount(float value)
    {
        coinCount = value;
        if (coinsField)
        {
            coinsField.enabled = !hideCoins;
            coinsField.text = coinFieldPrefix + (int)value;
        }
    }

    public void SetPublicName(string value)
    {
        if (congratsField) congratsField.text = congratsPrefix + value;
    }

    public void SetTotalValue(float value)
    {
        totalScore = value;
        if (totalField) totalField.text = (int)value + totalSuffix;
    }
}
