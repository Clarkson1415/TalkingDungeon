using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
#nullable enable

namespace Assets.GracesScripts.UI
{
    /// <summary>
    /// parent class to be inherited by Gear and ItemPages as they have similar behaviour
    /// </summary>
    public class PageWithSlots : BookPage
    {
        [SerializeField] private PlayerInfo playerInfoSection;
        [SerializeField] private ItemView itemView;
        [SerializeField] private List<InventorySlot> InventorySlots;

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
        public void FillItemSlots(List<Item> itemToFillWith, Item equippedWeapon, Item? equippedItem)
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
            this.UpdatePlayersEquipped(equippedWeapon, equippedItem);
            this.playerInfoSection.UpdatePlayerStatsDisplay();
            this.itemView.SetItemViewToEmptyItem();
        }

        /// <summary>
        /// TODO is not setup? anywhere i can see? but should be used to unequip from player equipped slot. when invenotry item selected that matches an equipped item.
        /// and also UpdatePlayersEquipped should be called somewhere
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void RemoveEquippedItem(Item item)
        {
            InventorySlot oldItem;

            if (item.Type == ItemType.Weapon)
            {
                oldItem = this.playerInfoSection.equippedWeaponSlot;
            }
            else if (item.Type == ItemType.SpecialItem)
            {
                oldItem = this.playerInfoSection.equippedSpecialItemSlot;
            }
            else
            {
                throw new InvalidEnumArgumentException($"item type on {item} not accepted");
            }

            this.ToggleEquipGraphicOnInventorySlot(item, false);
            oldItem.ReplaceSlotWithBlanks();
            oldItem.ToggleEquipGraphic(false);
        }

        public void EquipItem(Item item)
        {
            if (item.Type == ItemType.Weapon)
            {
                this.playerInfoSection.equippedWeaponSlot.SetItemAndImage(item);
                this.playerInfoSection.equippedWeaponSlot.ToggleEquipGraphic(true);
            }
            else if (item.Type == ItemType.SpecialItem)
            {
                this.playerInfoSection.equippedSpecialItemSlot.SetItemAndImage(item);
                this.playerInfoSection.equippedSpecialItemSlot.ToggleEquipGraphic(true);
            }

            this.ToggleEquipGraphicOnInventorySlot(item, true);
        }


        /// <summary>
        /// Add an item to its relevant equipped player slot.
        /// </summary>
        /// <param name="newItem"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void UpdatePlayersEquipped(Item weapon, Item? specialItem)
        {
            if (this.playerInfoSection.equippedWeaponSlot.Item != null)
            {
                ToggleEquipGraphicOnInventorySlot(this.playerInfoSection.equippedWeaponSlot.Item, false);
            }

            this.playerInfoSection.equippedWeaponSlot.SetItemAndImage(weapon);
            this.playerInfoSection.equippedWeaponSlot.ToggleEquipGraphic(true);
            ToggleEquipGraphicOnInventorySlot(weapon, true);

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
                this.playerInfoSection.equippedSpecialItemSlot.ToggleEquipGraphic(true);
                ToggleEquipGraphicOnInventorySlot(specialItem, true);
            }
            else
            {
                this.playerInfoSection.equippedSpecialItemSlot.ToggleEquipGraphic(false);
            }
        }

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
            if (slot.Item != null || slot.Ability != null)
            {
                slot.PlayHighlightOptionChangedSound();
                this.itemView.UpdateItemView(slot);
            }
            else
            {
                this.itemView.SetItemViewToEmptyItem();
            }
        }
    }
}
