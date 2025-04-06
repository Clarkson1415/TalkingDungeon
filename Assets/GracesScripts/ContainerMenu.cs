using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
#nullable enable

public class ContainerMenu : Menu
{
    [SerializeField] GameObject prefabItemButton;
    [SerializeField] List<GameObject> itemButtonLocations;
    List<GameObject> Buttons = new();
    [SerializeField] private GameObject currentShownDescription;
    [SerializeField] private GameObject currentShownName;

    /// <summary>
    /// returns selected and removed item image and item data from the Item slot.
    /// </summary>
    public Item GiveItemToPlayer()
    {
        var selected = this.UIEventSystem.currentSelectedGameObject;

        var itemToReturn = selected.GetComponent<ItemOptionButton>();

        // save item so dont edit the one returning
        var tempItem = itemToReturn;
        
        // edit the old item
        itemToReturn.ReplaceItemWithBlank();

        return tempItem.Item;
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

    private void Awake()
    {
        UpdateItemView();
    }

    private void UpdateItemView()
    {
        this.currentlyShownItem = this.UIEventSystem.currentSelectedGameObject;
        MyGuard.IsNotNull(currentShownDescription);
        MyGuard.IsNotNull(currentShownName);

        currentShownDescription.GetComponentInChildren<ItemDescriptionContainer>().SetDescription("Blank");
        currentShownName.GetComponentInChildren<ItemNameContainer>().SetName("Empty Slot");

        if (currentlyShownItem == null)
        {
            return;
        }

        var newItemButtonComponent = this.currentlyShownItem.TryGetComponent<ItemOptionButton>(out var itemButtonComp);

        if (itemButtonComp.Item != null)
        {
            currentShownDescription.GetComponentInChildren<ItemDescriptionContainer>().SetDescription(itemButtonComp.Item.description);
            currentShownName.GetComponentInChildren<ItemNameContainer>().SetName(itemButtonComp.Item.name);
            return;
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
