using UnityEngine;
using TMPro;
using System;

[ExecuteAlways]
public class Stage : MonoBehaviour
{
    new public string name;
    public TMP_Text stageNameGUI;
    public bool showInputPanel;

    private void OnValidate() => Load();
    private void OnEnable() => Load();
    private void OnDisable() => Unload();
    private void OnDestroy() => Unload();

    private void CameraSetup(bool? activateOtherCameras = null)
    {
        Camera[] stageCameras = GetComponentsInChildren<Camera>(true);
        Camera[] otherCameras = Array.FindAll(FindObjectsOfType<Camera>(true), c => Array.IndexOf(stageCameras, c) == -1);
        bool activateValue = activateOtherCameras == null ? stageCameras.Length == 0 : activateOtherCameras.Value;
        foreach (Camera c in otherCameras) c.gameObject.SetActive(activateValue);
    }

    public void Load()
    {
        if (stageNameGUI != null) stageNameGUI.text = name;
        MainGUI.ShowInputPanel = showInputPanel;
        CameraSetup();
    }

    public void Unload()
    {
        CameraSetup(activateOtherCameras: true);
    }
}
