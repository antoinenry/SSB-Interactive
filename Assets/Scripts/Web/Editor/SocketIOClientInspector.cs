using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SocketIOClientScriptable))]
public class SocketClientIOInspector1 : Editor
{
    private SocketIOClientScriptable targetClient;
    private string eventNameField;
    private string responseField;

    private void OnEnable()
    {
        targetClient = target as SocketIOClientScriptable;
        targetClient.onReceive.AddListener(OnClientReceive);
    }

    private void OnDisable()
    {
        targetClient.onReceive.RemoveListener(OnClientReceive);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ConnectionGUI();
        SubscriptionGUI();
        ResponsesGUI();
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
                EditorUtility.SetDirty(target);
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
            if (GUILayout.Button("X", EditorStyles.miniButtonRight)) targetClient.Unsubscribe(sub);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.BeginHorizontal();
        eventNameField = EditorGUILayout.TextField(eventNameField);
        if (GUILayout.Button("Subscribe"))
        {
            targetClient.Subscribe(eventNameField);
            eventNameField = null;
            GUI.FocusControl(null);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void ResponsesGUI()
    {
        EditorGUILayout.LabelField("Responses", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(responseField);
        if (GUILayout.Button("Clear")) responseField = "";
    }

    private void EmissionGUI()
    {

    }

    private void OnClientReceive(string eventName)
    {
        responseField += eventName + "\n";
    }
}
