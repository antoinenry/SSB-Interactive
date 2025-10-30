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
            bool enableGUI = GUI.enabled;
            GUI.enabled = Application.isPlaying;
            EditorGUILayout.BeginHorizontal("box");
            if (GUILayout.Button("Press")) abl.PressButton();
            EditorGUILayout.LabelField(
                "IN : " + abl.AudienceInputValue + " | " +
                "AUTO : " + abl.AutopressValue + " | " +
                "EXT : " + abl.ExternalInputValue + " | " +
                "OUT : " + abl.OutputValue + " | ",
                EditorStyles.centeredGreyMiniLabel);
            if (GUILayout.Button("Reset")) abl.ResetButton();
            EditorGUILayout.EndHorizontal();
            GUI.enabled = enableGUI;
        }
    }
}
