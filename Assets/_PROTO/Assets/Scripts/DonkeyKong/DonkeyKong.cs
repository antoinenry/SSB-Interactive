using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DonkeyKong : MonoBehaviour
{
    public float climbSecondsPerProp;
    public float jumpSpeed;
    public float jumpHeight;
    public Sprite idleSprite;
    public Sprite[] climbSprites;

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private PropTower tower;
    private List<Vector2> climbPath;
    private Vector2 positionOnPlatform;

    private void OnDrawGizmos()
    {
        if(climbPath != null)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0, pathLength = climbPath.Count; i < pathLength - 1; i++)
            {
                Gizmos.DrawLine(climbPath[i], climbPath[i + 1]);
            }
        }
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        tower = FindObjectOfType<PropTower>();
        positionOnPlatform = body.position - tower.currentFloor.position;
    }

    private void OnEnable()
    {
        tower.onPassFloor.AddListener(ClimbTower);
        spriteRenderer.sprite = idleSprite;
    }

    private void OnDisable()
    {
        tower.onPassFloor.RemoveListener(ClimbTower);
    }

    private void ClimbTower()
    {
        StopAllCoroutines();
        if (tower.props != null)
        {
            climbPath = tower.props.ConvertAll(prop => prop.position);
            climbPath.OrderBy(pt => pt.y);
            Vector2 finalSide = Random.Range(0, 2) == 0 ? Vector2.right : Vector2.left;
            positionOnPlatform.x = .3f * (tower.currentDifficulty.floorWidth);
            if (Random.Range(0, 2) == 1) positionOnPlatform.x = -positionOnPlatform.x;
            climbPath.Add((Vector2)tower.nextFloor.transform.position + positionOnPlatform);
        }
        else
            climbPath = null;
        StartCoroutine(ClimbCoroutine());
    }

    private IEnumerator ClimbCoroutine()
    {
        int climbStep = 0;
        foreach(Vector2 pt in climbPath)
        {
            if (pt.y - body.position.y < jumpHeight) continue;
            yield return new WaitForSeconds(climbSecondsPerProp);
            while (body.position != pt)
            {
                climbStep++;
                spriteRenderer.sprite = climbSprites[climbStep % climbSprites.Length];
                body.MovePosition(Vector2.MoveTowards(body.position, pt, jumpSpeed * Time.fixedDeltaTime));
                yield return new WaitForFixedUpdate();
            }
        }
        body.position = climbPath[climbPath.Count - 1];
        spriteRenderer.sprite = idleSprite;
        climbPath = null;
    }
}
