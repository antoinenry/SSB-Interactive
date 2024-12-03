using UnityEngine;
using TMPro;
using System.Collections;

public class CoinCatcher : MonoBehaviour
{
    public TMP_Text counterField;
    public string prefix = "x";
    public int count = 0;
    public float pointsPerCoin = 100f;
    public ParticleSystem loseCoinEffect;
    public float loseCoinEffectDuration = 1f;

    public int PointsCount => Mathf.FloorToInt(count * pointsPerCoin);

    private void Update()
    {
        if (counterField) counterField.text = prefix + count;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Coin coin = collision.GetComponent<Coin>();
        if (coin && !coin.Caught)
        {
            coin.CatchAndDestroy();
            count++;
        }
    }

    public void LoseCoins(int loseCount)
    {
        int c = Mathf.Min(count, loseCount);
        if (c <= 0) return;
        count -= c;
        StartCoroutine(LoseCoinsEffectCoroutine(c));
    }

    private IEnumerator LoseCoinsEffectCoroutine(int loseCount)
    {
        if (loseCoinEffect == null || loseCount <= 0) yield break;
        ParticleSystem effect = Instantiate(loseCoinEffect);
        effect.transform.position = transform.position;
        ParticleSystem.MainModule main = effect.main;
        main.maxParticles = loseCount;
        main.startLifetime = loseCoinEffectDuration;
        effect.Play();
        yield return new WaitForSeconds(loseCoinEffectDuration);
        Destroy(effect.gameObject);
    }
}
