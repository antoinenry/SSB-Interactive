using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(HttpRequestLoop))]
public class HttpRequestLoopDrawer : PropertyDrawer
{
    private static bool mainUnfold;
    private static bool statusUnfold;
    private int drawerLineCount;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return drawerLineCount * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect foldOutRect = position;
        foldOutRect.y = position.y - .5f * (drawerLineCount - 1) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        mainUnfold = EditorGUI.Foldout(foldOutRect, mainUnfold, label);
        drawerLineCount = 1;
        if (mainUnfold) UnfoldGUI(position, property);
    }

    private void AddFieldLine(ref Rect lineRect)
    {
        lineRect.y += lineRect.height + EditorGUIUtility.standardVerticalSpacing;
        drawerLineCount++;
    }

    private void UnfoldGUI(Rect position, SerializedProperty property)
    {
        EditorGUI.indentLevel++;

        Rect fieldRect = position;
        fieldRect.height = EditorGUIUtility.singleLineHeight;

        AddFieldLine(ref fieldRect);
        SerializedProperty uriProperty = property.FindPropertyRelative("requestUri");
        uriProperty.stringValue = EditorGUI.TextField(fieldRect, "URI", uriProperty.stringValue);

        AddFieldLine(ref fieldRect);
        SerializedProperty requestTimeoutProperty = property.FindPropertyRelative("requestTimeout");
        requestTimeoutProperty.floatValue = EditorGUI.FloatField(fieldRect, "Timeout", requestTimeoutProperty.floatValue);

        AddFieldLine(ref fieldRect);
        SerializedProperty infiniteProperty = property.FindPropertyRelative("infiniteLoops");
        infiniteProperty.boolValue = EditorGUI.Toggle(fieldRect, "Loop Always", infiniteProperty.boolValue);

        SerializedProperty maxLoopsProperty = property.FindPropertyRelative("maxLoops");
        if (infiniteProperty.boolValue == false)
        {
            AddFieldLine(ref fieldRect);
            maxLoopsProperty.intValue = EditorGUI.IntField(fieldRect, "Max loops", maxLoopsProperty.intValue);
        }

        SerializedProperty minLoopDurationProperty = property.FindPropertyRelative("minLoopDuration");
        if (maxLoopsProperty.intValue > 1)
        {
            AddFieldLine(ref fieldRect);
            minLoopDurationProperty.floatValue = EditorGUI.FloatField(fieldRect, "Min Loop Duration", minLoopDurationProperty.floatValue);
        }

        AddFieldLine(ref fieldRect);
        HttpRequest.RequestStatus status = (HttpRequest.RequestStatus)property.FindPropertyRelative("status").enumValueIndex;
        statusUnfold = EditorGUI.Foldout(fieldRect, statusUnfold, "");
        EditorGUI.LabelField(fieldRect, "Status :", status.ToString());

        if (statusUnfold)
        {
            EditorGUI.indentLevel++;

            AddFieldLine(ref fieldRect);
            float responseTime = property.FindPropertyRelative("responseTime").floatValue;
            EditorGUI.LabelField(fieldRect, "Response time :", responseTime.ToString("0.00") + " s");

            AddFieldLine(ref fieldRect);
            int loopCount = property.FindPropertyRelative("loopCount").intValue;
            EditorGUI.LabelField(fieldRect, "Loop count", loopCount.ToString());

            AddFieldLine(ref fieldRect);
            HttpRequestLoop.FailureFlag failure = (HttpRequestLoop.FailureFlag)property.FindPropertyRelative("failureInfo").enumValueFlag;
            EditorGUI.LabelField(fieldRect, "Failure info : ", failure.ToString());

            EditorGUI.indentLevel--;
        }        

        EditorGUI.indentLevel--;
    }
}
