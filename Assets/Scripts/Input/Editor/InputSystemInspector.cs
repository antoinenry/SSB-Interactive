using System;
using UnityEditor;

[CustomEditor(typeof(InputSystem))]
public class InputSystemInspector : Editor
{
    private InputSystem targetInputSystem;
    private ButtonCountData[] buttonCounts;

    private void OnEnable()
    {
        targetInputSystem = target as InputSystem;
        targetInputSystem.onInput.AddListener(OnInput);
    }

    private void OnDisable()
    {
        targetInputSystem.onInput.RemoveListener(OnInput);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (buttonCounts != null && buttonCounts.Length > 0)
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("Button ID", "Delta");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUI.indentLevel++;
            foreach (ButtonCountData b in buttonCounts)
                EditorGUILayout.LabelField(b.buttonID.ToString(), b.DeltaInput.ToString());
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
            
    }

    private void OnInput(ButtonCountData[] data)
    {
        foreach (ButtonCountData b in data)
            if (b.DeltaInput != 0)
            {
                buttonCounts = new ButtonCountData[data.Length];
                Array.Copy(data, buttonCounts, data.Length);
                EditorUtility.SetDirty(target);
            }
    }
}
