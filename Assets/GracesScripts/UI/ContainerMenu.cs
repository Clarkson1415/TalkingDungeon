using Assets.GracesScripts.UI;
using System.Collections.Generic;
using UnityEngine;
#nullable enable

public class ContainerMenu : MenuWithItemSlots
{
    [Header("Item UI description")]
    [SerializeField] private ItemView itemView;
    [SerializeField] private List<InventorySlot> slots = new();
    [HideInInspector] public bool TellPlayerContainerButtonClicked;

    public void RemoveOldItem(InventorySlot itemToRemove)
    {
        this.itemView.SetItemViewToEmptyItem();
        itemToRemove.ReplaceSlotWithBlanks();
    }

    public void OnContainerButtonClicked()
    {
        TellPlayerContainerButtonClicked = true;
    }

    protected override void UpdateItemView(InventorySlot slot)
    {
        this.itemView.UpdateItemView(slot);
    }

    public void PopulateContainer(List<DungeonItem> items)
    {
        // Could optimise this and FillItemSlots
        this.slots.ForEach(slot => slot.ReplaceSlotWithBlanks());
        slots.ForEach(slot => slot.ToggleEquipGraphic(false));

        for (int i = 0; i < items.Count; i++)
        {
            slots[i].SetItemAndImage(items[i]);
        }

        // Make sure player equip slots match the player equipped items.
        this.itemView.SetItemViewToEmptyItem();
    }
}
