using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Progress;
#nullable enable

public class ContainerMenu : MonoBehaviour
{
    [SerializeField] GameObject prefabItemButton;
    [SerializeField] GameObject prefabItemDescription;
    [SerializeField] GameObject prefabItemName;
    [SerializeField] List<GameObject> itemButtonLocations;
    [SerializeField] GameObject ItemDescriptionLoc;
    [SerializeField] GameObject ItemNameLoc;
    [SerializeField] EventSystem UIEventSystem;
    List<GameObject> Buttons = new();
    private List<GameObject> Items = new();
    [SerializeField] public Sprite emptySlot;

    public void CreateLootButtons(List<Item> items)
    {
        for(int i = 0; i < itemButtonLocations.Count; i++)
        {
            // add all buttons
            var buttonObj = Instantiate(prefabItemButton, itemButtonLocations[i].transform);
            var itemButton = buttonObj.GetComponent<ItemOptionButton>();
            
            // add items to buttons if there is an item in that slot
            if (i < items.Count)
            {
                itemButton.SetItem(items[i]);
                Buttons.Add(buttonObj);
                Items.Add(buttonObj);

                var spriteImageComponent = buttonObj.gameObject.GetComponentInChildren<ItemOptionButtonImage>();
                spriteImageComponent.SetImage(items[i].image);
            }
        }

        InitialiseItemView(this.Buttons[0]);
        UIEventSystem.SetSelectedGameObject(this.Buttons[0]);
    }

    public void Close()
    {
        this.UIEventSystem.SetSelectedGameObject(null);
    }

    private GameObject? currentShownDescription;
    private GameObject? currentShownName;

    private void InitialiseItemView(GameObject firstSelected)
    {
        this.currentlyShownItem = firstSelected;
        this.currentShownDescription = Instantiate(prefabItemDescription, ItemDescriptionLoc.transform);
        var firstSelectedItem = firstSelected.GetComponent<ItemOptionButton>().Item;
        this.currentShownDescription.GetComponentInChildren<ItemDescriptionContainer>().SetItem(firstSelectedItem.description);
        Items.Add(this.currentShownDescription);

        currentShownName = Instantiate(prefabItemName, ItemNameLoc.transform);
        currentShownName.GetComponentInChildren<ItemNameContainer>().SetItem(firstSelectedItem.Name);
        Items.Add(currentShownName);
    }

    private void UpdateItemView()
    {
        this.currentlyShownItem = this.UIEventSystem.currentSelectedGameObject;
        MyGuard.IsNotNull(currentShownDescription);
        MyGuard.IsNotNull(currentShownName);

        var newItemButtonComponent = this.currentlyShownItem.TryGetComponent<ItemOptionButton>(out var itemButtonComp);

        if (itemButtonComp.Item == null)
        {
            currentShownDescription.GetComponentInChildren<ItemDescriptionContainer>().SetItem("Blank");
            currentShownName.GetComponentInChildren<ItemNameContainer>().SetItem("Empty Slot");
            return;
        }

        currentShownDescription.GetComponentInChildren<ItemDescriptionContainer>().SetItem(itemButtonComp.Item.description);
        currentShownName.GetComponentInChildren<ItemNameContainer>().SetItem(itemButtonComp.Item.name);
    }

    public void ClearItems()
    {
        foreach(var item in Items)
        {
            Destroy(item);
        }
        Items.Clear();

        foreach (var butt in Buttons)
        {
            Destroy(butt);
        }
        Buttons.Clear();
    }

    /// <summary>
    /// returns selected. 
    /// </summary>
    public Item OnButtonSelected()
    {
        var selected = this.UIEventSystem.currentSelectedGameObject;

        selected.GetComponentInChildren<TMP_Text>().text = string.Empty;
        var spriteImageComponent = selected.GetComponentInChildren<ItemOptionButtonImage>();
        spriteImageComponent.SetImage(emptySlot);

        this.UIEventSystem.currentSelectedGameObject.TryGetComponent<ItemOptionButton>(out var itemBut);
        itemBut.RemoveItem();

        return itemBut.Item;
    }

    private GameObject? currentlyShownItem;

    // Update is called once per frame
    void Update()
    {
        var highlightedMenuItem = this.UIEventSystem.currentSelectedGameObject;

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
