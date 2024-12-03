using System.Collections;
using UnityEngine;
using TMPro;

public class HeartPlayer : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float respawnDuration = 1f;
    public float respawnBlinkPeriod = .1f;
    public int health = 100;
    public TMP_Text healthField;
    public string healthPrefix = "x";

    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private MiniGameScore score;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        score = GetComponent<MiniGameScore>();
    }

    private void Update()
    {
        healthField.text = healthPrefix + health;
        score.unitValue = health;
    }

    private void FixedUpdate()
    {
        float xInput = InputSource.GetAxis(InputSource.Axis.Direction.Horizontal, ButtonValueType.RateRaw);
        float yInput = InputSource.GetAxis(InputSource.Axis.Direction.Vertical, ButtonValueType.RateRaw);
        body.velocity = moveSpeed * new Vector2(xInput, yInput);
    }

    public void Die()
    {
        health--;
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
