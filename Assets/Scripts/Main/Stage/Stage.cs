using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;

[ExecuteAlways]
public class Stage : MonoBehaviour
{
    new public string name;
    public TMP_Text stageNameGUI;
    public bool showInputPanel;
    public string loadMessage = "";
    public UnityEvent onStageEnd;

    private int moment;
    private bool paused;

    protected ConcertAdmin concert;
    protected MessengerAdmin adminMessenger;

    public MiniGameScore Score { get; private set; }
    public CoinCatcher Coins { get; private set; }

    public bool HasScore => Score != null || Coins != null;

    public int Moment
    {
        set
        {
            if (moment != value)
            {
                moment = value;
                OnMomentChange(value);
            }
        }
        get => moment;
    }

    public bool Pause
    {
        get => paused;
        set
        {
            paused = value;
            OnPause(value);
        }
    }

    virtual public int MomentCount => 1;

    virtual protected void Awake()
    {
        Score = GetComponentInChildren<MiniGameScore>(true);
        Coins = GetComponentInChildren<CoinCatcher>(true);
        if (!HasAllComponents()) Debug.LogWarning("Missing components");
    }

    virtual protected void OnEnable() => Load();

    virtual protected void OnDisable() => Unload();

    virtual protected void OnDestroy() => Unload();

    virtual protected bool HasAllComponents()
    {
        if (concert && adminMessenger) return true;
        concert = FindObjectOfType<ConcertAdmin>(true);
        adminMessenger = FindObjectOfType<MessengerAdmin>(true);
        return concert && adminMessenger;
    }

    virtual protected void OnMomentChange(int value)
    {
        if (value >= MomentCount)
        {
            Pause = true;
            onStageEnd.Invoke();
        }
        else
        {
            Pause = false;
        }
    }

    virtual protected void OnPause(bool value)
    {
        Time.timeScale = value ? 0f : 1f;
    }

    private void CameraSetup(bool? activateOtherCameras = null)
    {
        Camera[] stageCameras = GetComponentsInChildren<Camera>(true);
        Camera[] otherCameras = Array.FindAll(FindObjectsOfType<Camera>(true), c => Array.IndexOf(stageCameras, c) == -1);
        bool activateValue = activateOtherCameras == null ? stageCameras.Length == 0 : activateOtherCameras.Value;
        foreach (Camera c in otherCameras) c.enabled = activateValue;
    }

    public void Load()
    {
        MessengerAdmin.Send(loadMessage);
        if (!HasAllComponents()) return;
        if (stageNameGUI != null) stageNameGUI.text = name;
        MainGUI.ShowInputPanel = showInputPanel;
        CameraSetup();
        OnMomentChange(Moment);
    }

    public void Unload()
    {
        CameraSetup(activateOtherCameras: true);
        onStageEnd.RemoveAllListeners();
    }
}
