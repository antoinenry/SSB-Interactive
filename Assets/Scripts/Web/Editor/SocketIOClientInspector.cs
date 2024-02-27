using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScriptableSocketIOClient_DogHappy))]
public class SocketClientIOInspector1 : Editor
{
    private ScriptableSocketIOClient_DogHappy targetClient;
    private string inspectorRequest;

    private void OnEnable()
    {
        targetClient = target as ScriptableSocketIOClient_DogHappy;
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
        switch(targetClient.CurrentState)
        {
            case ScriptableSocketIOClient_DogHappy.State.Disconneted:
                if (GUILayout.Button("Connect")) targetClient.Connect();
                break;
            case ScriptableSocketIOClient_DogHappy.State.Connecting:
                if (GUILayout.Button("Cancel connexion")) targetClient.Disconnect();
                break;
            case ScriptableSocketIOClient_DogHappy.State.Connected:
                if (GUILayout.Button("Disconnect")) targetClient.Disconnect();
                break;

        }
        EditorGUILayout.LabelField("State", targetClient.CurrentState.ToString());
        EditorGUILayout.EndVertical();
        // Repaint UI as long as connexion is not completed
        if (targetClient.CurrentState == ScriptableSocketIOClient_DogHappy.State.Connecting || targetClient.CurrentState == ScriptableSocketIOClient_DogHappy.State.Disconnecting)
            EditorUtility.SetDirty(target);
    }
}
