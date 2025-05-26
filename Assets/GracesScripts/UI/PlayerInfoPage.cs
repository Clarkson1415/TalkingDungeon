using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.GracesScripts.UI
{
    public class PlayerInfoPage : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerPowerStatText;
        [SerializeField] private TMP_Text playerDefenceStatText;

        /// <summary>
        /// This is the text that shows how much / 100 it is. NOT the "/100" bit
        /// </summary>
        [SerializeField] private TMP_Text playerWellBeingText;

        public InventorySlot equippedWeaponSlot;
        public InventorySlot equippedSpecialItemSlot;

        // shows current abilities at a glance.
        [SerializeField] private List<InventorySlot> abilitySlots;

        private PlayerDungeon player;
        private void Awake()
        {
            this.player = FindObjectOfType<PlayerDungeon>();
        }
        public void UpdatePlayerStatsDisplay()
        {
            this.playerPowerStatText.text = player.Power.ToString();
            this.playerDefenceStatText.text = player.Defence.ToString();
            this.playerWellBeingText.text = player.currentHealth.ToString();
        }

        public void ClearAbilitySlots()
        {
            foreach (var ab in this.abilitySlots)
            {
                ab.ReplaceSlotWithBlanks();
            }
        }

        public void UpdateAbilitySlots(List<Ability> abilities)
        {
            this.ClearAbilitySlots();

            for (int i = 0; i < abilities.Count; i++)
            {
                this.abilitySlots[i].gameObject.SetActive(true);
                this.abilitySlots[i].SetAbilityAndImage(abilities[i]);
            }
        }
    }
}
