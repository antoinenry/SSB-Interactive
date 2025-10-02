using System;

[Serializable]
public struct ButtonValueType
{
    public enum ValueType
    {
        Total,
        Delta,
        Velocity
    }

    public ValueType value;
    public bool smooth;

    public ButtonValueType (ValueType value, bool smooth)
    {
        this.value = value;
        this.smooth = smooth;
    }

    public static ButtonValueType Default => new ButtonValueType (ValueType.Total, false);
}