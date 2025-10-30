using UnityEngine;
using UnityEngine.Events;

public class PropDrop : MonoBehaviour
{
    public Rigidbody2D[] propPrefabs;
    public int[] chances;
    public float horizontalSpeed;
    public float rotateSpeed;
    public float dropHeightMargin;    

    private Rigidbody2D loadedProp;

    public UnityEvent<Rigidbody2D> onDropProp;

    private void OnEnable()
    {
        if (MiniGameConfig.Current != null)
        {
            MiniGameConfigData.DonkeyKongConfig config = MiniGameConfig.Current.Data.donkeyKong;
            horizontalSpeed = config.horizontalSpeed;
            rotateSpeed = config.rotateSpeed;
        }
    }

    private void OnDisable()
    {
        if (loadedProp != null)
        {
            Destroy(loadedProp.gameObject);
        }
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
            float hInput = AudienceInputSource.Current.GetHorizontalAxis().deltaPresses;
            float vInput = AudienceInputSource.Current.GetVerticalAxis().deltaPresses;
            transform.position += hInput * deltaTime * horizontalSpeed * Vector3.right;
            if (vInput > 0f) loadedProp.transform.rotation *= Quaternion.AngleAxis(rotateSpeed, Vector3.forward);
            else if (vInput < 0f) DropProp();
        }
    }

    private void LoadProp()
    {
        int chanceSum = 0;
        foreach (int c in chances) chanceSum += c;
        float random = Random.Range(0, chanceSum);
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
        loadedProp.simulated = false;
    }

    private void DropProp()
    {
        if (loadedProp != null)
        {
            Rigidbody2D droppedProp = loadedProp;
            loadedProp = null;
            droppedProp.transform.parent = droppedProp.transform.parent.parent;
            droppedProp.simulated = true;
            onDropProp.Invoke(droppedProp);
        }
    }

    public void MoveSpawnHeight(float height)
    {
        Vector3 pos = transform.position;
        pos.y = height + dropHeightMargin;
        transform.position = pos;
    }    
}
