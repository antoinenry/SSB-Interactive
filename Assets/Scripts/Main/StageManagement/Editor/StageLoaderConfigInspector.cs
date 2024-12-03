using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageNamingConfig))]
public class StageLoaderConfigInspector : Editor
{
    private StageNamingConfig targetConfig;

    private void OnEnable()
    {
        targetConfig = target as StageNamingConfig;
        targetConfig.FindMissingStages();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Auto complete")) targetConfig.Data.AutoComplete();
        JsonAssetInspector.JsonAssetInspectorGUI(targetConfig);
        string[] missingScenes = targetConfig.Data.missingStages;
        string[] otherStages = targetConfig.Data.otherStages;
        if (missingScenes == null || otherStages == null) targetConfig.FindMissingStages();
        EditorGUILayout.LabelField("Missing stages", EditorStyles.boldLabel);
        if (missingScenes.Length == 0)
            EditorGUILayout.HelpBox("No missing scenes", MessageType.Info);
        else
        {
            EditorGUILayout.BeginVertical("box");
            foreach (string missing in missingScenes) EditorGUILayout.LabelField(missing);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.LabelField("Other stages", EditorStyles.boldLabel);
        if (otherStages.Length == 0)
            EditorGUILayout.HelpBox("No other scenes", MessageType.Info);
        else
        {
            EditorGUILayout.BeginVertical("box");
            foreach (string other in otherStages)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(other);
                if (GUILayout.Button("Add"))
                {
                    targetConfig.AddStage(other, other);
                    targetConfig.FindMissingStages();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        if (GUILayout.Button("Refresh")) targetConfig.FindMissingStages();
    }
}
