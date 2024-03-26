using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InputSystem))]
public class InputSystemInspector : Editor
{
    private InputSystem targetInputSystem;
    private ButtonCountData[] buttonCounts;

    private int ColumnWidth => 80;

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
            EditorGUILayout.LabelField("Button ID", EditorStyles.miniBoldLabel, GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("Total", EditorStyles.miniBoldLabel, GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("Delta", EditorStyles.miniBoldLabel, GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("DeltaTime", EditorStyles.miniBoldLabel, GUILayout.Width(ColumnWidth));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUI.indentLevel++;
            foreach (ButtonCountData b in buttonCounts)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(b.buttonID, EditorStyles.miniLabel, GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(b.totalPresses.ToString(), EditorStyles.miniLabel, GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(b.deltaPresses.ToString(), EditorStyles.miniLabel, GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(b.deltaTime.ToString(), EditorStyles.miniLabel, GUILayout.Width(ColumnWidth));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        Repaint();
        //EditorUtility.SetDirty(target);
    }

    private void OnInput(ButtonCounter counter)
    {
        if (counter?.data == null) return;
        foreach (ButtonCountData b in counter.data)
        {
            buttonCounts = new ButtonCountData[counter.Buttons];
            Array.Copy(counter.data, buttonCounts, counter.Buttons);
        }
    }
}
