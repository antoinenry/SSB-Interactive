using UnityEngine;

[ExecuteAlways]
public class MapNavigator : MonoBehaviour
{
    [Header("Position")]
    public MapNavigationStep currentLocation;
    [Range(0f, 1f)] public float travelProgress = 0f;
    [Header("Motion")]
    [Range(-1,1)] public int travelDirection = 0;
    public float travelSpeed = 1f;

    private void OnEnable()
    {
        if (currentLocation)
        {
            currentLocation.OnNavigatorEnter(this);
            currentLocation.onSendNavigatorTo.AddListener(Enter);
        }
    }

    private void OnDisable()
    {
        if (currentLocation)
        {
            currentLocation.onSendNavigatorTo.RemoveListener(Enter);
            currentLocation.OnNavigatorExit(this);
        }
    }

    private void Update()
    {
        if (Application.isPlaying) UpdateMotion();
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        currentLocation?.SetNavigatorPosition(this);
    }

    public void UpdateMotion()
    {
        currentLocation?.SetNavigatorMotion(this, Time.deltaTime);
    }

    public void Enter(MapNavigationStep navigationStep)
    {
        if (navigationStep)
        {
            navigationStep.OnNavigatorEnter(this);
            navigationStep.onSendNavigatorTo.AddListener(Enter);
        }
        if (currentLocation)
        {
            Exit(currentLocation);
        }
        currentLocation = navigationStep;
    }

    public void Exit(MapNavigationStep navigationStep)
    {
        if (navigationStep)
        {
            navigationStep.onSendNavigatorTo.RemoveListener(Enter);
        }
        if (currentLocation != navigationStep) return;
        currentLocation?.OnNavigatorExit(this);
        currentLocation = null;
    }
}
