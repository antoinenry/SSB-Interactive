using UnityEngine;
using static AudienceInputSource;


public static class AudienceInput
{
    public static bool initialized;
    public static AudienceInputSource sourceInstance;

    public static void Init()
    {
        if (sourceInstance == null) sourceInstance = GameObject.FindObjectOfType<AudienceInputSource>();
        if (sourceInstance == null) sourceInstance = new GameObject().AddComponent<AudienceInputSource>();
        initialized = true;
    }

    static public float GetButton(string buttonId)
    {
        if (!initialized) Init();
        return sourceInstance.GetButton(buttonId);
    }

    static public float GetAxis(AudienceInputConfiguration.Axis.Direction direction)
    {
        if (!initialized) Init();
        return sourceInstance.GetAxis(direction);
    }

    static public ButtonInput GetButtonRaw(string buttonId)
    {
        if (!initialized) Init();
        return sourceInstance.GetButtonRaw(buttonId);
    }

    static public ButtonInput GetButtonSmooth(string buttonId)
    {
        if (!initialized) Init();
        return sourceInstance.GetButtonSmooth(buttonId);
    }

    static public float GetButton(string buttonId, ButtonValueType descriptor)
    {
        if (!initialized) Init();
        return sourceInstance.GetButton(buttonId, descriptor);
    }

    static public float GetAxisRaw(string negativeButtonId, string positiveButtonId)
    {
        if (!initialized) Init();
        return sourceInstance.GetAxisRaw(negativeButtonId, positiveButtonId);
    }

    static public float GetAxisSmooth(string negativeButtonId, string positiveButtonId)
    {
        if (!initialized) Init();
        return sourceInstance.GetAxisSmooth(negativeButtonId, positiveButtonId);
    }

    static public float GetAxis(string negativeButtonId, string positiveButtonId, bool smooth)
    {
        if (!initialized) Init();
        return sourceInstance.GetAxis(negativeButtonId, positiveButtonId, smooth);
    }
}