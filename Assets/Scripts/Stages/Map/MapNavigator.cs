using UnityEngine;

[ExecuteAlways]
public class MapNavigator : MonoBehaviour
{
    public abstract class NavigableMapElement : MonoBehaviour
    {

    }


    public NavigableMapElement currentLocation;
    [Range(0f, 1f)] public float travelProgress = 0f;

    private void Update()
    {
        UpdatePosition();    
    }

    private void UpdatePosition()
    {
        if (currentLocation == null) return;
        if (currentLocation is MapNode)
        {
            transform.position = currentLocation.transform.position;
            travelProgress = 1f;
        }
        else if (currentLocation is MapRoad)
        {
            transform.position = (currentLocation as MapRoad).GetTravelPosition(travelProgress);
        }
    }
}
