using System;
using UnityEngine;
using UnityEngine.Events;

public class Map : MonoBehaviour
{
    [Header("Components")]
    public GUIAnimatedText currentNodeLabel;
    public AudienceButtonListener validateButton;
    [SerializeField] private MapNavigator navigator;
    [SerializeField] private MapNavigationStep[] layout;
    [Header("Output")]
    [SerializeField] private MapNode currentNode;
    public HttpRequestLoop addSongChoiceRequest = new(HttpRequest.RequestType.POST, "setlists/songs/{setlist_id}/chosen/{song_id}", HttpRequestLoop.ParameterFormat.Path);
    public string choiceMessage = "Morceau choisi : ";

    public UnityEvent onValidateNodeChoice;

    private void Reset()
    {
        navigator = GetComponentInChildren<MapNavigator>(true);
        GetLayoutInChildren();
    }

    private void OnEnable()
    {
        GetLayoutInChildren();
        AddNavigatorListeners();
        InitNavigatorLocation();
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
        if (navigator.currentLocation == null) CurrentNode = null;
        else if (navigator.currentLocation is MapNode) CurrentNode = navigator.currentLocation as MapNode;
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

    public void InitNavigatorLocation()
    {
        if (layout == null) return;
        foreach (MapNavigationStep step in  layout)
        {
            if (step == null) continue;
            if (navigator?.currentLocation == step)
            {
                step.OnNavigatorComing(navigator);
                step.OnNavigatorEnter(navigator);
            }
            else
            {
                step.OnNavigatorExit(navigator);
            }
        }
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
            ShowValidateButton();
        }
        else
        {
            currentNodeLabel.visible = false;
            HideValidateButton();
        }
    }

    private void ShowValidateButton()
    {
        if (validateButton)
        {
            validateButton.gameObject.SetActive(true);
            validateButton.onValueMaxed.AddListener(OnValidateButtonMaxed);
        }
    }

    private void HideValidateButton()
    {
        if (validateButton)
        {
            validateButton.gameObject.SetActive(false);
            validateButton.onValueMaxed.RemoveListener(OnValidateButtonMaxed);
        }
    }

    private void OnValidateButtonMaxed()
    {
        if (validateButton)
        {
            validateButton.ResetButton();
            HideValidateButton();
        }
        ValidateCurrentNode();
    }

    private void ValidateCurrentNode()
    {
        if (currentNode == null) return;
        if (addSongChoiceRequest != null)
        {
            int setlistId = ConcertAdmin.Current != null ? ConcertAdmin.Current.state.setlist.databaseID : -1;
            int songId = currentNode.song.databaseID;
            addSongChoiceRequest.parameters = new string[] { setlistId.ToString(), songId.ToString() };
            addSongChoiceRequest.onRequestEnd.AddListener(OnAddSongChoiceRequestEnd);
            addSongChoiceRequest.StartRequestCoroutine(this, restart: true);
        }
        onValidateNodeChoice.Invoke();
    }

    private void OnAddSongChoiceRequestEnd(HttpRequest request)
    {
        MessengerAdmin.Send(NodeChoiceMessage);
        if (addSongChoiceRequest != null)
            addSongChoiceRequest.onRequestEnd.RemoveListener(OnAddSongChoiceRequestEnd);
    }

    private string NodeChoiceMessage => choiceMessage + currentNode.nodeName;
}
