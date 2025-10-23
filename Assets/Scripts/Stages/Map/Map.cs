using System;
using UnityEngine;

public class Map : MonoBehaviour
{
    [Header("Components")]
    public GUIAnimatedText currentNodeLabel;
    [SerializeField] private MapNavigator navigator;
    [SerializeField] private MapNavigationStep[] layout;
    [Header("Output")]
    [SerializeField] private MapNode currentNode;

    private void Reset()
    {
        navigator = GetComponentInChildren<MapNavigator>(true);
        GetLayoutInChildren();
    }

    private void OnEnable()
    {
        GetLayoutInChildren();
        AddNavigatorListeners();
    }

    private void OnDisable()
    {
        RemoveNavigatorListeners();
    }

    private void AddNavigatorListeners()
    {
        if (navigator == null) return;
        navigator.onNavigatorEnter.AddListener(OnNavigatorEnter);
        navigator.onNavigatorExit.AddListener(OnNavigatorExit);
    }

    private void RemoveNavigatorListeners()
    {
        if (navigator == null) return;
        navigator.onNavigatorEnter.RemoveListener(OnNavigatorEnter);
        navigator.onNavigatorExit.RemoveListener(OnNavigatorExit);
    }

    public MapNavigator Navigator
    {
        get => navigator;
        set
        {
            RemoveNavigatorListeners();
            navigator = value;
            AddNavigatorListeners();
        }
    }

    public void GetLayoutInChildren()
    {
        // Get all nodes & roads
        MapNode[] nodes = GetComponentsInChildren<MapNode>(true);
        MapRoad[] roads = GetComponentsInChildren<MapRoad>(true);
        layout = new MapNavigationStep[nodes.Length +  roads.Length];
        if (nodes.Length > 0) Array.Copy(nodes, 0, layout, 0, nodes.Length);
        if (roads.Length > 0) Array.Copy(roads, 0, layout, nodes.Length, roads.Length);
        // Set links
        foreach (MapRoad road in roads) road.AutoSetNodes(nodes);
    }

    private void OnNavigatorEnter(MapNavigationStep location)
    {
        if (location != null && location is MapNode) CurrentNode = location as  MapNode;
        else CurrentNode = null;
    }

    private void OnNavigatorExit(MapNavigationStep location)
    {
        if (location != null && location is MapNode) CurrentNode = location as MapNode;
        else CurrentNode = null;
    }

    public MapNode CurrentNode
    {
        get => currentNode;
        private set
        {
            currentNode = value;
            UpdateCurrentNodeLabel();
        }
    }

    private void UpdateCurrentNodeLabel()
    {
        if (currentNodeLabel == null) return;
        if (currentNode != null)
        {
            currentNodeLabel.text = currentNode.nodeName;
            currentNodeLabel.visible = true;
        }
        else
        {
            currentNodeLabel.visible = false;
        }
    }
}
