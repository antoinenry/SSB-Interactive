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

    public static ButtonValueType RawTotal => new ButtonValueType (ValueType.Total, false);
    public static ButtonValueType RawDelta => new ButtonValueType(ValueType.Delta, false);
    public static ButtonValueType RawVelocity => new ButtonValueType(ValueType.Velocity, false);
    public static ButtonValueType SmoothTotal => new ButtonValueType(ValueType.Total, true);
    public static ButtonValueType SmoothDelta => new ButtonValueType(ValueType.Delta, true);
    public static ButtonValueType SmoothVelocity => new ButtonValueType(ValueType.Velocity, true);
}