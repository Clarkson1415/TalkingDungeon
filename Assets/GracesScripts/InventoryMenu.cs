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

    [SerializeField] private GameObject currentShownDescription;
    [SerializeField] private GameObject currentShownName;

    /// <summary>
    /// TODO: mahbe change so first selected item was the same as when it was last opened. instead of inistialising to button 0?
    /// </summary>
    /// <param name="Items"></param>
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

        UIEventSystem.SetSelectedGameObject(this.Buttons[0]);
        UpdateItemView();
    }

    private void Start()
    {
    }

    public void AddItem(Item item)
    {
        var itemButtonToUpdate = this.Buttons.Find(x => x.GetComponent<ItemOptionButton>().Item != null);

        itemButtonToUpdate.GetComponent<ItemOptionButton>().SetItem(item);
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
