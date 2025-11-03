using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(StageLoader))]
public class StageLoaderInspector : Editor
{
    private StageLoader targetStageLoader;
    private string[] stageOptions;
    private int selectedStageIndex;
    private int momentField = 0;
    private string[] missingScenes;
    private int nullScenes;

    private void OnEnable()
    {
        targetStageLoader = target as StageLoader;
        stageOptions = targetStageLoader.Config?.GetAllLocalNames();
        selectedStageIndex = Array.IndexOf(stageOptions, targetStageLoader.LoadedStage?.name);
        if (stageOptions == null) stageOptions = new string[0];
        momentField = targetStageLoader.LoadedStage != null ? targetStageLoader.LoadedStage.Moment : 0;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();
        selectedStageIndex = EditorGUILayout.Popup("Stage", Array.IndexOf(stageOptions, targetStageLoader.LoadedStage?.name), stageOptions);
        if (EditorGUI.EndChangeCheck()) targetStageLoader.LoadStage(localStageName: stageOptions[selectedStageIndex]);
        if (selectedStageIndex == -1) targetStageLoader.LoadStage(null);

        EditorGUI.BeginChangeCheck();
        momentField = EditorGUILayout.IntField("Moment", targetStageLoader.LoadedMoment);
        if (EditorGUI.EndChangeCheck()) targetStageLoader.LoadedMoment = momentField;

        EditorGUILayout.BeginHorizontal();
        if (targetStageLoader.LoadedStage != null && GUILayout.Button("Unload")) targetStageLoader.LoadStage(null);
        if (targetStageLoader.LoadedStage == null && GUILayout.Button("Reload")) targetStageLoader.LoadStage(localStageName: stageOptions[selectedStageIndex]);
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
