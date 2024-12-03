using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class ScrollerPathGenerator : MonoBehaviour
{
    public enum Wrapping { Clamp, Bounce, Loop }

    public Camera followCamera;
    public float rangeCameraScale = 1f;
    public float rangeMin;
    public float rangeMax;
    public float width;
    public Vector2 stepMin;
    public Vector2 stepMax;
    public bool vertical;
    public bool dontChangeDirection;
    public Wrapping wrapMode;
    public bool allowReverse = false;

    private float stepLength;
    private List<Vector2> horizontalPath;
    private Vector2 FirstPointHorizontal => PathLength > 0 ? horizontalPath[0] : Vector2.zero;
    private Vector2 LastPointHorizontal => PathLength > 0 ? horizontalPath[PathLength - 1] : Vector2.zero;

    public int PathLength => horizontalPath != null ? horizontalPath.Count : 0;
    public Vector2[] Path
    {
        get
        {
            if (horizontalPath == null) return new Vector2[0];
            if (vertical) return horizontalPath.ConvertAll(pt => new Vector2(pt.y, pt.x)).ToArray();
            else return horizontalPath.ToArray();

        }
    }

    public float Range => rangeMax - rangeMin;
    public Vector2 Center => vertical ? new Vector2(width / 2f, (rangeMin + rangeMax) / 2f) : new Vector2((rangeMin + rangeMax) / 2f, width / 2f);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 size = vertical ? new Vector3(width, Range, 0f) : new Vector3(Range, width, 0f);
        Gizmos.DrawWireCube(transform.position + (Vector3)Center, size);
        if (PathLength > 0)
        {
            Gizmos.color = Color.green;
            Vector2[] getPath = Path;
            for (int i = 0; i < getPath.Length - 1; i++)
                Gizmos.DrawLine(transform.position + (Vector3)getPath[i], transform.position + (Vector3)getPath[i + 1]);
        }
        
    }

    protected virtual void Update()
    {
        if (followCamera != null)
        {
            if (vertical)
            {
                float cameraHeight = 2f * followCamera.orthographicSize;
                float y = followCamera.transform.position.y;
                if (allowReverse == false) y = Mathf.Max(y, (rangeMin + rangeMax) / 2f);
                rangeMin = y - rangeCameraScale * cameraHeight / 2f;
                rangeMax = y + rangeCameraScale * cameraHeight / 2f;

            }
            else
            {
                float cameraWidth = followCamera.aspect * 2f * followCamera.orthographicSize;
                float x = followCamera.transform.position.x;
                if (allowReverse == false) x = Mathf.Max(x, (rangeMin + rangeMax) / 2f);
                rangeMin = x - rangeCameraScale * cameraWidth / 2f;
                rangeMax = x + rangeCameraScale * cameraWidth / 2f;
            }
        }
        ClearPath();
        GeneratePath();
    }

    protected virtual void OnPathChange() { }

    private Vector2 RandomStep()
    {
        float stepY = Random.Range(stepMin.y, stepMax.y);
        if (dontChangeDirection == false && Random.Range(0, 2) == 1) stepY = -stepY;
        Vector2 step = new Vector2(stepLength, stepY);        
        stepLength = Random.Range(stepMin.x, stepMax.x);
        return step;
    }

    private Vector2 WrapPoint(Vector2 pt)
    {
        switch (wrapMode)
        {
            case Wrapping.Bounce:
                if (pt.y > width) pt.y = 2f * width - pt.y;
                else if (pt.y < 0f) pt.y = -pt.y;
                break;
            case Wrapping.Clamp:
                pt.y = Mathf.Clamp(pt.y, 0f, width);
                break;
            case Wrapping.Loop:
                pt.y = Mathf.Repeat(pt.y, width);
                break;
        }
        return pt;
    }

    private void GeneratePath()
    {
        if (horizontalPath == null)
        {
            horizontalPath = new List<Vector2>() { Vector2.zero };
            stepLength = Random.Range(stepMin.x, stepMax.x);
        }
        // Add points at end of path
        while (rangeMax > LastPointHorizontal.x && stepLength > 0)
        {
            Vector2 newPoint = LastPointHorizontal + RandomStep();
            newPoint = WrapPoint(newPoint);
            horizontalPath.Add(newPoint);
            stepLength = Random.Range(stepMin.x, stepMax.x);
            OnPathChange();
        }
        // Add points at start of path
        while (rangeMin < FirstPointHorizontal.x && stepLength > 0)
        {
            Vector2 newPoint = FirstPointHorizontal - RandomStep();
            newPoint = WrapPoint(newPoint);
            horizontalPath.Insert(0, newPoint);
            OnPathChange();
        }
    }

    public void ClearPath()
    {
        if (horizontalPath != null)
        {
            int pathLength = PathLength;
            // Remove point at end of path
            if (rangeMax + stepMax.x < LastPointHorizontal.x)
            {
                int lastIndex = horizontalPath.FindLastIndex(pt => pt.x < rangeMax + stepMax.x);
                if (lastIndex > 0 && lastIndex < pathLength - 1)
                {
                    stepLength = horizontalPath[lastIndex + 1].x - horizontalPath[lastIndex].x;
                    horizontalPath.RemoveRange(lastIndex + 1, pathLength - lastIndex - 1);
                }
                else
                {
                    horizontalPath.Clear();
                }
                OnPathChange();
            }
            // Remove point at start of path
            else if (rangeMin - stepMax.x > FirstPointHorizontal.x)
            {
                int firstIndex = horizontalPath.FindIndex(pt => pt.x > rangeMin - stepMax.x);
                if (firstIndex >= 0)
                {
                    horizontalPath.RemoveRange(0, firstIndex);
                }
                OnPathChange();
            }
        }
    }
}
