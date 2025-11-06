using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(HttpRequestLoop))]
public class HttpRequestLoopDrawer : PropertyDrawer
{
    public bool mainUnfold;
    public bool statusUnfold;
    public int drawerLineCount;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return drawerLineCount * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Label with status
        Rect labelRect = position;
        labelRect.height = EditorGUIUtility.singleLineHeight;
        HttpRequest.RequestStatus status = (HttpRequest.RequestStatus)property.FindPropertyRelative("status").enumValueIndex;
        GUIStyle labelStyle = new(EditorStyles.boldLabel);
        labelStyle.normal.textColor = StatusColor(status);
        EditorGUI.LabelField(labelRect, label.text, "[" + status.ToString() + "]", labelStyle);

        // Drawer foldout
        Rect foldOutRect = position;
        foldOutRect.y = position.y - .5f * (drawerLineCount - 1) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        mainUnfold = EditorGUI.Foldout(foldOutRect, mainUnfold, "");
        drawerLineCount = 1;
        if (mainUnfold == true) UnfoldGUI(position, property);
    }

    private Color StatusColor(HttpRequest.RequestStatus status)
    {
        switch (status)
        {
            case HttpRequest.RequestStatus.Success: return Color.gray * Color.green;
            case HttpRequest.RequestStatus.Failed: return Color.gray * Color.red;
            case HttpRequest.RequestStatus.Running: return Color.yellow;
            default : return Color.white;
        }
    }

    private void UnfoldGUI(Rect position, SerializedProperty property)
    {
        EditorGUI.indentLevel++;

        Rect fieldRect = position;
        fieldRect.height = EditorGUIUtility.singleLineHeight;

        // Request type popup
        AddFieldLine(ref fieldRect);
        SerializedProperty typeProperty = property.FindPropertyRelative("requestType");
        typeProperty.enumValueIndex = (int)(HttpRequest.RequestType)EditorGUI.EnumPopup(fieldRect, "Format", (HttpRequest.RequestType)typeProperty.enumValueIndex);

        // URI field
        AddFieldLine(ref fieldRect);
        SerializedProperty uriProperty = property.FindPropertyRelative("requestUri");
        uriProperty.stringValue = EditorGUI.TextField(fieldRect, "URI", uriProperty.stringValue);

        // Parameter array
        AddFieldLine(ref fieldRect);
        SerializedProperty parametersProperty = property.FindPropertyRelative("parameters");
        EditorGUI.PropertyField(fieldRect, parametersProperty, new("Parameters"), true);
        if (parametersProperty.isExpanded)
        {
            if (parametersProperty.arraySize == 0) AddFieldLine(ref fieldRect);
            for (int i = 0, iend = parametersProperty.arraySize + 2; i < iend; ++i) AddFieldLine(ref fieldRect);
        }

        // Parameter format popup
        if (parametersProperty.arraySize > 0)
        {
            EditorGUI.indentLevel++;
            AddFieldLine(ref fieldRect);
            SerializedProperty formatProperty = property.FindPropertyRelative("parametersFormat");
            formatProperty.enumValueIndex = (int)(HttpRequestLoop.ParameterFormat)EditorGUI.EnumPopup(fieldRect, "Format", (HttpRequestLoop.ParameterFormat)formatProperty.enumValueIndex);
            EditorGUI.indentLevel--;
        }

        // Timeout field
        AddFieldLine(ref fieldRect);
        SerializedProperty requestTimeoutProperty = property.FindPropertyRelative("requestTimeout");
        requestTimeoutProperty.floatValue = EditorGUI.FloatField(fieldRect, "Timeout", requestTimeoutProperty.floatValue);

        // Loop flags
        AddFieldLine(ref fieldRect);
        SerializedProperty loopProperty = property.FindPropertyRelative("loop");
        HttpRequestLoop.LoopBehaviour loopFlags = (HttpRequestLoop.LoopBehaviour)loopProperty.enumValueFlag;
        loopFlags = (HttpRequestLoop.LoopBehaviour)EditorGUI.EnumFlagsField(fieldRect, "Loop", loopFlags);
        loopProperty.enumValueFlag = (int)loopFlags;

        // Loop parameters
        if (loopFlags != 0)
        {
            // Max count
            SerializedProperty maxLoopsProperty = property.FindPropertyRelative("maxLoops");
            if (loopFlags.HasFlag(HttpRequestLoop.LoopBehaviour.InfiniteLoop) == false)
            {
                AddFieldLine(ref fieldRect);
                maxLoopsProperty.intValue = EditorGUI.IntField(fieldRect, "Max loops", maxLoopsProperty.intValue);
            }

            // Duration
            SerializedProperty minLoopDurationProperty = property.FindPropertyRelative("minLoopDuration");
            AddFieldLine(ref fieldRect);
            minLoopDurationProperty.floatValue = EditorGUI.FloatField(fieldRect, "Min Loop Duration", minLoopDurationProperty.floatValue);
        }

        // Detail log
        AddFieldLine(ref fieldRect);
        statusUnfold = EditorGUI.Foldout(fieldRect, statusUnfold, "Detail");
        if (statusUnfold)
        {
            EditorGUI.indentLevel++;

            AddFieldLine(ref fieldRect);
            float responseTime = property.FindPropertyRelative("responseTime").floatValue;
            EditorGUI.LabelField(fieldRect, "Response time :", responseTime.ToString("0.000") + " s");

            AddFieldLine(ref fieldRect);
            int loopCount = property.FindPropertyRelative("loopCount").intValue;
            EditorGUI.LabelField(fieldRect, "Loop count", loopCount.ToString());

            AddFieldLine(ref fieldRect);
            HttpRequestLoop.FailureFlag failure = (HttpRequestLoop.FailureFlag)property.FindPropertyRelative("failureInfo").enumValueFlag;
            EditorGUI.LabelField(fieldRect, "Failure info : ", failure.ToString());

            AddFieldLine(ref fieldRect);
            string responseBody = property.FindPropertyRelative("responseBody").stringValue;
            EditorGUI.TextField(fieldRect, "Response body :", responseBody);

            EditorGUI.indentLevel--;
        }

        EditorGUI.indentLevel--;
    }

    private void AddFieldLine(ref Rect lineRect)
    {
        lineRect.y += lineRect.height + EditorGUIUtility.standardVerticalSpacing;
        drawerLineCount++;
    }
}
