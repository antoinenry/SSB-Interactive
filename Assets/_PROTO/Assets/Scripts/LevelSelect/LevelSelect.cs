using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public string selectedLevelName;
    public SliderCounter ySlider;
    public string yLevelName;
    public SliderCounter xSlider;
    public string xLevelName;
    public SliderCounter bSlider;
    public string bLevelName;
    public SliderCounter aSlider;
    public string aLevelName;

    //private SceneLoader sceneLoader;
    //private Setlist setlist;
    private int yStartCount;
    private int xStartCount;
    private int bStartCount;
    private int aStartCount;

    private void Awake()
    {
        //sceneLoader = FindObjectOfType<SceneLoader>(true);
        //setlist = FindObjectOfType<Setlist>(true);
    }

    private void OnEnable()
    {
        //yStartCount = InputSystem.TotalUp;
        //xStartCount = InputSystem.TotalLeft;
        //bStartCount = InputSystem.TotalRight;
        //aStartCount = InputSystem.TotalDown;
    }

    private void OnDisable()
    {
        //setlist.nextTitleRequest = "";
    }

    private void Update()
    {
        //ySlider.value = InputSystem.TotalUp - yStartCount;
        //xSlider.value = InputSystem.TotalLeft - xStartCount;
        //bSlider.value = InputSystem.TotalRight - bStartCount;
        //aSlider.value = InputSystem.TotalDown - aStartCount;
        SliderBalance();
        SelectMaxedLevel();
    }

    private void SliderBalance()
    {
        int valueSum = 0;
        valueSum += ySlider.value;
        valueSum += xSlider.value;
        valueSum += bSlider.value;
        valueSum += aSlider.value;
        ySlider.maxValue = valueSum;
        xSlider.maxValue = valueSum;
        bSlider.maxValue = valueSum;
        aSlider.maxValue = valueSum;
    }

    private void SelectMaxedLevel()
    {
        int maxValue = Mathf.Max(ySlider.value, xSlider.value, aSlider.value, bSlider.value);
        string maxedLevelName = "rien";
        if (maxValue == ySlider.value) maxedLevelName = yLevelName;
        else if (maxValue == xSlider.value) maxedLevelName = xLevelName;
        else if (maxValue == aSlider.value) maxedLevelName = aLevelName;
        else if (maxValue == bSlider.value) maxedLevelName = bLevelName;
        if (maxedLevelName != selectedLevelName)
        {
            selectedLevelName = maxedLevelName;
            //sceneLoader.SendAMessage(selectedLevelName);
            //setlist.nextTitleRequest = selectedLevelName;
        }
    }
}
