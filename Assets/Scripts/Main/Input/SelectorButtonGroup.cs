using UnityEngine;
using System;

public abstract class SelectorButtonGroup<T> : MonoBehaviour
{
    [SerializeField] AudienceButtonListenerGroup buttonGroup;
    [SerializeField] SelectorButton<T>[] selectors;

    public T[] RankedItems { get; private set; }
    public int SelectorCount => selectors != null ? selectors.Length : 0;

    private void Reset()
    {
        buttonGroup = GetComponentInChildren<AudienceButtonListenerGroup>(true);
        selectors = GetComponentsInChildren<SelectorButton<T>>(true);
    }

    private void OnEnable()
    {
        if (buttonGroup != null)
            buttonGroup.onRankingChange.AddListener(OnRankingChange);
    }

    private void OnDisable()
    {
        if (buttonGroup != null)
            buttonGroup.onRankingChange.RemoveListener(OnRankingChange);
    }

    private void OnRankingChange()
    {
        int rankCount = SelectorCount;
        Array.Clear(RankedItems, 0, rankCount);
        if (RankedItems == null || RankedItems.Length != SelectorCount) RankedItems = new T[rankCount];
        if (buttonGroup == null) return;
        int rank = -1;
        for (int i = 0; i < SelectorCount; i++)
        {
            if (selectors[i] == null) continue;
            rank = buttonGroup.GetRank(selectors[i].Button);
            if (rank >= 0 && rank < rankCount) RankedItems[rank] = selectors[i].item;
        }
    }

    public void SetItems(params T[] items)
    {
        int i = 0, itemCount = items != null ? items.Length : 0;
        foreach (SelectorButton<T> s in selectors)
        {
            if (s == null) continue;
            s.item = i < itemCount ? items[i++] : default;
        }
    }
}