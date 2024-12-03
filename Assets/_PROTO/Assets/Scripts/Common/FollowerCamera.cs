using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerCamera : Follower
{
    [Header("Zoom")]
    public float minimumCameraSize;
    public float maximumCameraSize;
    public float cameraSizeMargin;
    public float cameraZoomSpeed;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    protected override void FixedUpdate()
    {
        if (targets == null || targets.Length == 0) return;
        Vector3[] targetPositions = TargetPositions;
        Vector3 followPos = GetFollowPosition(targetPositions);
        FollowPosition(followPos, Time.fixedDeltaTime);
        Vector2 targetDistances = GetMaxDistanceXY(targetPositions, transform.position);
        float cameraSize;
        if (targetDistances.x > targetDistances.y * cam.aspect) cameraSize = targetDistances.x / cam.aspect;
        else cameraSize = targetDistances.y;
        cameraSize = Mathf.Clamp(cameraSize + cameraSizeMargin, minimumCameraSize, maximumCameraSize);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, cameraSize, cameraZoomSpeed * Time.fixedDeltaTime);
    }

    private Vector2 GetMaxDistanceXY(Vector3[] positions, Vector3 center)
    {
        Vector2 max = Vector2.zero;
        foreach (Vector3 v in positions)
        {
            max.x = Mathf.Max(max.x, Mathf.Abs(v.x - center.x));
            max.y = Mathf.Max(max.y, Mathf.Abs(v.y - center.y));
        }
        return max;
    }
}
