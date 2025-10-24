using System;
using UnityEngine;
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
            if (button && road)
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
        }
    }

    [Header("Components")]
    public GUIAnimatedText label;
    [Header("Configuration")]
    public string nodeName = "Node";
    public SongInfo song;
    public RoadConnection[] connectedRoads;
    [Header("Web")]
    public HttpRequestLoop songInfoRequest = new(HttpRequest.RequestType.GET, "songs/title/{title}", HttpRequestLoop.ParameterFormat.Path);

    public UnityEvent<MapRoad> onSelectRoad;

    private void OnValidate()
    {
        if (label) label.text = nodeName;
    }

    private void Awake()
    {
        FindSongInfo();
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
        SetLabelVisible(false);
    }

    public override void OnNavigatorExit(MapNavigator navigator)
    {
        DisableConnections();
        SetLabelVisible(true);
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
        ResetButtons();
    }

    public bool IsConnectedTo(MapRoad road)
    {
        if (connectedRoads == null) return false;
        return Array.FindIndex(connectedRoads, c => c != null && c.road == road) != -1;
    }

    public void ResetButtons()
    {
        if (connectedRoads == null) return;
        foreach (RoadConnection road in connectedRoads)
            if (road.button != null) road.button.ResetButton();
    }

    public void SetLabelVisible(bool visible)
    {
        if (label == null) return;
        label.visible = visible;
        if (visible) label.text = nodeName;
    }

    public void FindSongInfo()
    {
        if (songInfoRequest != null)
        {
            songInfoRequest.parameters = new string[] { song.title };
            songInfoRequest.onRequestEnd.AddListener(OnFindSongInfo);
            songInfoRequest.StartRequestCoroutine(this, restart: true);
        }
    }

    private void OnFindSongInfo(HttpRequest request)
    {
        songInfoRequest.onRequestEnd.RemoveListener(OnFindSongInfo);
        if (request.Status == HttpRequest.RequestStatus.Success)
        {
            song = request.DeserializeResponse < SongInfo>();
        }
        else
        {
            Debug.LogWarning("Couldn't find song info for node " + gameObject.name);
        }
    }
}
