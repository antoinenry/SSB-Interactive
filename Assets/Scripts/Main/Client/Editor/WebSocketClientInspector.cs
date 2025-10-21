using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WebSocketClientScriptable))]
public class WebSocketClientInspector : Editor
{
    private WebSocketClientScriptable targetClient;
    private string inspectorRequest;

    private void OnEnable()
    {
        targetClient = target as WebSocketClientScriptable;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // Inspector for testing requests
        if (Application.isPlaying) RequestInspectorGUI();
        else EditorGUILayout.HelpBox("Enter playmode to test connexion.", MessageType.Info);
    }

    private void RequestInspectorGUI()
    {
        // Connection
        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Connect WebSocket")) targetClient.Connect();
        EditorGUILayout.LabelField("State", targetClient.State.ToString());
        EditorGUILayout.EndVertical();
        // Repaint UI as long as connexion is not completed
        if (targetClient.State == System.Net.WebSockets.WebSocketState.Connecting)
            EditorUtility.SetDirty(target);
        // Display emission and reception controls when connesion is open
        if (targetClient.State == System.Net.WebSockets.WebSocketState.Open)
        {
            // Emission
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Test Emission", EditorStyles.boldLabel);
            inspectorRequest = EditorGUILayout.TextArea(inspectorRequest);
            EditorGUILayout.EndVertical();
            // Buttons to run/cancel request
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Send")) targetClient.Send(inspectorRequest);
            if (GUILayout.Button("Cancel")) targetClient.CancelSend();
            EditorGUILayout.EndHorizontal();
            // Reception
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Test Reception", EditorStyles.boldLabel);
            EditorGUILayout.TextArea("<result>");
            EditorGUILayout.EndVertical();
        }
    }
}
