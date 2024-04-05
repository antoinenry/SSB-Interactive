using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StageLoader))]
public class StageLoaderInspector : Editor
{
    private StageLoader targetStageLoader;
    private string stageField;
    private string[] missingScenes;

    private void OnEnable()
    {
        targetStageLoader = target as StageLoader;
        stageField = targetStageLoader.LoadedStage?.name;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginHorizontal();
        stageField = EditorGUILayout.TextField("Stage", stageField);
        if (GUILayout.Button("Load")) targetStageLoader.LoadStage(stageField);
        if (GUILayout.Button("Unload")) targetStageLoader.LoadStage(null);
        EditorGUILayout.EndHorizontal();
        missingScenes = targetStageLoader.Config?.Data.missingStages;
        if (missingScenes != null)
        {
            if (missingScenes.Length == 0) EditorGUILayout.HelpBox("No missing scenes.", MessageType.Info);
            else EditorGUILayout.HelpBox(missingScenes.Length + " missing scenes.", MessageType.Warning);
        }
    }
}
