using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

[CustomPropertyDrawer(typeof(JsonAssetButtonsAttribute))]
public class StageLoaderConfigInspector : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string[] methods = (attribute as JsonAssetButtonsAttribute).methods;
        int? methodCount = methods?.Length;
        if (methodCount.HasValue && methodCount > 0)
        {
            Rect buttonPosition = position;
            buttonPosition.width = position.width / methodCount.Value;
            foreach (string m in methods)
            {
                if (GUI.Button(buttonPosition, m)) CallMethod(property, m);
                buttonPosition.x += buttonPosition.width;
            }
        }
        else
        {
            EditorGUI.HelpBox(position, "Missing JsonAssetButtonsAttribute methods.", MessageType.Warning);
        }
    }

    private void CallMethod(SerializedProperty property, string methodName)
    {
        Type methodOwnerType = property.serializedObject.targetObject.GetType();
        MethodInfo method = methodOwnerType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (method != null) method.Invoke(property.serializedObject.targetObject, null);
        else Debug.LogWarning(string.Format("Unable to find method {0} in {1}", methodName, methodOwnerType));
    }
}
