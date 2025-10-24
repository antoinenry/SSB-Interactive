using UnityEngine;
using UnityEngine.Events;

namespace Map
{
    [ExecuteAlways]
    public class MapNavigator : MonoBehaviour
    {
        [Header("Position")]
        public MapNavigationStep currentLocation;
        [Range(0f, 1f)] public float travelProgress = 0f;
        [Header("Motion")]
        [Range(-1, 1)] public int travelDirection = 0;
        public float travelSpeed = 1f;
        public bool pauseNavigation = false;

        public UnityEvent<MapNavigationStep> onNavigatorEnter;
        public UnityEvent<MapNavigationStep> onNavigatorExit;

        public void OnEnable()
        {
            if (currentLocation)
            {
                currentLocation.OnNavigatorComing(this);
                currentLocation.OnNavigatorEnter(this);
                currentLocation.onSendNavigatorTo.AddListener(Enter);
            }
        }

        public void OnDisable()
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
            if (pauseNavigation) return;
            currentLocation?.SetNavigatorPosition(this);
        }

        public void UpdateMotion()
        {
            if (pauseNavigation) return;
            currentLocation?.SetNavigatorMotion(this, Time.deltaTime);
        }

        public void Enter(MapNavigationStep navigationStep)
        {
            if (navigationStep)
            {
                navigationStep.OnNavigatorEnter(this);
                navigationStep.onSendNavigatorTo.AddListener(Enter);
            }
            if (navigationStep != currentLocation)
            {
                Exit(currentLocation);
            }
            onNavigatorEnter.Invoke(navigationStep);
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
            onNavigatorExit.Invoke(navigationStep);
            currentLocation = null;
        }
    }
}