using System;
using UnityEngine;

[ExecuteAlways]
public class MapRoad : MapNavigationStep
{
    public MapNode firstNode;
    public MapNode lastNode;

    private LineRenderer lineRenderer;
    private Vector2[] wayPoints;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        SnapToNodes();
    }

    private void SnapToNodes()
    {
        // Get waypoints from line renderer
        if (lineRenderer != null)
        {
            int lineLength = lineRenderer.positionCount;
            Vector3[] linePositions = new Vector3[lineLength];
            lineRenderer.GetPositions(linePositions);
            if (PointCount != lineLength) Array.Resize(ref wayPoints, lineLength);
            Array.Copy(Array.ConvertAll(linePositions, p => (Vector2)p), wayPoints, wayPoints.Length);
        }
        // Ensure minimum length and snap to nodes
        if (wayPoints == null) wayPoints = new Vector2[0];
        if (firstNode != null)
        {
            if (PointCount == 0) Array.Resize(ref wayPoints, 1);
            wayPoints[0] = firstNode.transform.position;
        }
        if (lastNode != null)
        {
            if (PointCount < 2) Array.Resize(ref wayPoints, 2);
            wayPoints[PointCount - 1] = lastNode.transform.position;
        }
        // Update changes to line renderer
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = PointCount;
            lineRenderer.SetPositions(Array.ConvertAll(wayPoints, pt => (Vector3)pt));
        }
    }

    public int PointCount => wayPoints != null ? wayPoints.Length : 0;

    public float Length
    {
        get
        {
            float l = 0f;
            for (int i = 0, iend = PointCount - 1; i < iend; i++)
                l += Vector2.Distance(wayPoints[i], wayPoints[i + 1]);
            return l;
        }
    }

    public Vector2 GetTravelPosition(float t)
    {
        if (t <= 0f) return wayPoints[0];
        if (t >= 1f) return wayPoints[PointCount - 1];
        float target_distance = t * Length;
        float wayPointDistance = 0f;
        int wayPointIndex = 0;
        for (int i = 0, iend = PointCount - 1; i < iend; i++)
        {
            wayPointDistance += Vector2.Distance(wayPoints[i], wayPoints[i + 1]);
            if (wayPointDistance >  target_distance)
            {
                wayPointIndex = i;
                break;
            }
        }
        return Vector2.MoveTowards(wayPoints[wayPointIndex + 1], wayPoints[wayPointIndex], wayPointDistance - target_distance);
    }

    public override void SetNavigatorPosition(MapNavigator navigator)
    {
        if (navigator == null) return;
        navigator.transform.position = GetTravelPosition(navigator.travelProgress);
    }

    public override void SetNavigatorMotion(MapNavigator navigator, float deltaTime)
    {
        if (navigator == null || Length == 0f) return;
        float directionFactor = 0f;
        if (navigator.travelDirection != 0) directionFactor = Mathf.Sign(navigator.travelDirection);
        float deltaDistance = deltaTime * navigator.travelSpeed * directionFactor;
        navigator.travelProgress += deltaDistance / Length;
        if (navigator.travelProgress < 0f)
        {
            navigator.travelProgress = 0f;
            onSendNavigatorTo.Invoke(firstNode);
        }
        else if (navigator.travelProgress > 1f)
        {
            navigator.travelProgress = 1f;
            onSendNavigatorTo.Invoke(lastNode);
        }
    }

    public override void OnNavigatorEnter(MapNavigator navigator)
    {
        if (navigator == null) return;
        if (navigator.currentLocation == firstNode)
        {
            navigator.travelProgress = 0f;
            navigator.travelDirection = 1;
        }
        else if (navigator.currentLocation == lastNode)
        {
            navigator.travelProgress = 1f;
            navigator.travelDirection = -1;
        }
    }

    public int GetDirectionTo(MapNode node)
    {
        if (node == null) return 0;
        if (node == firstNode) return -1;
        if (node == lastNode) return 1;
        return 0;
    }
}
