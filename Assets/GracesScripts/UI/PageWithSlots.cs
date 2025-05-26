using System;
using System.Collections.Generic;
using UnityEngine;
#nullable enable

namespace Assets.GracesScripts.UI
{
    /// <summary>
    /// parent class to be inherited by Gear and ItemPages as they have similar behaviour
    /// </summary>
    public class PageWithSlots : BookPage
    {
        [SerializeField] private PlayerInfoPage playerInfoSection;
        [SerializeField] private ItemView itemView;
        [SerializeField] private List<InventorySlot> InventorySlots;
        public ItemType ItemType;

        /// <inheritdoc/>
        public override void TogglePageComponents(bool OnOff)
        {
            this.ToggleChildComponents(OnOff);

            this.itemView.gameObject.SetActive(OnOff);
            this.playerInfoSection.gameObject.SetActive(OnOff);

            var SLotsParentGameObject = this.InventorySlots[0].gameObject.transform.parent.gameObject;
            SLotsParentGameObject.SetActive(OnOff);

            Debug.Log("toggle on or off page happened. need to have page content toggle off before page turn and on after.");
            Debug.Log("also need to toggle Tabs lol.");
            Debug.Log("Then also add closing. so disable, flip page the other way, then play close book animation.");
        }

        /// <summary>
        /// Updates item buttons to current category selected from this.selectedTab. TODO this better by maybe could make booktabs have a enum type that encompasses abilityes and items and have a parent class for abilities and items?
        /// </summary>
        public void FillItemSlots(List<Item> itemToFillWith, Item equippedWeapon, Item? equippedItem, Item DefaultHands)
        {
            InventorySlots.ForEach(slot => slot.ReplaceSlotWithBlanks());
            InventorySlots.ForEach(slot => slot.ToggleEquipGraphic(false));

            for (int i = 0; i < itemToFillWith.Count; i++)
            {
                InventorySlots[i].SetItemAndImage(itemToFillWith[i]);

                if (InventorySlots[i].Item == equippedWeapon || InventorySlots[i].Item == equippedItem)
                {
                    InventorySlots[i].ToggleEquipGraphic(true);
                }
            }

            // Make sure player equip slots match the player equipped items.
            this.InitialSetPlayersEquipped(equippedWeapon, equippedItem, DefaultHands);
            this.playerInfoSection.UpdatePlayerStatsDisplay();
            this.itemView.SetItemViewToEmptyItem();
        }

        /// <summary>
        /// Remove equipped Item when inventory slot or the equipment item slot is clicked and there is something in it. Will not remove the default Hands.
        /// </summary>
        /// <param name="WeaponToRemove"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void RemoveEquippedWeapon(Item WeaponToRemove, Item DefaultHands)
        {
            var EquipmentSlot = this.playerInfoSection.equippedWeaponSlot;

            if (EquipmentSlot.Item == null || EquipmentSlot.Item == DefaultHands)
            {
                return;
            }

            this.ToggleEquipGraphicOnInventorySlot(EquipmentSlot.Item, false);
            EquipmentSlot.SetItemAndImage(DefaultHands);
            this.playerInfoSection.UpdateAbilitySlots(DefaultHands.Abilities);
        }

        public void RemoveEquippedItem(Item itemToTryRemove)
        {
            var EquipmentSlot = this.playerInfoSection.equippedSpecialItemSlot;

            if (EquipmentSlot.Item == null)
            {
                return;
            }

            this.ToggleEquipGraphicOnInventorySlot(EquipmentSlot.Item, false);
            EquipmentSlot.ReplaceSlotWithBlanks();
        }

        public void EquipItem(Item item)
        {
            if (item.Type == ItemType.Weapon)
            {
                this.playerInfoSection.equippedWeaponSlot.SetItemAndImage(item);
                this.playerInfoSection.UpdateAbilitySlots(item.Abilities);
            }
            else if (item.Type == ItemType.SpecialItem)
            {
                this.playerInfoSection.equippedSpecialItemSlot.SetItemAndImage(item);
            }

            this.playerInfoSection.UpdateAbilitySlots(item.Abilities);
            this.ToggleEquipGraphicOnInventorySlot(item, true);
        }


        /// <summary>
        /// Add an item to its relevant equipped player slot.
        /// </summary>
        /// <param name="newItem"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void InitialSetPlayersEquipped(Item weapon, Item? specialItem, Item Hands)
        {
            if (this.playerInfoSection.equippedWeaponSlot.Item != null)
            {
                ToggleEquipGraphicOnInventorySlot(this.playerInfoSection.equippedWeaponSlot.Item, false);
            }

            this.playerInfoSection.equippedWeaponSlot.SetItemAndImage(weapon);
            ToggleEquipGraphicOnInventorySlot(weapon, true);
            ToggleEquipGraphicOnInventorySlot(Hands, true);

            // also update ability slots
            this.playerInfoSection.UpdateAbilitySlots(weapon.Abilities);

            // if had item equipped before replacing, remove the equipped highlight from the inventory item
            if (this.playerInfoSection.equippedSpecialItemSlot.Item != null)
            {
                ToggleEquipGraphicOnInventorySlot(this.playerInfoSection.equippedSpecialItemSlot.Item, false);
            }

            if (specialItem != null)
            {
                this.playerInfoSection.equippedSpecialItemSlot.SetItemAndImage(specialItem);
                ToggleEquipGraphicOnInventorySlot(specialItem, true);
            }
        }

        /// <summary>
        /// Toggles the graphic on the corresponding Item in the Inventory slots only.
        /// </summary>
        /// <param name="ItemToMatch"></param>
        /// <param name="OnOff"></param>
        private void ToggleEquipGraphicOnInventorySlot(Item ItemToMatch, bool OnOff)
        {
            foreach (var button in this.InventorySlots)
            {
                var itemOption = button.GetComponentInChildren<InventorySlot>();
                if (ItemToMatch == itemOption.Item)
                {
                    itemOption.ToggleEquipGraphic(OnOff);
                }
            }
        }

        public void UpdateItemView(InventorySlot slot)
        {
            slot.PlayHighlightedSound();

            if (slot.Item != null)
            {
                this.itemView.UpdateItemView(slot);
            }
            else if (slot.Ability != null)
            {
                // don't change item view ability has a tooltip.
            }
            else
            {
                this.itemView.SetItemViewToEmptyItem();
            }
        }
    }
}
