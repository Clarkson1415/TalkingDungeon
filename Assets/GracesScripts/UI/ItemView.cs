using Assets.GracesScripts.ScriptableObjects;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.GracesScripts.UI
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text powerValueText;
        [SerializeField] private TMP_Text defenceValueText;
        [SerializeField] ItemDescriptionContainer itemdescriptionContainer;
        [SerializeField] ItemNameContainer itemNameContainer;
        [SerializeField] private List<InventorySlot> AbilitySlots;

        private void Awake()
        {
            GrantsAbilitySection.SetActive(true);
        }

        public void SetItemViewToEmptyItem()
        {
            itemdescriptionContainer.SetDescription("Blank");
            itemNameContainer.SetName("Empty Slot");

            this.powerValueText.text = "0";
            this.defenceValueText.text = "0";

            GrantsAbilitySection.SetActive(false);
        }

        public void UpdateItemView(InventorySlot itemSelected)
        {
            if (itemSelected.Ability == null)
            {
                this.SetItemViewToEmptyItem();
            }

            if (itemSelected.Item == null)
            {
                return;
            }

            this.itemdescriptionContainer.SetDescription(itemSelected.Item.description);
            this.itemNameContainer.SetName(itemSelected.Item.name);
            this.defenceValueText.text = "";
            this.powerValueText.text = "";

            if (itemSelected.Item is Weapon weapon)
            {
                MyGuard.IsNotNull(weapon);
                this.itemdescriptionContainer.SetDescription($"{weapon.description}");
                this.defenceValueText.text = weapon.DefenceStat.ToString();
                this.powerValueText.text = weapon.PowerStat.ToString();
                UpdateAbilitySlots(weapon.Abilities);
            }
        }

        [SerializeField] private GameObject GrantsAbilitySection;

        private void UpdateAbilitySlots(List<Ability> abilities)
        {
            GrantsAbilitySection.SetActive(true);

            for (int i = 0; i < abilities.Count; i++)
            {
                this.AbilitySlots[i].gameObject.SetActive(true);
                this.AbilitySlots[i].SetAbilityAndImage(abilities[i]);
            }

            for (int j = abilities.Count; j < this.AbilitySlots.Count; j++)
            {
                this.AbilitySlots[j].gameObject.SetActive(false);
            }
        }
    }
}
