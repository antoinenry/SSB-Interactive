using UnityEngine;

public class MainGUI : MonoBehaviour
{
    static private MainGUI instance;

    public GameObject inputPanel;

    private void Awake()
    {
        instance = this;
    }

    static public MainGUI Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = FindObjectOfType<MainGUI>(true);
            return instance;
        }
    }

    static public bool ShowInputPanel
    {
        get => Instance?.inputPanel != null && Instance.inputPanel.activeInHierarchy;
        set => Instance?.inputPanel?.SetActive(value);
    }
}
