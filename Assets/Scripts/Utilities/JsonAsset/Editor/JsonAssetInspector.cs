using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(JsonAsset), editorForChildClasses:true)]
public class JsonAssetInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        JsonAssetInspectorGUI(target as JsonAsset);
    }

    static public void JsonAssetInspectorGUI(JsonAsset target)
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load")) target.Load();
        if (GUILayout.Button("Save")) target.Save();
        EditorGUILayout.EndHorizontal();
    }
}
