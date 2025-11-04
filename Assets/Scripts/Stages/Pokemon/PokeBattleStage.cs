using NPC;
using System;
using UnityEngine;

namespace Pokefanf
{
    public class PokeBattleStage : Stage
    {
        [Serializable]
        public struct NarrationIndices
        {
            public int call;
            public int askFirstAttack;
            public int firstAttackEffect;
            public int triggerEnemyAttack;
            public int enemyAttackEffect;
            public int askSecondAttack;
            public int secondAttackEffect;
            public int victory;
        }

        [Header("Components")]
        public NPCDialog narrator;
        public GUIPokefanfPanel allyPanelGUI;
        public GUIPokefanfPanel enemyPanelGUI;
        public StringSelectorButtonGroup attackPanel;
        [Header("Content")]
        public NPCDialogContentAsset narration;
        public NarrationIndices narrationIndices;
        [SerializeField] Pokefanf allyPoke;
        [SerializeField] Pokefanf enemyPoke;
        [SerializeField] string currentAttack;
        [Header("Sequence")]
        [Range(0f, 1f)] public float firstAttackDamage = .15f;
        [Range(0f, 1f)] public float enemyAttackDamage = .9f;

        public override int MomentCount => 9;

        protected override bool HasAllComponents()
        {
            return (base.HasAllComponents() && allyPanelGUI != null && enemyPanelGUI != null && attackPanel != null);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            InventoryTracker playerInventory = CurrentAssetsManager.GetCurrent<InventoryTracker>();
            PokeConfigAsset pokeConfig = CurrentAssetsManager.GetCurrent<PokeConfigAsset>();
            SetPokefanfs(
                pokeConfig.GetStarterByPokeName(playerInventory.Data.StarterMusician),
                pokeConfig.GetEnemyMusician(playerInventory.Data.StarterMusician)
            );
            SetCurrentAttack(null);
            if (attackPanel) attackPanel.onSelectorMaxed.AddListener(OnSelectAttack);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (attackPanel) attackPanel.onSelectorMaxed.RemoveListener(OnSelectAttack);
        }

        protected override void OnMomentChange(int value)
        {
            base.OnMomentChange(value);
            switch (value)
            {
                case 0:     //ShowIntroDialog
                    SetGameObjectsActive(false, allyPanelGUI?.gameObject, enemyPanelGUI?.gameObject, attackPanel?.gameObject);
                    ShowNarration(0);
                    break;
                case 1:     //ShowCallDialog
                    SetHealths(1f, 1f);
                    SetGameObjectsActive(true, enemyPanelGUI?.gameObject, attackPanel?.gameObject);
                    SetGameObjectsActive(false, allyPanelGUI?.gameObject);
                    ShowNarration(narrationIndices.call);
                    break;
                case 2:     //AskFirstAttack
                    SetHealths(1f, 1f);
                    SetGameObjectsActive(true, allyPanelGUI?.gameObject, enemyPanelGUI?.gameObject, attackPanel?.gameObject);
                    ShowNarration(narrationIndices.askFirstAttack);
                    break;
                case 3:     //FirstAttackEffect
                    SetHealths(1f, 1f - firstAttackDamage);
                    SetGameObjectsActive(true, allyPanelGUI?.gameObject, enemyPanelGUI?.gameObject);
                    SetGameObjectsActive(false, attackPanel?.gameObject);
                    ShowNarration(narrationIndices.firstAttackEffect);
                    break;
                case 4:     //TriggerEnnemyAttack
                    SetHealths(1f, 1f - firstAttackDamage);
                    SetGameObjectsActive(true, allyPanelGUI?.gameObject, enemyPanelGUI?.gameObject);
                    SetGameObjectsActive(false, attackPanel?.gameObject);
                    SetCurrentAttack(enemyPoke.GetAttack());
                    ShowNarration(narrationIndices.triggerEnemyAttack);
                    break;
                case 5:     //EnnemyAttackEffect
                    SetHealths(1f - enemyAttackDamage, 1f - firstAttackDamage);
                    SetGameObjectsActive(true, allyPanelGUI?.gameObject, enemyPanelGUI?.gameObject);
                    SetGameObjectsActive(false, attackPanel?.gameObject);
                    ShowNarration(narrationIndices.enemyAttackEffect);
                    break;
                case 6:     //AskSecondAttack
                    SetHealths(1f - enemyAttackDamage, 1f - firstAttackDamage);
                    SetGameObjectsActive(true, allyPanelGUI?.gameObject, enemyPanelGUI?.gameObject, attackPanel?.gameObject);
                    ShowNarration(narrationIndices.askSecondAttack);
                    break;
                case 7:     //SecondAttackEffect
                    SetHealths(1f - enemyAttackDamage, 0f);
                    SetGameObjectsActive(true, allyPanelGUI?.gameObject, enemyPanelGUI?.gameObject);
                    SetGameObjectsActive(false, attackPanel?.gameObject);
                    ShowNarration(narrationIndices.secondAttackEffect);
                    break;
                case 8:     //ShowVictoryDialog
                    SetGameObjectsActive(false, allyPanelGUI?.gameObject, enemyPanelGUI?.gameObject, attackPanel?.gameObject);
                    ShowNarration(narrationIndices.victory);
                    break;
            }
        }

        private void SetPokefanfs(Pokefanf ally, Pokefanf enemy)
        {
            allyPoke = ally;
            enemyPoke = enemy;
            if (allyPanelGUI) allyPanelGUI.pokefanf = allyPoke;
            if (enemyPanelGUI) enemyPanelGUI.pokefanf = enemyPoke;
            NPCDialogInjector_Pokefanf injector = NPCDialogInjectorConfig.Current.pokeFanf;
            if (injector == null) return;
            injector.UpdateDictionary(injector.key_AllyPoke, allyPoke.pokeName);
            injector.UpdateDictionary(injector.key_EnemyPoke, enemyPoke.pokeName);
        }

        private void SetHealths(float allyHealth, float enemyHealth)
        {
            if (allyPanelGUI) allyPanelGUI.health = allyHealth;
            if (enemyPanelGUI) enemyPanelGUI.health = enemyHealth;
        }

        private void OnSelectAttack(string attackName)
        {
            SetGameObjectsActive(false, attackPanel?.gameObject);
            SetCurrentAttack(attackName);
            if (narrator) narrator.ShowNextLine();
        }

        private void SetCurrentAttack(string attackName)
        {
            currentAttack = attackName;
            NPCDialogInjector_Pokefanf injector = NPCDialogInjectorConfig.Current.pokeFanf;
            if (injector == null) return;
            injector.UpdateDictionary(injector.key_CurrentAttack, currentAttack);
        }

        private void ShowNarration(int lineIndex)
        {
            if (narrator == null) return;
            narrator.ShowDialogLine(lineIndex);
        }

        private void SetGameObjectsActive(bool active, params GameObject[] gameObjects)
        {
            if (gameObjects == null) return;
            foreach (GameObject go in gameObjects) if (go != null) go.SetActive(active);
        }
    }
}