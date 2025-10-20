using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T current;

    public static T Current
    {
        get
        {
            if (current != null) return current;
            T[] instances = FindObjectsOfType<T>(true);
            if (instances.Length == 0) Debug.LogWarning("Missing singleton instance : " + typeof(T).Name);
            else if (instances.Length > 1) Debug.LogWarning("Multiple singleton instances : " + typeof(T).Name);
            else current = instances[0];
            return current;
        }
    }

    public static bool HasInstance => current != null;
}
