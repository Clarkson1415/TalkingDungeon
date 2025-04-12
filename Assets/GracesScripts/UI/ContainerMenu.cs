using Assets.GracesScripts;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#nullable enable

public class ContainerMenu : Menu
{
    [SerializeField] GameObject prefabItemButton;
    [SerializeField] List<GameObject> itemButtonLocations;
    List<GameObject> Buttons = new();

    [Header("Item UI description")]
    [SerializeField] private Image ItemTypeIndicatorImage;
    [SerializeField] private Sprite specialItemImage;
    [SerializeField] private Sprite ArmourItemImage;
    [SerializeField] private Sprite WeaponItemImage;
    [SerializeField] private GameObject descriptionContainer;
    [SerializeField] private GameObject nameContainer;
    [SerializeField] private TMP_Text powerValue;
    [SerializeField] private TMP_Text defenceValue;

    /// <summary>
    /// returns selected and removed item image and item data from the Item slot.
    /// </summary>
    public ItemOptionButton GetCurrentSelected()
    {
        var selected = this.UIEventSystem.currentSelectedGameObject;

        var itemToReturn = selected.GetComponent<ItemOptionButton>();

        return itemToReturn;
    }

    public void RemoveOldItem(ItemOptionButton itemToRemove)
    {
        itemToRemove.ReplaceItemWithBlank();
    }

    public void PopulateContainer(List<Item> items)
    {
        Buttons.Clear();

        for (int i = 0; i < itemButtonLocations.Count; i++)
        {
            // add all buttons
            var buttonObj = itemButtonLocations[i].GetComponentInChildren<ItemOptionButton>();
            Buttons.Add(buttonObj.gameObject);

            // add items to buttons if there is an item in that slot
            if (i < items.Count)
            {
                buttonObj.SetItemAndImage(items[i]);
            }
        }

        this.UIEventSystem.SetSelectedGameObject(this.Buttons[0]);
        UpdateItemView();
    }

    private void UpdateItemView()
    {
        this.currentlyShownItem = this.UIEventSystem.currentSelectedGameObject;
        MyGuard.IsNotNull(descriptionContainer);
        MyGuard.IsNotNull(nameContainer);

        if (this.currentlyShownItem == null)
        {
            return;
        }

        if (!this.currentlyShownItem.TryGetComponent<ItemOptionButton>(out var itemButtonComp))
        {
            descriptionContainer.GetComponentInChildren<ItemDescriptionContainer>().SetDescription("Blank");
            nameContainer.GetComponentInChildren<ItemNameContainer>().SetName("Empty Slot");

            this.powerValue.text = "0";
            this.defenceValue.text = "0";

            this.ItemTypeIndicatorImage.sprite = emptySlotImage;
            return;

        }

        if (itemButtonComp.Item == null)
        {
            descriptionContainer.GetComponentInChildren<ItemDescriptionContainer>().SetDescription("Blank");
            nameContainer.GetComponentInChildren<ItemNameContainer>().SetName("Empty Slot");

            this.powerValue.text = "0";
            this.defenceValue.text = "0";

            this.ItemTypeIndicatorImage.sprite = emptySlotImage;
            return;
        }

        descriptionContainer.GetComponentInChildren<ItemDescriptionContainer>().SetDescription(itemButtonComp.Item.description);
        nameContainer.GetComponentInChildren<ItemNameContainer>().SetName(itemButtonComp.Item.name);

        this.powerValue.text = itemButtonComp.Item.PowerStat.ToString();
        this.defenceValue.text = itemButtonComp.Item.DefenceStat.ToString();

        Sprite typeSprite = itemButtonComp.Item.Type switch
        {
            ItemType.Weapon => this.WeaponItemImage,
            ItemType.Clothing => this.ArmourItemImage,
            ItemType.SpecialItem => this.specialItemImage,
            _ => throw new ArgumentOutOfRangeException($"no Item Type found {itemButtonComp.Item.Type}")
        };

        this.ItemTypeIndicatorImage.sprite = typeSprite;
    }

    private GameObject? currentlyShownItem;

    // Update is called once per frame
    void Update()
    {
        var highlightedMenuItem = this.UIEventSystem.currentSelectedGameObject;

        // on menu open after another has been open do onece
        if (highlightedMenuItem == null)
        {
            this.UIEventSystem.SetSelectedGameObject(this.Buttons[0]);
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
