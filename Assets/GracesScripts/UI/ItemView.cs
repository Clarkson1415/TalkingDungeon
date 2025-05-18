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
        public void SetItemViewToEmptyItem()
        {
            itemdescriptionContainer.SetDescription("Blank");
            itemNameContainer.SetName("Empty Slot");

            this.powerValueText.text = "0";
            this.defenceValueText.text = "0";
        }
        public void UpdateItemView(InventorySlot itemSelected)
        {
            if (itemSelected.Ability != null)
            {
                this.itemdescriptionContainer.SetDescription(itemSelected.Ability.description);
                this.itemNameContainer.SetName(itemSelected.Ability.name);
                this.defenceValueText.text = 0.ToString();
                this.defenceValueText.text = itemSelected.Ability.attackPower.ToString();
            }
            else if (itemSelected.Item != null)
            {
                this.itemdescriptionContainer.SetDescription(itemSelected.Item.description);
                this.itemNameContainer.SetName(itemSelected.Item.name);
                this.defenceValueText.text = itemSelected.Item.DefenceStat.ToString();
                this.defenceValueText.text = itemSelected.Item.AttackStat.ToString();

                if (itemSelected.Item.Type == ItemType.Weapon)
                {
                    var abilityString = "Grants Abilities: \n ";
                    for (int i = 0; i < itemSelected.Item.Abilities.Count; i++)
                    {
                        abilityString += $"{itemSelected.Item.Abilities[i].name} : \n";
                        abilityString += $"{itemSelected.Item.Abilities[i].description}\n";
                    }

                    this.itemdescriptionContainer.SetDescription($"{itemSelected.Item.description} \n" +
                        $"{abilityString}");
                }
            }
        }
    }
}
