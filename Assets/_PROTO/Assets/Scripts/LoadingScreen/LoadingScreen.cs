using System;
using System.IO;
using System.Text.Json;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class LoadingScreen : MonoBehaviour
{
    public string[] tips;
    public int currentTipIndex;
    public float minimumTipDuration;
    public TMP_Text tipsDisplay;
    public TMP_Text instructionDisplay;
    public FloatRange progressDuration;
    public FloatRange progressStep;
    public float inclinaisonSpeed;
    public float maxInclinaison;
    public float gravity;
    public Slider loadingSlider;
    public Slider nextSlider;
    public string nextTipButton = "a";
    [Header("JSON")]
    public bool loadFromFile;
    public bool saveToFile;
    public string fileName = "tips.json";

    private float timer;
    private float timeBeforeProgress;

    static public JsonSerializerOptions JsonOptions => new JsonSerializerOptions()
    {
        WriteIndented = true,

    };

    private void OnValidate()
    {
        if (saveToFile) SaveTips();
        timer = 0f;
    }

    private void Awake()
    {
        LoadTips();
    }

    private void OnEnable()
    {
        loadingSlider.value = 0f;
        nextSlider.value = 0;
    }

    private void Update()
    {
        if (loadFromFile)
        {
            LoadTips();
            saveToFile = false;
        }
        timer += Time.deltaTime;
        if (timer > minimumTipDuration)
        {
            instructionDisplay.enabled = true;
            nextSlider.enabled = true;
            if (AudienceInput.GetButtonRaw(nextTipButton).Velocity > 0f)
            {
                nextSlider.value++;
            }
            if (nextSlider.value >= nextSlider.maxValue)
            {
                NextTip();
                timer = 0f;
                nextSlider.value = 0;
            }
        }
        else
        {
            instructionDisplay.enabled = false;
            nextSlider.enabled = false;
        }
        if (currentTipIndex >= 0 && currentTipIndex < tips.Length)
        {
            tipsDisplay.text = tips[currentTipIndex];
        }
        if (timeBeforeProgress < 0f)
        {
            timeBeforeProgress = progressDuration.RandomValue;
            if (loadingSlider.transform.rotation.z <= 0f)
                loadingSlider.value += progressStep.RandomValue;
        }
        else
        {
            timeBeforeProgress -= Time.deltaTime;
        }
        float horizontalInput = AudienceInput.GetAxis(AudienceInputConfiguration.Axis.Direction.Horizontal);
        if (horizontalInput > 0f && loadingSlider.transform.rotation.z > -maxInclinaison) loadingSlider.transform.rotation *= Quaternion.Euler(0f, 0f, -inclinaisonSpeed * Time.deltaTime);
        else if (horizontalInput < 0f && loadingSlider.transform.rotation.z < maxInclinaison) loadingSlider.transform.rotation *= Quaternion.Euler(0f, 0f, inclinaisonSpeed * Time.deltaTime);
        loadingSlider.value -= loadingSlider.transform.rotation.z * gravity * Time.deltaTime;
    }

    private void SaveTips()
    {
        string dataString = JsonSerializer.Serialize(tips, JsonOptions);
        File.WriteAllText(Application.dataPath + "/" + fileName, dataString);
    }

    private void LoadTips()
    {
        string dataString = File.ReadAllText(Application.dataPath + "/" + fileName);
        object dataObject = JsonSerializer.Deserialize(dataString, typeof(string[]), JsonOptions);
        if (dataObject != null && dataObject is string[]) tips = dataObject as string[];
        currentTipIndex = 0;
    }

    public void NextTip()
    {
        currentTipIndex = (currentTipIndex + 1) % tips.Length;
    }
}
