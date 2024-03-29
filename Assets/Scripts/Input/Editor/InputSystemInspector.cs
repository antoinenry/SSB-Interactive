using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InputSystem))]
public class InputSystemInspector : Editor
{
    private InputSystem targetInputSystem;
    private List<ButtonCountDelta> window;

    private void OnEnable()
    {
        targetInputSystem = target as InputSystem;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.LabelField("Inputs", "(last " + (targetInputSystem.timeWindow) + "s)");
        window = targetInputSystem.GetWindow();
        if (window != null)
            foreach (ButtonCountDelta b in window)
            {
                EditorGUILayout.LabelField(b.buttonID, b.maxCount + "(+" + b.DeltaCount + ")");
            }
        Repaint();
    }
}
