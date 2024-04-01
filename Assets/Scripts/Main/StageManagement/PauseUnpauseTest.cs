using UnityEngine;
using SocketIOClient;

public class PauseUnpauseTest : MonoBehaviour
{
    public string pauseEvent = "pause";
    public string resumeEvent = "resume";

    private SocketIOClientScriptable client;
    private SpriteRenderer s;
    private Color c;

    private void Awake()
    {
        s = GetComponent<SpriteRenderer>(); 
        client = CurrentAssetsManager.GetCurrent<SocketIOClientScriptable>();
    }

    private void OnEnable()
    {
        client.Subscribe(pauseEvent, OnClientRequestsPause);
        client.Subscribe(resumeEvent, OnClientRequestsResume);
        if (client.Connection != ConnectionState.Connected) client.Connect();
    }

    private void OnDisable()
    {
        client.Unsubscribe(pauseEvent, OnClientRequestsPause);
        client.Unsubscribe(resumeEvent, OnClientRequestsResume);
    }

    private void Update()
    {
        s.color = new(c.r, c.g, c.b, .5f + .5f * Mathf.Cos(3f * Time.time));
        if (Input.GetKeyDown(KeyCode.P)) Pause();
        if (Input.GetKeyDown(KeyCode.R)) Resume();
    }

    private void OnClientRequestsPause(string eventName, SocketIOResponse response) => Pause();

    private void OnClientRequestsResume(string eventName, SocketIOResponse response) => Resume();

    public void Pause()
    {
        c = Color.red;
    }

    public void Resume()
    {
        c = Color.green;
    }
}
