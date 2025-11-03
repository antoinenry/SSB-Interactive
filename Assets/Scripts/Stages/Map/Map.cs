using System;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;

namespace Map
{
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
        public SocketIOClientScriptable socketClient;
        public string choiceMessage = "Morceau choisi : ";
        private bool lockValidation = false;

        public UnityEvent onValidateNodeChoice;

        private void Reset()
        {
            navigator = GetComponentInChildren<MapNavigator>(true);
            GetLayoutInChildren();
        }

        private void OnEnable()
        {
            if (socketClient == null)
            {
                socketClient = CurrentAssetsManager.GetCurrent<SocketIOClientScriptable>();
            }
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
            if (navigator.currentLocation == null) CurrentNode = null;
            else if (navigator.currentLocation is MapNode) CurrentNode = navigator.currentLocation as MapNode;
        }

        private void RemoveNavigatorListeners()
        {
            if (navigator == null) return;
            navigator.onNavigatorEnter.RemoveListener(OnNavigatorEnter);
            navigator.onNavigatorExit.RemoveListener(OnNavigatorExit);
        }

        public void StartNavigation()
        {
            if (navigator) navigator.pauseNavigation = false;
            InitNavigatorLocation();
        }

        public void PauseNavigation()
        {
            if (navigator)
            {
                navigator.pauseNavigation = true;
                navigator.currentLocation?.DisableConnections();
            }
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
            layout = new MapNavigationStep[nodes.Length + roads.Length];
            if (nodes.Length > 0) Array.Copy(nodes, 0, layout, 0, nodes.Length);
            if (roads.Length > 0) Array.Copy(roads, 0, layout, nodes.Length, roads.Length);
            // Set links
            foreach (MapRoad road in roads) road.AutoSetNodes(nodes);
        }

        public void InitNavigatorLocation()
        {
            if (navigator == null)
                return;
            if (CurrentNode == null && navigator.currentLocation != null && navigator.currentLocation is MapNode) CurrentNode = navigator.currentLocation as MapNode;
            else navigator.currentLocation = CurrentNode;
            if (layout == null)
                return;
            foreach (MapNavigationStep step in layout)
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

        public void GoToLatestPlayedSong()
        {
            if (navigator == null) return;
            MapNode latestNode = GetLatestPlayedSongNode();
            if (latestNode == null) return;
            CurrentNode = latestNode;
        }

        private MapNode GetLatestPlayedSongNode()
        {
            if (layout == null) return null;
            MapNode latestNode = null, candidate = null;
            SetlistInfo currentSetlist = ConcertAdmin.Current.state.setlist;
            int latestIndex = -1, candidateIndex = -1, currentSongPosition = ConcertAdmin.Current.state.songPosition;
            foreach (MapNavigationStep step in layout)
            {
                if (step == null || step is MapNode == false)
                    continue;
                candidate = step as MapNode;
                candidateIndex = currentSetlist.FindIndex(s => s.title == candidate.song.title);
                if (candidateIndex < currentSongPosition && candidateIndex > latestIndex)
                {
                    latestNode = candidate;
                    latestIndex = candidateIndex;
                }
            }
            return latestNode;
        }

        private void OnNavigatorEnter(MapNavigationStep location)
        {
            if (location != null && location is MapNode) CurrentNode = location as MapNode;
            else CurrentNode = null;
        }

        private void OnNavigatorExit(MapNavigationStep location)
        {
            if (CurrentNode == location) CurrentNode = null;
        }

        public MapNode CurrentNode
        {
            get => currentNode;
            set
            {
                currentNode = value;
                if (navigator) navigator.currentLocation = currentNode;
                InitNavigatorLocation();
                UpdateCurrentNodeLabel();
            }
        }

        private void UpdateCurrentNodeLabel()
        {
            if (currentNodeLabel == null) return;
            if (currentNode != null && currentNode.canBeSelected)
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
            PauseNavigation();
            if (socketClient != null && !lockValidation)
            {
                lockValidation = true;
                socketClient.client.EmitAsync("choice", response => { lockValidation = false; }, currentNode.song.databaseID);
                MessengerAdmin.Send(NodeChoiceMessage);
            }
            onValidateNodeChoice.Invoke();
        }

        private string NodeChoiceMessage => choiceMessage + currentNode.nodeName;
    }
}