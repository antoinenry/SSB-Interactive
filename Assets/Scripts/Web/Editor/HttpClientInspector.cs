using UnityEngine;
using UnityEditor;

// An augmented inspector allowing to test requests directly from Unity Editor
[CustomEditor(typeof(HttpClientScriptable))]
public class HttpClientInspector : Editor
{
    private HttpClientScriptable targetClient;
    private HttpRequest inspectorRequest;

    private void OnEnable()
    {
        targetClient = target as HttpClientScriptable;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // Inspector for testing requests
        RequestInspectorGUI();
    }

    private void RequestInspectorGUI()
    {
        if (inspectorRequest == null) inspectorRequest = new HttpRequest();
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Test Request", EditorStyles.boldLabel);
        // Request status
        EditorGUILayout.LabelField("Status", inspectorRequest.Status.ToString() + " (" + inspectorRequest.Duration + "s)");
        // Configure request
        inspectorRequest.requestUri = EditorGUILayout.TextField("URI", inspectorRequest.requestUri);
        inspectorRequest.type = (HttpRequest.RequestType)EditorGUILayout.EnumPopup("Type", inspectorRequest.type);
        if (inspectorRequest.type == HttpRequest.RequestType.POST)
            inspectorRequest.requestBody = EditorGUILayout.TextField("Body", inspectorRequest.requestBody);
        EditorGUILayout.EndVertical();
        // Buttons to run/cancel request
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Send")) targetClient.SendRequest(inspectorRequest);
        if (GUILayout.Button("Cancel")) inspectorRequest.Cancel();
        EditorGUILayout.EndHorizontal();
        // Response
        if (inspectorRequest.type == HttpRequest.RequestType.GET)
            EditorGUILayout.TextArea(inspectorRequest.ResponseBody);
        // Repaint UI as long as request is not completed
        if (inspectorRequest.Status == HttpRequest.RequestStatus.Running)
            EditorUtility.SetDirty(target);

    }
}
