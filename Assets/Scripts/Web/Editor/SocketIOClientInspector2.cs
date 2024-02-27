using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScriptableSocketIOClient_SocketIOSharp))]
public class SocketIOClientInspector2 : Editor
{
    private ScriptableSocketIOClient_SocketIOSharp targetClient;
    private string inspectorRequest;

    private void OnEnable()
    {
        targetClient = target as ScriptableSocketIOClient_SocketIOSharp;
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
        if (GUILayout.Button("Connect")) targetClient.Connect();
        if (GUILayout.Button("Dispose")) targetClient.Dispose();
    }
}
