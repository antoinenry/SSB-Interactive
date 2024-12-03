using UnityEngine;
using System;

public class Follower : MonoBehaviour
{
    public enum MultipleTargetsMode { AveragePosition, xMax, xMin, yMax, yMin }

    public Transform[] targets;
    public MultipleTargetsMode multipleMode;
    public Vector2 offset;
    public float xSpeed;
    public float ySpeed;
    public float minDistance;
    public float maxDistance;

    virtual protected void FixedUpdate()
    {
        if (targets == null || targets.Length == 0) return;
        Vector3 followPos = GetFollowPosition(TargetPositions);
        FollowPosition(followPos, Time.fixedDeltaTime);
    }

    protected Vector3[] TargetPositions => Array.ConvertAll(targets, t => t.position + (Vector3)offset);

    protected Vector3 GetFollowPosition(Vector3[] targetPositions)
    {
        Vector3 followPos = targetPositions[0];
        if (targets.Length > 1)
        {
            switch (multipleMode)
            {
                case MultipleTargetsMode.AveragePosition: followPos = AveragePosition(targetPositions); break;
                case MultipleTargetsMode.xMax: followPos = XMaxPosition(targetPositions); break;
                case MultipleTargetsMode.xMin: followPos = XMinPosition(targetPositions); break;
                case MultipleTargetsMode.yMax: followPos = YMaxPosition(targetPositions); break;
                case MultipleTargetsMode.yMin: followPos = YMinPosition(targetPositions); break;
            }
        }
        return followPos;
    }

    private Vector3 AveragePosition(Vector3[] positions)
    {
        Vector3 average = Vector3.zero;
        foreach (Vector3 v in positions) average += v;
        return average / positions.Length;
    }

    private Vector3 XMaxPosition(Vector3[] positions)
    {
        Vector3 max = positions[0];
        foreach (Vector3 v in positions) if (v.x > max.x) max = v;
        return max;
    }

    private Vector3 XMinPosition(Vector3[] positions)
    {
        Vector3 min = positions[0];
        foreach (Vector3 v in positions) if (v.x < min.x) min = v;
        return min;
    }

    private Vector3 YMaxPosition(Vector3[] positions)
    {
        Vector3 max = positions[0];
        foreach (Vector3 v in positions) if (v.y > max.y) max = v;
        return max;
    }

    private Vector3 YMinPosition(Vector3[] positions)
    {
        Vector3 min = positions[0];
        foreach (Vector3 v in positions) if (v.y < min.y) min = v;
        return min;
    }

    protected void FollowPosition(Vector3 targetPosition, float deltaTime)
    {
        Vector3 pos = transform.position;
        if (xSpeed >= 0)
        {
            float xDistance = Mathf.Abs(pos.x - targetPosition.x);
            if (maxDistance >= 0 && xDistance > maxDistance)
                pos.x = Mathf.MoveTowards(pos.x, targetPosition.x, xDistance - maxDistance);
            else if (xDistance > minDistance)
                pos.x = Mathf.Lerp(pos.x, targetPosition.x, xSpeed * deltaTime);
        }
        if (ySpeed >= 0)
        {
            float yDistance = Mathf.Abs(pos.y - targetPosition.y);
            if (maxDistance >= 0 && yDistance > maxDistance)
                pos.y = Mathf.MoveTowards(pos.y, targetPosition.y, yDistance - maxDistance);
            else if (yDistance > minDistance)
                pos.y = Mathf.Lerp(pos.y, targetPosition.y, ySpeed * deltaTime);
        }
        transform.position = pos;

    }
}
