using System.Collections;
using UnityEngine;
using TMPro;

public class HeartPlayer : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float respawnDuration = 1f;
    public float respawnBlinkPeriod = .1f;

    private Rigidbody2D body;
    private SpriteRenderer sprite;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        float xInput = AudienceInputSource.Current.GetHorizontalAxis().deltaPresses;
        float yInput = AudienceInputSource.Current.GetVerticalAxis().deltaPresses;
        body.velocity = moveSpeed * new Vector2(xInput, yInput);
    }

    public void Die()
    {
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        float startTime = Time.time;
        transform.localPosition = Vector3.zero;
        while (Time.time < startTime + respawnDuration)
        {
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(respawnBlinkPeriod);
        }
        sprite.enabled = true;
    }
}
