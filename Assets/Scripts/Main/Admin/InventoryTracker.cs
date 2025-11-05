using System;
using UnityEngine;

[Serializable]
public struct InventoryData
{
    public int Money;
    public string StarterMusician;
}

[CreateAssetMenu(menuName = "Utility/InventoryTracker", fileName = "InventoryTracker")]
public class InventoryTracker : JsonAsset<InventoryData>
{
    private void OnEnable() => Load();
    private void OnDisable() => Save();
    public void SetMoney(int money)
    {
        Data = new InventoryData
        {
            Money = money,
            StarterMusician = Data.StarterMusician
        };
        Save();
    }

    public void SetStarter(string name)
    {
        Data = new InventoryData
        {
            Money = Data.Money,
            StarterMusician = name
        };
        Save();
    }

    public void Reset()
    {
        Data = new InventoryData
        {
            Money = 5,
            StarterMusician = null
        };
        Save();
    }
}
