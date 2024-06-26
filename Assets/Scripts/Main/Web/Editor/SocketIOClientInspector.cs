using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using SocketIOClient;

[CustomEditor(typeof(SocketIOClientScriptable))]
public class SocketClientIOInspector1 : Editor
{
    private SocketIOClientScriptable targetClient;
    private string eventNameField;
    private string responseLog;
    private UnityAction<string, SocketIOResponse> showResponse;

    private void OnEnable()
    {
        targetClient = target as SocketIOClientScriptable;
        if (showResponse == null) showResponse = new(ShowResponse);
        if (targetClient.Subscriptions != null)
            foreach(string sub in targetClient.Subscriptions)
                targetClient.Subscribe(sub, showResponse);
    }

    private void OnDisable()
    {
        if (showResponse == null) return;
        if (targetClient.Subscriptions != null)
            foreach (string sub in targetClient.Subscriptions)
                targetClient.Subscribe(sub, showResponse);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ConnectionGUI();
        SubscriptionGUI();
        ResponsesGUI();
        EditorUtility.SetDirty(target);
    }

    private void ConnectionGUI()
    {
        EditorGUILayout.LabelField("Connexion", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("State", targetClient.Connection.ToString()) ;
        switch (targetClient.Connection)
        {
            case ConnectionState.Disconnected:
                if (GUILayout.Button("Connect", EditorStyles.miniButtonRight)) targetClient.Connect();
                break;
            case ConnectionState.Connected:
                if (GUILayout.Button("Disconnect", EditorStyles.miniButtonRight)) targetClient.Disconnect();
                break;
            default:
                if (GUILayout.Button("Dispose", EditorStyles.miniButtonRight)) targetClient.Dispose();
                break;
        }
    }

    private void SubscriptionGUI()
    {
        EditorGUILayout.LabelField("Subscriptions", EditorStyles.boldLabel);
        foreach (string sub in targetClient.Subscriptions)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(sub) ;
            if (GUILayout.Button("X", EditorStyles.miniButton)) targetClient.Unsubscribe(sub, showResponse);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.BeginHorizontal();
        eventNameField = EditorGUILayout.TextField(eventNameField);
        if (GUILayout.Button("Subscribe"))
        {
            string eventName = new(eventNameField);
            targetClient.Subscribe(eventNameField, showResponse);
            eventNameField = null;
            GUI.FocusControl(null);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void ResponsesGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Responses", EditorStyles.boldLabel);
        if (responseLog?.Length > 0 && GUILayout.Button("Clear", EditorStyles.miniButton)) responseLog = "";
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.HelpBox(responseLog, MessageType.None);
    }

    private void ShowResponse(string eventName, SocketIOResponse response)
    {
        responseLog += eventName + "\n";
    }
}
