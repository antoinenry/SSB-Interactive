using UnityEditor;

[CustomEditor(typeof(InputSystem))]
public class InputSystemInspector : Editor
{
    private InputSystem targetInputSystem;
    private ButtonTimeSpawnData[] window;

    private void OnEnable()
    {
        targetInputSystem = target as InputSystem;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.LabelField("Inputs", "(last " + (targetInputSystem.timeWindow) + "s)");
        window = targetInputSystem.ButtonCounts;
        if (window != null)
            foreach (ButtonTimeSpawnData b in window)
            {
                EditorGUILayout.LabelField(b.buttonID, b.maxCount + "(+" + b.DeltaCount + ")");
            }
        Repaint();
    }
}
