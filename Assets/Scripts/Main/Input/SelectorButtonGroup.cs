using UnityEngine;
using System;
using UnityEngine.Events;

public abstract class SelectorButtonGroup<T> : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] AudienceButtonListenerGroup buttonGroup;
    [SerializeField] SelectorButton<T>[] selectors;
    [Header("Events")]
    public UnityEvent onRankingChange;
    public UnityEvent<T> onSelectorMaxed;

    public T[] RankedItems { get; private set; }

    private void Reset()
    {
        buttonGroup = GetComponentInChildren<AudienceButtonListenerGroup>(true);
        selectors = GetComponentsInChildren<SelectorButton<T>>(true);
    }

    private void OnEnable()
    {
        if (buttonGroup != null)
        {
            buttonGroup.onRankingChange.AddListener(OnRankingChange);
            buttonGroup.SetButtonsEnabled(true);
        }
        if (selectors != null)
        {
            foreach (SelectorButton<T> s in selectors)
            {
                if (s == null) continue;
                s.onItemValueMaxed.AddListener(OnSelectorMaxed);
            }
        }
    }

    private void OnDisable()
    {
        if (buttonGroup != null)
        {
            buttonGroup.onRankingChange.RemoveListener(OnRankingChange);
            buttonGroup.SetButtonsEnabled(false);
        }
        if (selectors != null)
        {
            foreach (SelectorButton<T> s in selectors)
            {
                if (s == null) continue;
                s.onItemValueMaxed.RemoveListener(OnSelectorMaxed);
            }
        }
    }

    private void OnRankingChange()
    {
        int rankCount = SelectorCount;
        if (RankedItems == null || RankedItems.Length != SelectorCount) RankedItems = new T[rankCount];
        else Array.Clear(RankedItems, 0, rankCount);
        int rank = -1;
        for (int i = 0; i < SelectorCount; i++)
        {
            if (selectors[i] == null) continue;
            if (buttonGroup != null) rank = buttonGroup.GetRank(selectors[i].Button);
            if (rank >= 0 && rank < rankCount) RankedItems[rank] = selectors[i].item;
        }
        onRankingChange.Invoke();
    }

    private void OnSelectorMaxed(T item)
    {
        onSelectorMaxed.Invoke(item);
    }

    public int SelectorCount => selectors != null ? selectors.Length : 0;

    public void SetItems(params T[] items)
    {
        int i = 0, itemCount = items != null ? items.Length : 0;
        foreach (SelectorButton<T> s in selectors)
        {
            if (s == null) continue;
            s.item = i < itemCount ? items[i++] : default;
        }
    }

    public T GetLeader()
    {
        if (buttonGroup == null || selectors == null) return default;
        SelectorButton<T> leaderSelector = Array.Find(selectors, s => s.Button == buttonGroup.GetLeaderButton());
        return leaderSelector != null ? leaderSelector.item : default;
    }

    public bool HasWinner() => buttonGroup != null ? buttonGroup.HasWinner() : false;
}