using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
#nullable enable

public class InventoryMenu : Menu
{
    [SerializeField] GameObject prefabItemButton;

    [SerializeField] List<GameObject> itemButtonLocations;
    [SerializeField] GameObject ItemDescriptionLoc;
    [SerializeField] GameObject ItemNameLoc;
    List<GameObject> Buttons = new();

    // TODO: change these to [serialise field] references instead of instantiating in start and chaneg this in containermenu
    [SerializeField] private GameObject currentShownDescription;
    [SerializeField] private GameObject currentShownName;

    public void OpenInventory(List<Item> Items)
    {
        Buttons.Clear();

        for (int i = 0; i < itemButtonLocations.Count; i++)
        {
            // add all buttons
            var buttonObj = Instantiate(prefabItemButton, itemButtonLocations[i].transform);
            var itemButton = buttonObj.GetComponent<ItemOptionButton>();
            Buttons.Add(buttonObj);

            // add items to buttons if there is an item in that slot
            if (i < Items.Count)
            {
                itemButton.SetItem(Items[i]);
            }
        }

        InitialiseItemView(this.Buttons[0]);
        UIEventSystem.SetSelectedGameObject(this.Buttons[0]);
    }

    private void Start()
    {
        this.currentShownDescription.GetComponentInChildren<ItemDescriptionContainer>().SetDescription(string.Empty);
        currentShownName.GetComponentInChildren<ItemNameContainer>().SetName(string.Empty);
    }

    public void AddItem(Item item)
    {
        var itemButtonToUpdate = this.Buttons.Find(x => x.GetComponent<ItemOptionButton>().Item != null);

        itemButtonToUpdate.GetComponent<ItemOptionButton>().SetItem(item);
    }

    private void InitialiseItemView(GameObject firstSelected)
    {
        this.currentlyShownItem = firstSelected;
        var firstSelectedItem = firstSelected.GetComponent<ItemOptionButton>().Item;
        this.currentShownDescription.GetComponentInChildren<ItemDescriptionContainer>().SetDescription(firstSelectedItem.description);
        currentShownName.GetComponentInChildren<ItemNameContainer>().SetName(firstSelectedItem.Name);
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

    /// <summary>
    /// returns selected. 
    /// </summary>
    public override Item OnButtonSelected()
    {
        var selected = this.UIEventSystem.currentSelectedGameObject;

        selected.GetComponentInChildren<TMP_Text>().text = string.Empty;
        var spriteImageComponent = selected.GetComponentInChildren<ItemOptionButtonImage>();

        // TODO: do something to indicate equipped.
        // spriteImageComponent.SetImage(emptySlot);
        this.UIEventSystem.currentSelectedGameObject.TryGetComponent<ItemOptionButton>(out var itemBut);

        return itemBut.Item;
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
