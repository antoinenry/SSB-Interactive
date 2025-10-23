using UnityEngine;
using UnityEngine.Events;

public abstract class MapNavigationStep : MonoBehaviour
{
    public UnityEvent<MapNavigationStep> onSendNavigatorTo;

    public abstract void SetNavigatorPosition(MapNavigator navigator);
    public virtual void SetNavigatorMotion(MapNavigator navigator, float deltaTime) { }
    public virtual void OnNavigatorComing(MapNavigator navigator) { }
    public virtual void OnNavigatorEnter(MapNavigator navigator) { }
    public virtual void OnNavigatorExit(MapNavigator navigator) { }
}