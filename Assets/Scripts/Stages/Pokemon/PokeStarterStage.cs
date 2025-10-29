using UnityEngine;

public class PokeStarterStage : Stage
{
    [Header("Components")]
    public NPCDialog npc;
    public AudienceButtonListenerGroup selector;
    [Header("Contents")]
    public NPCDialogAsset introDialog;
    public NPCDialogAsset outroDialog;

    public override int MomentCount => 3;

    protected override bool HasAllComponents()
    {
        if (base.HasAllComponents() && npc != null && selector != null) return true;
        if (npc == null) npc = GetComponentInChildren<NPCDialog>(true);
        if (selector == null) selector = GetComponentInChildren<AudienceButtonListenerGroup>(true);
        return (base.HasAllComponents() && npc != null && selector != null);
    }

    protected override void OnMomentChange(int value)
    {
        base.OnMomentChange(value);
        switch (value)
        {
            case 0:
                HideSelector();
                ShowDialog(introDialog);
                break;
            case 1:
                HideDialog();
                ShowSelector();
                break;
            case 2:
                HideSelector();
                ShowDialog(outroDialog);
                break;
        }
    }

    private void OnSelectionChange()
    {

    }

    private void OnSelectionMax()
    {
        if (selector) selector.SetButtonsEnabled(false);
    }

    private void ShowDialog(NPCDialogAsset dialogAsset)
    {
        if (npc)
        {
            npc.gameObject.SetActive(true);
            npc.ShowDialogLine(dialogAsset, 0);
        }
    }

    private void HideDialog()
    {
        if (npc) npc.gameObject.SetActive(false);
    }

    private void ShowSelector()
    {
        if (selector)
        {
            selector.gameObject.SetActive(true);
            selector.SetButtonsEnabled(true);
            selector.onRankingChange.AddListener(OnSelectionChange);
            selector.onButtonMaxed.AddListener(OnSelectionMax);
        }
    }

    private void HideSelector()
    {
        if (selector)
        {
            selector.gameObject.SetActive(false);
            selector.SetButtonsEnabled(false);
            selector.onRankingChange.RemoveListener(OnSelectionChange);
            selector.onButtonMaxed.RemoveListener(OnSelectionMax);
        }

    }
}
