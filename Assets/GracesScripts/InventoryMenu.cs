using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;
#nullable enable

public class InventoryMenu : MonoBehaviour
{
    [SerializeField] GameObject prefabItemButton;
    [SerializeField] GameObject prefabItemDescription;
    [SerializeField] GameObject prefabItemName;
    [SerializeField] List<GameObject> itemButtonLocations;
    [SerializeField] GameObject ItemDescriptionLoc;
    [SerializeField] GameObject ItemNameLoc;
    [SerializeField] EventSystem UIEventSystem;
    List<GameObject> Buttons = new();
    [SerializeField] private List<Item> Items;
    private GameObject? currentShownDescription;
    private GameObject? currentShownName;

    private void Start()
    {
        for (int i = 0; i < itemButtonLocations.Count; i++)
        {
            // add all buttons
            var buttonObj = Instantiate(prefabItemButton, itemButtonLocations[i].transform);
            var itemButton = buttonObj.GetComponent<ItemOptionButton>();

            // add items to buttons if there is an item in that slot
            if (i < Items.Count)
            {
                itemButton.SetItem(Items[i]);
                Buttons.Add(buttonObj);

                var spriteImageComponent = buttonObj.gameObject.GetComponentInChildren<ItemOptionButtonImage>();
                spriteImageComponent.SetImage(Items[i].image);
            }
        }

        InitialiseItemView(this.Buttons[0]);
        UIEventSystem.SetSelectedGameObject(this.Buttons[0]);
    }

    public void Close()
    {
        this.UIEventSystem.SetSelectedGameObject(null);
    }

    public void AddItem(Item item)
    {
        var itemButtonToUpdate = this.Buttons.Find(x => x.GetComponent<ItemOptionButton>().Item != null);
        var spriteImageComponent = itemButtonToUpdate.gameObject.GetComponentInChildren<ItemOptionButtonImage>();
        spriteImageComponent.SetImage(item.image);
    }

    private void InitialiseItemView(GameObject firstSelected)
    {
        this.currentlyShownItem = firstSelected;
        this.currentShownDescription = Instantiate(prefabItemDescription, ItemDescriptionLoc.transform);
        var firstSelectedItem = firstSelected.GetComponent<ItemOptionButton>().Item;
        this.currentShownDescription.GetComponentInChildren<ItemDescriptionContainer>().SetItem(firstSelectedItem.description);

        currentShownName = Instantiate(prefabItemName, ItemNameLoc.transform);
        currentShownName.GetComponentInChildren<ItemNameContainer>().SetItem(firstSelectedItem.Name);
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

    /// <summary>
    /// </summary>
    /// <summary>
    /// returns selected. 
    /// </summary>
    public Item OnButtonSelected()
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

        // on menu open do onece
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
