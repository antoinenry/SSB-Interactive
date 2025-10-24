using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudienceButtonListener))]
public class AudienceButtonListenerInspector : Editor
{
    private AudienceButtonListener abl;

    private void OnEnable()
    {
        abl = (AudienceButtonListener)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (Application.isPlaying)
        {
            EditorGUILayout.BeginHorizontal("box");
            if (GUILayout.Button("Press " + abl.buttonID)) abl.PressButton();
            EditorGUILayout.LabelField("IN : " + abl.InputValue.ToString(), "OUT : " + abl.StaticOutputValue.ToString());
            EditorGUILayout.EndHorizontal();
        }
    }
}
