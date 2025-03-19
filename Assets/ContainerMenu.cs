using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using UnityEngine;
using UnityEngine.EventSystems;
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

        InitialiseItemView(items[0]);

        UIEventSystem.SetSelectedGameObject(this.Buttons[0]);
    }

    private GameObject? currentShownDescription;
    private GameObject? currentShownName;

    private void InitialiseItemView(Item item)
    {
        this.currentlyShownItem = item;
        this.currentShownDescription = Instantiate(prefabItemDescription, ItemDescriptionLoc.transform);
        this.currentShownDescription.GetComponentInChildren<ItemDescriptionContainer>().SetItem(item.description);
        Items.Add(this.currentShownDescription);

        currentShownName = Instantiate(prefabItemName, ItemNameLoc.transform);
        currentShownName.GetComponentInChildren<ItemNameContainer>().SetItem(item.name);
        Items.Add(currentShownName);
    }

    private void UpdateItemView(Item? item)
    {
        this.currentlyShownItem = item;
        MyGuard.IsNotNull(currentShownDescription);
        MyGuard.IsNotNull(currentShownName);

        if (item == null)
        {
            currentShownDescription.GetComponentInChildren<ItemDescriptionContainer>().SetItem(" ");
            currentShownName.GetComponentInChildren<ItemNameContainer>().SetItem("Empty Slot");
            return;
        }

        currentShownDescription.GetComponentInChildren<ItemDescriptionContainer>().SetItem(item.description);
        currentShownName.GetComponentInChildren<ItemNameContainer>().SetItem(item.name);
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
    /// TODO: really slow too many get components
    /// </summary>
    public Item? GetSelectedItem()
    {
        if (this.UIEventSystem.currentSelectedGameObject.GetComponent<ItemOptionButton>().Item == null || this.Buttons.Count == 0)
        {
            return null;
        }

        var selectedItemButton = this.Buttons.First(x => x.GetComponent<ItemOptionButton>().isSelected);
        var optionButton = selectedItemButton.GetComponent<ItemOptionButton>();
        return optionButton.Item;
    }

    private Item? currentlyShownItem;

    // Update is called once per frame
    void Update()
    {
        // TODO this is causing a null reference error do this another way?
        var highlightedMenuItem = this.UIEventSystem.currentSelectedGameObject.GetComponent<ItemOptionButton>().Item;
        if (highlightedMenuItem != currentlyShownItem)
        {
            this.UpdateItemView(highlightedMenuItem);
        }
    }
}
