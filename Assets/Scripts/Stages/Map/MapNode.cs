using System;
using System.Collections;
using TMPro;
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
    public TMP_Text labelField;
    [Header("Configuration")]
    [SerializeField] private string label = "Node";
    public bool showLabel = true;
    public float labelAnimationSpeed = 10f;
    public RoadConnection[] connectedRoads;


    public UnityEvent<MapRoad> onSelectRoad;

    private void Awake()
    {
        DisableConnections();
    }
    private void OnEnable()
    {
        showLabel = true;
        StartCoroutine(AnimateLabelCoroutine());
    }

    private void OnDisable()
    {
        showLabel = false;
        StopCoroutine(AnimateLabelCoroutine());
    }

    private void OnValidate()
    {
        if (showLabel) SetLabel(label);
        else SetLabel("");
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
        showLabel = false;
    }

    public override void OnNavigatorExit(MapNavigator navigator)
    {
        DisableConnections();
        showLabel = true;
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

    public void SetLabel(string text)
    {
        labelField?.SetText(text);
    }

    public IEnumerator AnimateLabelCoroutine()
    {
        if (labelField == null) yield break;
        if (labelField.text == null) labelField.SetText("");
        int animatedLabelLength = labelField.text.Length, labelLength = label != null ? label.Length : 0;
        while (true)
        {
            if (labelAnimationSpeed > 0f)
            {
                string animatedLabel = "";
                if (showLabel && animatedLabelLength < labelLength) animatedLabelLength++;
                else if (!showLabel && animatedLabelLength > 0) animatedLabelLength--;
                animatedLabel = "";
                for (int i = 0; i < animatedLabelLength; i++) animatedLabel += label[i];
                SetLabel(animatedLabel);
                yield return new WaitForSeconds(1f / labelAnimationSpeed);
            }
            else
            {
                SetLabel("");
                yield return null;
            }
        }
    }
}
