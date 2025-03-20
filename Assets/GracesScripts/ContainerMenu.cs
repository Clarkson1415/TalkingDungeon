using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;
#nullable enable

[RequireComponent(typeof(AudioSource))]
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
    private AudioSource? audioSource;

    private void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

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

    private void UpdateItemView(GameObject selectedButton)
    {
        this.currentlyShownItem = selectedButton;
        MyGuard.IsNotNull(currentShownDescription);
        MyGuard.IsNotNull(currentShownName);

        var newItemButtonComponent = selectedButton.GetComponent<ItemOptionButton>();

        // TODO: make cancel button more explicit than just not having an ItemOptionButton on the gameObject. maybe make a cancelButton Monobehaviour just to attach. to tell the difference bewteen the Cancel and other buttons when they are added
        if (newItemButtonComponent == null)
        {
            currentShownDescription.GetComponentInChildren<ItemDescriptionContainer>().SetItem("It says cancel stop looking here!");
            currentShownName.GetComponentInChildren<ItemNameContainer>().SetItem("Cancel DUH");
            return;
        }
        else if (newItemButtonComponent.Item == null)
        {
            currentShownDescription.GetComponentInChildren<ItemDescriptionContainer>().SetItem("Blank");
            currentShownName.GetComponentInChildren<ItemNameContainer>().SetItem("Empty Slot");
            return;
        }

        var newItem = newItemButtonComponent.Item;
        currentShownDescription.GetComponentInChildren<ItemDescriptionContainer>().SetItem(newItem.description);
        currentShownName.GetComponentInChildren<ItemNameContainer>().SetItem(newItem.name);
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

    private GameObject? currentlyShownItem;

    // Update is called once per frame
    void Update()
    {
        var highlightedMenuItem = this.UIEventSystem.currentSelectedGameObject;
        if (highlightedMenuItem != currentlyShownItem)
        {
            this.audioSource.Play();
            this.UpdateItemView(highlightedMenuItem);
        }
    }
}
