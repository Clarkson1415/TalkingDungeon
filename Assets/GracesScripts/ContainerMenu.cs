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
    private List<GameObject> GraphicalItems = new();
    [SerializeField] private GameObject currentShownDescription;
    [SerializeField] private GameObject currentShownName;

    public void PopulateContainer(List<Item> items)
    {
        for (int i = 0; i < itemButtonLocations.Count; i++)
        {
            // add all buttons
            var buttonObj = Instantiate(prefabItemButton, itemButtonLocations[i].transform);
            
            Buttons.Add(buttonObj);
            GraphicalItems.Add(buttonObj);
            var spriteImageComponent = buttonObj.gameObject.GetComponentInChildren<ItemOptionButtonImage>();
            spriteImageComponent.SetImage(this.emptySlotImage);

            // add items to buttons if there is an item in that slot
            if (i < items.Count)
            {
                var itemButton = buttonObj.GetComponent<ItemOptionButton>();
                itemButton.SetItem(items[i]);
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

    public void ClearItems()
    {
        foreach (var butt in Buttons)
        {
            Destroy(butt);
        }
        Buttons.Clear();
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
