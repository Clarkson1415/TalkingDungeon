using Assets.GracesScripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;
#nullable enable

namespace Assets.GracesScripts.UI
{
    /// <summary>
    /// parent class to be inherited by Gear and ItemPages as they have similar behaviour
    /// </summary>
    public abstract class PageWithSlots : BookPage
    {
        [SerializeField] private PlayerInfoPage playerInfoSection;
        [SerializeField] private ItemView itemView;
        [SerializeField] private List<InventorySlot> InventorySlots;
        public abstract Type TypeInPageSlots { get; set; }

        /// <inheritdoc/>
        public override void TogglePageComponents(bool OnOff)
        {
            this.ToggleChildComponents(OnOff);

            this.itemView.gameObject.SetActive(OnOff);
            this.playerInfoSection.gameObject.SetActive(OnOff);

            var SLotsParentGameObject = this.InventorySlots[0].gameObject.transform.parent.gameObject;
            SLotsParentGameObject.SetActive(OnOff);
        }

        /// <summary>
        /// Updates item buttons to current category selected from this.selectedTab. TODO this better by maybe could make booktabs have a enum type that encompasses abilityes and items and have a parent class for abilities and items?
        /// </summary>
        public void FillItemSlots(List<DungeonItem> itemToFillWith, Weapon equippedWeapon, SpecialItem? equippedItem, Weapon DefaultHands)
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
        public void RemoveEquippedWeapon(Weapon WeaponToRemove, Weapon DefaultHands)
        {
            var EquipmentSlot = this.playerInfoSection.equippedWeaponSlot;

            if (EquipmentSlot.Item == null || EquipmentSlot.Item == DefaultHands)
            {
                return;
            }

            this.ToggleEquipGraphicOnInventorySlot(EquipmentSlot.Item, false);
            EquipmentSlot.SetItemAndImage(DefaultHands);
            this.playerInfoSection.UpdateAbilitySlots(DefaultHands);
        }

        public void RemoveEquippedItem(SpecialItem itemToTryRemove)
        {
            var EquipmentSlot = this.playerInfoSection.equippedSpecialItemSlot;

            if (EquipmentSlot.Item == null)
            {
                return;
            }

            this.ToggleEquipGraphicOnInventorySlot(EquipmentSlot.Item, false);
            EquipmentSlot.ReplaceSlotWithBlanks();
        }

        public void EquipWeapon(Weapon weapon)
        {
            this.playerInfoSection.equippedWeaponSlot.SetItemAndImage(weapon);
            this.playerInfoSection.UpdateAbilitySlots(weapon);
            this.ToggleEquipGraphicOnInventorySlot(weapon, true);
        }

        public void EquipSpecialItem(SpecialItem item)
        {
            this.playerInfoSection.equippedSpecialItemSlot.SetItemAndImage(item);
            this.ToggleEquipGraphicOnInventorySlot(item, true);
        }


        /// <summary>
        /// Add an item to its relevant equipped player slot.
        /// </summary>
        /// <param name="newItem"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void InitialSetPlayersEquipped(Weapon weapon, SpecialItem? specialItem, Weapon Hands)
        {
            if (this.playerInfoSection.equippedWeaponSlot.Item != null)
            {
                ToggleEquipGraphicOnInventorySlot(this.playerInfoSection.equippedWeaponSlot.Item, false);
            }

            this.playerInfoSection.equippedWeaponSlot.SetItemAndImage(weapon);
            ToggleEquipGraphicOnInventorySlot(weapon, true);
            ToggleEquipGraphicOnInventorySlot(Hands, true);

            // also update ability slots
            this.playerInfoSection.UpdateAbilitySlots(weapon);

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
        private void ToggleEquipGraphicOnInventorySlot(DungeonItem ItemToMatch, bool OnOff)
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
