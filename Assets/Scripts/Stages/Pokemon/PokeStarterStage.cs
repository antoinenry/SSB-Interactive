using UnityEngine;

public class PokeStarterStage : Stage
{
    [Header("Components")]
    public NPCDialog npc;
    [Header("Contents")]
    public NPCDialogAsset introDialog;
    public NPCDialogAsset outroDialog;
    public GameObject selectionGUI;

    public override int MomentCount => 3;

    protected override void OnMomentChange(int value)
    {
        base.OnMomentChange(value);
        switch (value)
        {
            case 0: ShowIntro(); break;
            case 1: ShowSelection(); break;
            case 2: ShowOutro(); break;
        }
    }

    private void ShowIntro()
    {
        npc.gameObject.SetActive(true);
        npc.ShowDialogLine(setDialog: introDialog, setLineIndex: 0);
        selectionGUI.SetActive(false);
    }

    private void ShowSelection()
    {
        npc.gameObject.SetActive(false);
        selectionGUI.SetActive(true);
    }

    private void ShowOutro()
    {
        npc.gameObject.SetActive(true);
        npc.ShowDialogLine(setDialog: outroDialog, setLineIndex: 0);
        selectionGUI.SetActive(false);
    }
}
