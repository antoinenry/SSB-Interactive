using UnityEngine;
using UnityEngine.UI;

public class ChoicePanel : MonoBehaviour
{
    public enum Choice { None, Left, Middle, Right}

    public Slider leftSlider;
    public string leftChoiceName;
    public string leftButtonID;
    public Slider middleSlider;
    public string middleChoiceName;
    public string middleButtonID;
    public Slider rightSlider;
    public string rightChoiceName;
    public string rightButtonID;
    public Choice selected;
    public float scaleEffect;
    public Transform lockedDisplay;
    public ObjectMethodCaller methodCaller = new("AddLeft", "AddMiddle", "AddRight");

    private float leftCounter;
    private float leftStartCount;
    private float middleCounter;
    private float middleStartCount;
    private float rightCounter;
    private float rightStartCount;
    private bool locked;
    private Messenger adminMessenger;

    public bool Locked
    {
        get => locked;
        set { if (value == true) LockChoice(); else UnlockChoice(); }
    }

    public string SelectedChoice
    {
        get
        {
            switch(selected)
            {
                case Choice.Left: return leftChoiceName; 
                case Choice.Middle: return middleChoiceName;
                case Choice.Right: return rightChoiceName;
                default: return "nimp";
            }
        }
    }

    private void Awake()
    {
        adminMessenger = FindObjectOfType<Messenger>(true);
    }

    private void OnEnable()
    {
        leftStartCount = (int)AudienceInput.GetButtonRaw(leftButtonID).Total;
        middleStartCount = (int)AudienceInput.GetButtonRaw(middleButtonID).Total;
        rightStartCount = (int)AudienceInput.GetButtonRaw(rightButtonID).Total;
        UnlockChoice();
    }

    private void OnDisable()
    {
        foreach (Transform child in transform.GetComponentsInChildren<Transform>(true))
            if (child != transform) child.gameObject.SetActive(false);
    }

    public void LockChoice()
    {
        locked = true;
        foreach (Transform child in transform.GetComponentsInChildren<Transform>(true))
            if (child != transform && child != lockedDisplay) child.gameObject.SetActive(false);
        lockedDisplay.gameObject.SetActive(true);
    }

    public void UnlockChoice()
    {
        locked = false;
        foreach (Transform child in transform.GetComponentsInChildren<Transform>(true))
            child.gameObject.SetActive(child.transform != lockedDisplay);
        lockedDisplay.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (locked) return;
        leftCounter = (int)AudienceInput.GetButtonRaw(leftButtonID).Total - leftStartCount;
        middleCounter = (int)AudienceInput.GetButtonRaw(middleButtonID).Total - middleStartCount;
        rightCounter = (int)AudienceInput.GetButtonRaw(rightButtonID).Total - rightStartCount;

        float sum = leftCounter + middleCounter + rightCounter;
        float normalizer = sum != 0f ? 1f / sum : 0f;
        leftSlider.value = leftCounter * normalizer;
        middleSlider.value = middleCounter * normalizer;
        rightSlider.value = rightCounter * normalizer;

        if (leftCounter > middleCounter && leftCounter > rightCounter)
        {
            if (selected != Choice.Left) adminMessenger.Send(leftChoiceName);
            selected = Choice.Left;
            leftSlider.transform.localScale = scaleEffect * Vector3.one;
            middleSlider.transform.localScale = Vector3.one;
            rightSlider.transform.localScale = Vector3.one;
        }
        else if (middleCounter > leftCounter && middleCounter > rightCounter)
        {
            if (selected != Choice.Middle) adminMessenger.Send(middleChoiceName);
            selected = Choice.Middle;
            leftSlider.transform.localScale = Vector3.one;
            middleSlider.transform.localScale = scaleEffect * Vector3.one;
            rightSlider.transform.localScale = Vector3.one;
        }
        else if (rightCounter > leftCounter && rightCounter > middleCounter)
        {
            if (selected != Choice.Right) adminMessenger.Send(rightChoiceName);
            selected = Choice.Right;
            leftSlider.transform.localScale = Vector3.one;
            middleSlider.transform.localScale = Vector3.one;
            rightSlider.transform.localScale = scaleEffect * Vector3.one;
        }
        else selected = Choice.None;
    }

    public void AddLeft() => leftCounter++;
    public void AddMiddle() => middleCounter++;
    public void AddRight() => rightCounter++;
}
