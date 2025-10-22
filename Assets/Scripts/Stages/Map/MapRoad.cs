using System;
using UnityEngine;

[ExecuteAlways]
public class MapRoad : MonoBehaviour
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
}
