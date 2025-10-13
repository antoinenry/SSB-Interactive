using System;
using System.Collections.Generic;
using SocketIO.Core;
using SocketIOClient;
using SocketIOClient.Transport;

[Serializable]
public struct SocketIOClientOptions
{
    [Serializable] public struct KeyValueCouple { public string key, value; }

    public string path;
    public bool reconnection;
    public int reconnectionAttempts;
    public int reconnectionDelayMs;
    public float reconnectionRandomFactor;
    public int connectionTimeoutMs;
    public KeyValueCouple[] query;
    public EngineIO engineVersion;
    public KeyValueCouple[] extraHeaders;
    public bool httpPolling;
    public bool autoUpgradeToWs;
    public string credentials;

    static public SocketIOClientOptions Default => new SocketIOClientOptions()
    {
        path = "/socket.io",
        reconnection = true,
        reconnectionAttempts = int.MaxValue,
        reconnectionDelayMs = 1000,
        reconnectionRandomFactor = .5f,
        connectionTimeoutMs = 20000,
        query = null,
        engineVersion = EngineIO.V4,
        extraHeaders = null,
        httpPolling = false,
        autoUpgradeToWs = true,
        credentials = null
    };

    public static implicit operator SocketIOOptions(SocketIOClientOptions o) => new SocketIOOptions
    {
        Path = o.path != "" ? o.path : null,
        Reconnection = o.reconnection,
        ReconnectionAttempts = o.reconnectionAttempts,
        ReconnectionDelay = o.reconnectionDelayMs,
        RandomizationFactor = o.reconnectionRandomFactor,
        ConnectionTimeout = new TimeSpan(0, 0, 0, 0, o.connectionTimeoutMs),
        Query = (o.query != null && o.query.Length > 0) ? Array.ConvertAll(o.query, q => new KeyValuePair<string, string>(q.key, q.value)) : null,
        EIO = o.engineVersion,
        ExtraHeaders = (o.extraHeaders != null && o.extraHeaders.Length > 0) ? new Dictionary<string, string>(Array.ConvertAll(o.extraHeaders, q => new KeyValuePair<string, string>(q.key, q.value))) : null,
        Transport = o.httpPolling ? TransportProtocol.Polling : TransportProtocol.WebSocket,
        AutoUpgrade = o.autoUpgradeToWs,
        Auth = o.credentials != "" ? o.credentials : null
    };
}