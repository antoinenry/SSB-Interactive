using UnityEngine;
using UnityEngine.Events;
using System;

public class PropDrop : MonoBehaviour
{
    public Rigidbody2D[] propPrefabs;
    public int[] chances;
    public float horizontalRange = 5f;
    public float horizontalSpeed;
    public float rotateSpeed;
    public float dropHeightMargin;
    public bool randomizeStartTransform = true;

    private Rigidbody2D loadedProp;
    private AudienceButtonListenerGroup buttonGroup;
    private AudienceButtonListener dropButton;

    public UnityEvent<Rigidbody2D> onDropProp;

    private void Awake()
    {
        buttonGroup = GetComponentInChildren<AudienceButtonListenerGroup>(true);
        if (buttonGroup?.buttons != null)
            dropButton = Array.Find(buttonGroup.buttons, b => b.buttonID == AudienceInputSource.Current.verticalAxis.negativeButtonID);
    }

    private void OnEnable()
    {
        if (MiniGameConfig.Current != null)
        {
            MiniGameConfigData.DonkeyKongConfig config = MiniGameConfig.Current.Data.donkeyKong;
            horizontalSpeed = config.horizontalSpeed;
            rotateSpeed = config.rotateSpeed;
        }
        if (dropButton) dropButton.onValueMaxed.AddListener(DropProp);
    }

    private void OnDisable()
    {
        if (loadedProp != null)
        {
            Destroy(loadedProp.gameObject);
        }
        if (dropButton) dropButton.onValueMaxed.RemoveListener(DropProp);
    }

    private void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;
        if (loadedProp == null)
        {
            LoadProp();
        }
        else
        {
            AudienceInputSource inputSource = AudienceInputSource.Current;
            if (inputSource == null) return;
            float horizontalInput = inputSource.GetHorizontalAxis().velocity;
            float rotationInput = inputSource.GetButton(inputSource.verticalAxis.positiveButtonID).velocity;
            if (horizontalInput != 0f) transform.position += horizontalInput * deltaTime * horizontalSpeed * Vector3.right;
            if (rotationInput > 0f) loadedProp.transform.rotation *= Quaternion.AngleAxis(rotationInput * rotateSpeed * Time.fixedDeltaTime, Vector3.forward);
        }
    }

    private void LoadProp()
    {
        int chanceSum = 0;
        foreach (int c in chances) chanceSum += c;
        float random = UnityEngine.Random.Range(0, chanceSum);
        int propIndex = 0;
        chanceSum = 0;
        for (int i = 0; i < propPrefabs.Length && i < chances.Length; i++)
        {
            if (chances[i] == 0) continue;
            chanceSum += chances[i];
            if (random < chanceSum)
            {
                propIndex = i;
                break;
            }
        }
        loadedProp = Instantiate(propPrefabs[propIndex], transform, false);
        if (randomizeStartTransform)
        {
            Vector3 pos = transform.position;
            pos.x = UnityEngine.Random.Range(-horizontalRange, horizontalRange);
            transform.position = pos;
            Vector3 rot = transform.rotation.eulerAngles;
            rot.z = UnityEngine.Random.Range(0f, 360f);
            loadedProp.transform.rotation = Quaternion.Euler(rot);
        }
        loadedProp.simulated = false;
        if (buttonGroup)
        {
            buttonGroup.gameObject.SetActive(true);
        }
    }

    private void DropProp()
    {
        if (loadedProp)
        {
            Rigidbody2D droppedProp = loadedProp;
            loadedProp = null;
            droppedProp.transform.parent = droppedProp.transform.parent.parent;
            droppedProp.simulated = true;
            onDropProp.Invoke(droppedProp);
        }
        if (buttonGroup)
        {
            buttonGroup.gameObject.SetActive(false);
        }
    }

    public void MoveSpawnHeight(float height)
    {
        Vector3 pos = transform.position;
        pos.y = height + dropHeightMargin;
        transform.position = pos;
    }    
}
