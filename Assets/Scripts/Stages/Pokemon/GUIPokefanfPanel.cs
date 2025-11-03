using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pokefanf
{
    [ExecuteAlways]
    public class GUIPokefanfPanel : MonoBehaviour
    {
        [Header("Components")]
        public TMP_Text pokeNameField;
        public Slider healthBar;
        public Image healthFill;
        public StringSelectorButton[] attackFields;
        [Header("Content")]
        public Pokefanf pokefanf;
        [Range(0f, 1f)] public float health = .75f;
        [Range(0f, 1f)] public float lowHealth = .2f;
        [Header("Aspect")]
        public float healthBarSpeed = 1f;
        public float lowHealthBlinkRate = 1f;
        public Color normalHealthColor = Color.green;
        public Color lowHealthColor = Color.red;

        private void Reset()
        {
            pokeNameField = GetComponentInChildren<TMP_Text>(true);
            healthBar = GetComponentInChildren<Slider>(true);
        }

        private void Update()
        {
            if (pokeNameField)
            {
                pokeNameField.SetText(pokefanf.pokeName);
            }
            if (healthBar)
            {
                healthBar.value = Application.isPlaying ? Mathf.MoveTowards(healthBar.value, health, Time.deltaTime * healthBarSpeed) : health;
            }
            if (healthFill)
            {
                if (health > lowHealth)
                {
                    healthFill.color = normalHealthColor;
                }
                else
                {
                    if (Application.isPlaying && (int)(2f * Time.time * lowHealthBlinkRate) % 2 == 1) healthFill.color = Color.clear;
                    else healthFill.color = lowHealthColor;
                }
            }
            if (attackFields != null)
            {
                for (int i = 0; i < pokefanf.AttackCount; i++)
                {
                    if (i >= attackFields.Length) break;
                    if (attackFields[i] == null) continue;
                    attackFields[i].Item = pokefanf.attacks[i];
                }
            }
        }
    } }
