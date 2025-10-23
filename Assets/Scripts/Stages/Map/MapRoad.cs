using System;
using UnityEngine;

[ExecuteAlways]
public class MapRoad : MapNavigationStep
{
    public MapNode firstNode;
    public MapNode lastNode;
    public bool snapToNodes = true;

    private LineRenderer lineRenderer;
    private Vector2[] waypoints;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        GetWaypointsFromLine();
    }

    private void Update()
    {
        GetWaypointsFromLine();
        if (snapToNodes) SnapToNodes();
    }

    private void GetWaypointsFromLine()
    {
        if (lineRenderer != null)
        {
            int lineLength = lineRenderer.positionCount;
            Vector3[] linePositions = new Vector3[lineLength];
            lineRenderer.GetPositions(linePositions);
            if (PointCount != lineLength) Array.Resize(ref waypoints, lineLength);
            Array.Copy(Array.ConvertAll(linePositions, p => (Vector2)p), waypoints, waypoints.Length);
        }
    }

    public void AutoSetNodes(MapNode[] allNodes)
    {
        firstNode = null;
        lastNode = null;
        if (waypoints == null) return;
        foreach (MapNode node in allNodes)
        {
            if (node.IsConnectedTo(this) == false) continue;
            bool closestToFirstPoint = Vector2.Distance(node.transform.position, waypoints[0]) < Vector2.Distance(node.transform.position, waypoints[PointCount - 1]);
            if (closestToFirstPoint)
            {
                if (firstNode != null) Debug.LogWarning("Two candidates for " + gameObject.name + "'s first node : " + firstNode.gameObject.name + " and " + node.gameObject.name);
                else firstNode = node;
            }
            else
            {
                if (lastNode != null) Debug.LogWarning("Two candidates for " + gameObject.name + "'s last node : " + lastNode.gameObject.name + " and " + node.gameObject.name);
                else lastNode = node;
            }
        }
    }

    private void SnapToNodes()
    {
        if (waypoints == null) waypoints = new Vector2[0];
        if (firstNode != null) SetWaypoint(0, firstNode.transform.position);
        if (lastNode != null) SetWaypoint(PointCount - 1, lastNode.transform.position);
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = PointCount;
            lineRenderer.SetPositions(Array.ConvertAll(waypoints, pt => (Vector3)pt));
        }
    }

    private void SetWaypoint(int index,  Vector3 point)
    {
        if (index == -1) return;
        if (PointCount < index + 1) Array.Resize(ref waypoints, index + 1);
        waypoints[index] = point;
    }

    public int PointCount => waypoints != null ? waypoints.Length : 0;

    public float Length
    {
        get
        {
            float l = 0f;
            for (int i = 0, iend = PointCount - 1; i < iend; i++)
                l += Vector2.Distance(waypoints[i], waypoints[i + 1]);
            return l;
        }
    }

    public Vector2 GetTravelPosition(float t)
    {
        if (t <= 0f) return waypoints[0];
        if (t >= 1f) return waypoints[PointCount - 1];
        float target_distance = t * Length;
        float wayPointDistance = 0f;
        int wayPointIndex = 0;
        for (int i = 0, iend = PointCount - 1; i < iend; i++)
        {
            wayPointDistance += Vector2.Distance(waypoints[i], waypoints[i + 1]);
            if (wayPointDistance >  target_distance)
            {
                wayPointIndex = i;
                break;
            }
        }
        return Vector2.MoveTowards(waypoints[wayPointIndex + 1], waypoints[wayPointIndex], wayPointDistance - target_distance);
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
            if (lastNode != null) lastNode.OnNavigatorComing(navigator);
        }
        else if (navigator.currentLocation == lastNode)
        {
            navigator.travelProgress = 1f;
            navigator.travelDirection = -1;
            if (firstNode != null) firstNode.OnNavigatorComing(navigator);
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
