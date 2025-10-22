using System;
using UnityEngine;

public class MapNode : MapNavigator.NavigableMapElement
{
    [Serializable] public struct RoadConnection
    {
        public MapRoad road;
        public AudienceButtonListener button;
    }

    public RoadConnection[] connectedRoads;
}
