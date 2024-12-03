using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(StageLoader))]
public class StageLoaderInspector : Editor
{
    private StageLoader targetStageLoader;
    private string stageField;
    private string[] missingScenes;
    private int nullScenes;

    private void OnEnable()
    {
        targetStageLoader = target as StageLoader;
        stageField = targetStageLoader.LoadedStage?.name;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        stageField = EditorGUILayout.TextField("Stage", targetStageLoader.LoadedStage != null ? targetStageLoader.LoadedStage.name : stageField);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load")) targetStageLoader.LoadStage(stageField);
        if (GUILayout.Button("Unload")) targetStageLoader.LoadStage(null);
        if (GUILayout.Button(targetStageLoader.Pause ? "Unpause" : "Pause")) targetStageLoader.Pause = !targetStageLoader.Pause;
        EditorGUILayout.EndHorizontal();
        nullScenes = Array.FindAll(targetStageLoader.stages, s => s == null).Length;
        if (nullScenes > 0) EditorGUILayout.HelpBox(nullScenes + " null stages.", MessageType.Warning);
        missingScenes = targetStageLoader.Config?.Data.missingStages;
        if (missingScenes != null)
        {
            if (missingScenes.Length == 0) EditorGUILayout.HelpBox("No missing stages.", MessageType.Info);
            else EditorGUILayout.HelpBox(missingScenes.Length + " missing stages.", MessageType.Warning);
        }
    }
}
