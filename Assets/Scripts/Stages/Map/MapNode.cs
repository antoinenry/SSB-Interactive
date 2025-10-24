using System;
using UnityEngine;
using UnityEngine.Events;

namespace Map
{
    public class MapNode : MapNavigationStep
    {
        [Serializable]
        public class RoadConnection
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
        public SpriteRenderer checkMark;
        [Header("Configuration")]
        public string nodeName = "Node";
        public SongInfo song;
        public RoadConnection[] connectedRoads;
        public bool canBeSelected = true;
        [Header("Web")]
        public HttpRequestLoop songInfoRequest = new(HttpRequest.RequestType.GET, "songs/title/{title}", HttpRequestLoop.ParameterFormat.Path);

        public UnityEvent<MapRoad> onSelectRoad;

        private void OnValidate()
        {
            if (label) label.text = nodeName;
            if (checkMark) checkMark.enabled = !canBeSelected;
        }

        private void Awake()
        {
            FindSongInfo();
            canBeSelected = ConcertAdmin.Current.state.setlist.FindSong(s => s.title == song.title) == SongInfo.None;
            if (checkMark) checkMark.enabled = !canBeSelected;
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
            if (canBeSelected == false) TryKickNavigatorOut(navigator);
        }

        public bool TryKickNavigatorOut(MapNavigator navigator)
        {
            if (navigator == null) return false;
            RoadConnection availableRoadConnection = null;
            if (connectedRoads != null) availableRoadConnection = Array.Find(connectedRoads, c => GetConnectedNode(c.road) != null);
            if (availableRoadConnection == null) return false;
            onSendNavigatorTo.Invoke(availableRoadConnection.road);
            return true;
        }

        public MapNode GetConnectedNode(MapRoad road, bool onlyIfCanBeSelected = true)
        {
            MapNode otherNode = road?.GetOtherNode(this);
            if (otherNode != null && (onlyIfCanBeSelected == false || otherNode.canBeSelected)) return otherNode;
            else return null;
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

        public override void EnableConnections()
        {
            if (connectedRoads == null) return;
            foreach (RoadConnection c in connectedRoads)
            {
                MapNode destinationNode = c?.road?.GetOtherNode(this);
                if (destinationNode == null) continue;
                if (destinationNode.canBeSelected)
                {
                    c.Enable();
                    c.onSelectRoad.AddListener(OnSelectRoadConnection);
                }
                else
                {
                    c.Disable();
                }
            }
        }

        public override void DisableConnections()
        {
            if (connectedRoads == null) return;
            foreach (RoadConnection c in connectedRoads)
            {
                c.onSelectRoad.RemoveListener(OnSelectRoadConnection);
                c.Disable();
            }
        }

        public override bool CanBeSelected => canBeSelected;

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
                song = request.DeserializeResponse<SongInfo>();
            }
            else
            {
                Debug.LogWarning("Couldn't find song info for node " + gameObject.name);
            }
        }
    }
}