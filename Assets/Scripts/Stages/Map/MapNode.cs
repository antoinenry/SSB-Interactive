using System;
using UnityEngine.Events;

public class MapNode : MapNavigationStep
{
    [Serializable] public class RoadConnection
    {
        public MapRoad road;
        public AudienceButtonListener button;
        public UnityEvent<RoadConnection> onSelectRoad;

        public void Enable()
        {
            if (button)
            {
                button.gameObject.SetActive(true);
                button.onValueMaxed.AddListener(OnButtonValueMaxed);
            }
        }

        public void Disable()
        {
            if (button)
            {
                button.onValueMaxed.RemoveListener(OnButtonValueMaxed);
                button.gameObject.SetActive(false);
            }
        }

        private void OnButtonValueMaxed()
        {
            onSelectRoad.Invoke(this);
            button.ResetButton();
        }
    }

    public RoadConnection[] connectedRoads;

    public UnityEvent<MapRoad> onSelectRoad;

    private void Awake()
    {
        DisableConnections();
    }

    public override void SetNavigatorPosition(MapNavigator navigator)
    {
        if (navigator == null) return;
        navigator.transform.position = transform.position;
        navigator.travelProgress = 1f;
        navigator.travelDirection = 0;
    }

    public override void SetNavigatorMotion(MapNavigator navigator, float deltaTime)
    {
        if (navigator == null) return;

    }

    public override void OnNavigatorEnter(MapNavigator navigator)
    {
        navigator.travelDirection = 0;
        navigator.travelProgress = 1f;
        EnableConnections();
    }

    public override void OnNavigatorExit(MapNavigator navigator)
    {
        DisableConnections();
    }

    public void EnableConnections()
    {
        if (connectedRoads == null) return;
        foreach (RoadConnection c in connectedRoads)
        {
            c.Enable();
            c.onSelectRoad.AddListener(OnSelectRoadConnection);
        }
    }

    public void DisableConnections()
    {
        if (connectedRoads == null) return;
        foreach (RoadConnection c in connectedRoads)
        {
            c.onSelectRoad.RemoveListener(OnSelectRoadConnection);
            c.Disable();
        }
    }

    private void OnSelectRoadConnection(RoadConnection connection)
    {        
        onSendNavigatorTo.Invoke(connection.road);
    }
}
