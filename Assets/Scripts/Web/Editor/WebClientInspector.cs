using UnityEngine;
using UnityEditor;

// An enhanced WebClient inspector allowing to test requests directly form Unity Editor
[CustomEditor(typeof(WebClient))]
public class WebClientInspector : Editor
{
    private WebClient targetClient;
    private WebRequest inspectorRequest;

    private void OnEnable()
    {
        targetClient = target as WebClient;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // Inspector for testing requests
        if (Application.isPlaying) RequestInspectorGUI();
    }

    private void RequestInspectorGUI()
    {
        if (inspectorRequest == null) inspectorRequest = new WebRequest();
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Test Request", EditorStyles.boldLabel);
        // Request status
        EditorGUILayout.LabelField("Status", inspectorRequest.Status.ToString() + " (" + inspectorRequest.Duration + "s)");
        // Configure request
        inspectorRequest.requestUri = EditorGUILayout.TextField("URI", inspectorRequest.requestUri);
        inspectorRequest.type = (WebRequest.RequestType)EditorGUILayout.EnumPopup("Type", inspectorRequest.type);
        if (inspectorRequest.type == WebRequest.RequestType.POST)
            inspectorRequest.requestBody = EditorGUILayout.TextField("Body", inspectorRequest.requestBody);
        EditorGUILayout.EndVertical();
        // Button to run request
        if (GUILayout.Button("Send")) targetClient.SendRequest(inspectorRequest);
        // Response
        if (inspectorRequest.type == WebRequest.RequestType.GET)
            EditorGUILayout.TextArea(inspectorRequest.ResponseBody);
        // Repaint UI as long as request is not completed
        if (inspectorRequest.Status == WebRequest.RequestStatus.Running)
            EditorUtility.SetDirty(target);

    }
}
