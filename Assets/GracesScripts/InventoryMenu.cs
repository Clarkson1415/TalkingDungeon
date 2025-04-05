using Assets.GracesScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Loading;
using UnityEngine;
using UnityEngine.EventSystems;
#nullable enable

public class InventoryMenu : Menu
{
    [SerializeField] GameObject prefabItemButton;

    [SerializeField] List<GameObject> itemButtonLocations;
    [SerializeField] private GameObject equippedWeaponSlot;
    [SerializeField] private GameObject equippedClothingSlot;
    [SerializeField] private GameObject equippedSpecialItemSlot;

    [SerializeField] GameObject ItemDescriptionLoc;
    [SerializeField] GameObject ItemNameLoc;

    [SerializeField] private GameObject currentShownDescription;
    [SerializeField] private GameObject currentShownName;
    List<GameObject> Buttons_NotIncludesEquippedITems = new();

    /// <summary>
    /// TODO: mahbe change so first selected item was the same as when it was last opened. instead of inistialising to button 0?
    /// and this only needs to be initialised once.
    /// </summary>
    /// <param name="Items"></param>
    public void OpenInventory(List<Item> Items)
    {
        // todo instead of clearning juts add any enw items curreently it keeps equipped correct
        Buttons_NotIncludesEquippedITems.Clear();

        for (int i = 0; i < itemButtonLocations.Count; i++)
        {
            // add all buttons of your items 
            // TODO could this be a Menu.cs method instead?
            var itemButtonOld = itemButtonLocations[i].GetComponentInChildren<ItemOptionButton>();
            this.Buttons_NotIncludesEquippedITems.Add(itemButtonOld.gameObject);

            // add items to buttons if there is an item in that slot
            if (i < Items.Count)
            {
                itemButtonOld.SetItemAndImage(Items[i]);
            }
        }

        UIEventSystem.SetSelectedGameObject(this.Buttons_NotIncludesEquippedITems[0]);
        UpdateItemView();
    }

    private void Start()
    {
    }

    public void AddItem(Item item)
    {
        var itemButtonToUpdate = this.Buttons_NotIncludesEquippedITems.Find(x => x.GetComponent<ItemOptionButton>().Item != null);

        itemButtonToUpdate.GetComponent<ItemOptionButton>().SetItemAndImage(item);
    }

    private void UpdateItemView()
    {
        this.currentlyShownItem = this.UIEventSystem.currentSelectedGameObject;
        MyGuard.IsNotNull(currentShownDescription);
        MyGuard.IsNotNull(currentShownName);

        var newItemButtonComponent = this.currentlyShownItem.TryGetComponent<ItemOptionButton>(out var itemButtonComp);

        if (itemButtonComp.Item == null)
        {
            currentShownDescription.GetComponentInChildren<ItemDescriptionContainer>().SetDescription("Blank");
            currentShownName.GetComponentInChildren<ItemNameContainer>().SetName("Empty Slot");
            return;
        }

        currentShownDescription.GetComponentInChildren<ItemDescriptionContainer>().SetDescription(itemButtonComp.Item.description);
        currentShownName.GetComponentInChildren<ItemNameContainer>().SetName(itemButtonComp.Item.name);
    }

    public void RemoveEquippedItem(Item item)
    {
        GameObject equipmentSlot = item.Type switch
        {
            ItemType.Weapon => this.equippedWeaponSlot,
            ItemType.Clothing => this.equippedClothingSlot,
            ItemType.SpecialItem => this.equippedSpecialItemSlot,
            _ => throw new ArgumentOutOfRangeException($"no Item Type found {item.Type.ToString()}")
        };

        var oldItem = equipmentSlot.GetComponentInChildren<ItemOptionButton>();
        if (oldItem != null)
        {
            Destroy(oldItem.gameObject);
        }
    }

    /// <summary>
    /// TODO dont instantiate new one. add one in inspector then update the item instead.
    /// </summary>
    /// <param name="newItem"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void AddOrReplaceEquipped(Item newItem)
    {
        GameObject equipmentSlot = newItem.Type switch
        {
            ItemType.Weapon => this.equippedWeaponSlot,
            ItemType.Clothing => this.equippedClothingSlot,
            ItemType.SpecialItem => this.equippedSpecialItemSlot,
            _ => throw new ArgumentOutOfRangeException($"no Item Type found {newItem.Type.ToString()}")
        };

        var currentEquipped = equipmentSlot.GetComponentInChildren<ItemOptionButton>();
        if (currentEquipped != null)
        {
            // deselect old item UI Highlight
            foreach(var button in this.Buttons_NotIncludesEquippedITems)
            {
                var itemOption = button.GetComponentInChildren<ItemOptionButton>();
                if(itemOption.Item == currentEquipped.Item)
                {
                    itemOption.ToggleEquipGraphic();
                }
            }

            currentEquipped.SetItemAndImage(newItem);
        }
        else
        {
            var buttonObj = Instantiate(prefabItemButton, equipmentSlot.transform);
            var itemButton = buttonObj.GetComponent<ItemOptionButton>();
            itemButton.SetItemAndImage(newItem);
        }
    }

    private GameObject? currentlyShownItem;

    // Update is called once per frame
    void Update()
    {
        var highlightedMenuItem = this.UIEventSystem.currentSelectedGameObject;

        // on menu open after another has been open do onece
        if (highlightedMenuItem == null)
        {
            this.UIEventSystem.SetSelectedGameObject(this.Buttons_NotIncludesEquippedITems[0]);
            UpdateItemView();
            return;
        }

        // when it is open do this
        if (highlightedMenuItem != currentlyShownItem && currentlyShownItem != null)
        {
            if (highlightedMenuItem.TryGetComponent<ItemOptionButton>(out var button))
            {
                button.PlayHighlightOptionChangedSound();
            }
            this.UpdateItemView();
        }
    }
}
