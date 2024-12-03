using UnityEngine;

[System.Serializable]
public struct FloatRange
{
    public float min;
    public float max;

    public float RandomValue => Random.Range(min, max);
}

[System.Serializable]
public struct FloatRangeDiscrete
{
    public float[] values;

    public float RandomValue => values[Random.Range(0, values.Length)];
}