using Assets.GracesScripts.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
#nullable enable

public class ContainerMenu : MenuWithItemSlots
{
    [Header("Item UI description")]
    [SerializeField] private ItemView itemView;
    [SerializeField] private List<InventorySlot> slots = new();
    [HideInInspector] public bool TellPlayerContainerButtonClicked;
    private ItemContainer? container;
    private PlayerDungeon? player;
    
    /// <summary>
    /// When esc key is pressed.
    /// </summary>
    /// <param name="context"></param>
    public void OnMenuCancel(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }

        MyGuard.IsNotNull(this.container, "Container is null in ContainerMenu.OnMenuCancel");
        this.container.EndInteract();
        this.Close();
    }

    public void RemoveOldItem(InventorySlot itemToRemove)
    {
        this.itemView.SetItemViewToEmptyItem();
        itemToRemove.ReplaceSlotWithBlanks();
    }

    public void OnContainerButtonClicked()
    {
        TellPlayerContainerButtonClicked = true;

        var selected = this.GetSelectedButton();
        var itemOpButton = selected.GetComponent<InventorySlot>();
        itemOpButton.PlaySelectSound();
        if (itemOpButton.Item == null)
        {
            return;
        }

        MyGuard.IsNotNull(this.container, "Container is null");
        MyGuard.IsNotNull(this.player, "player is null");
        container.Loot.Remove(itemOpButton.Item);
        player.Inventory.Add(itemOpButton.Item);
        this.RemoveOldItem(itemOpButton);
    }

    protected override void UpdateItemView(InventorySlot slot)
    {
        this.itemView.UpdateItemView(slot);
    }

    public void SetupMenu(ItemContainer container)
    {
        player = FindObjectOfType<PlayerDungeon>();
        this.container = container;
        List<DungeonItem> items = container.Loot;
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
