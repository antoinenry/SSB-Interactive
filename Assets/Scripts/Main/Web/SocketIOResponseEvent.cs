using UnityEngine.Events;
using SocketIOClient;
using System;

[Serializable]
public class SocketIOResponseEvent : UnityEvent<string, SocketIOResponse>
{
    readonly public string Name;

    public SocketIOResponseEvent(string name)
    {
        Name = name;
    }

    public void Invoke(SocketIOResponse r) => Invoke(Name, r);
}