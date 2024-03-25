using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StageLoader))]
public class StageLoaderInspector : Editor
{
    private StageLoader targetStageLoader;
    private string stageField;

    private void OnEnable()
    {
        targetStageLoader = target as StageLoader;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (Application.isPlaying)
        {
            EditorGUILayout.BeginHorizontal();
            stageField = EditorGUILayout.TextField("Stage", stageField);
            if (GUILayout.Button("Load"))
            {
                targetStageLoader.Stage = stageField;
                stageField = targetStageLoader.Stage;
            }
            EditorGUILayout.EndHorizontal();
        }      
    }
}
