using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AudienceInputConfiguration", menuName = "Config/AudienceInput")]
public class AudienceInputConfiguration : ScriptableObject
{
    [Serializable]
    public struct Button
    {
        public enum Position { Undefined, Top, Bottom, Left, Right };

        public string id;
        public Position position;
        public ButtonValueType type;

        public static Button NoButton => new()
        {
            id = "",
            position = Position.Undefined,
            type = ButtonValueType.Default
        };
    }

    [Serializable]
    public struct Axis
    {
        public enum Direction { Horizontal,  Vertical };

        public string positiveButtonId;
        public string negativeButtonId;
    }

    public Button[] buttons;
    public Axis horizontalAxis;
    public Axis verticallAxis;

    public Button GetButtonConfig(string buttonId)
    {
        if (buttons == null || buttons.Length == 0) return Button.NoButton;
        int buttonIndex = Array.FindIndex(buttons, b => b.id == buttonId);
        return buttonIndex != -1 ? buttons[buttonIndex] : Button.NoButton;
    }

    public Axis GetAxisConfig(Axis.Direction direction)
    {
        switch (direction)
        {
            case Axis.Direction.Horizontal: return horizontalAxis;
            case Axis.Direction.Vertical: return verticallAxis;
            default: return new();
        }
    }
}