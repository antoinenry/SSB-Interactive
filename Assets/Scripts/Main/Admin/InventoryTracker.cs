using UnityEngine;

public struct InventoryData
{
    public int Money;
}

[CreateAssetMenu(menuName = "Utility/InventoryTracker", fileName = "InventoryTracker")]
public class InventoryTracker : JsonAsset<InventoryData>
{
    public override InventoryData Data { get => data; set => data = value; }
    [SerializeField] private InventoryData data;
    private void OnEnable() => Load();
    private void OnDisable() => Save();
    public void SetMoney(int money)
    {
        Data = new InventoryData
        {
            Money = money
        };
        Save();
    }

    public void Reset()
    {
        Data = new InventoryData
        {
            Money = 5
        };
        Save();
    }
}
