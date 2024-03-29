using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(JsonAsset), editorForChildClasses:true)]
public class JsonAssetInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load")) (target as JsonAsset).Load();
        if (GUILayout.Button("Save")) (target as JsonAsset).Save();
        EditorGUILayout.EndHorizontal();
    }
}
