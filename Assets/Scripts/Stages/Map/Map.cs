using System;
using UnityEngine;

public class Map : MonoBehaviour
{
    public MapNavigationStep[] steps;

    public ObjectMethodCaller methodCaller = new("UpdateLayout");

    private void OnEnable()
    {
        UpdateLayout();
    }

    public void UpdateLayout()
    {
        // Get all nodes & roads
        MapNode[] nodes = GetComponentsInChildren<MapNode>(true);
        MapRoad[] roads = GetComponentsInChildren<MapRoad>(true);
        steps = new MapNavigationStep[nodes.Length +  roads.Length];
        if (nodes.Length > 0) Array.Copy(nodes, 0, steps, 0, nodes.Length);
        if (roads.Length > 0) Array.Copy(roads, 0, steps, nodes.Length, roads.Length);
        // Set links
        foreach (MapRoad road in roads) road.AutoSetNodes(nodes);
    }
}
