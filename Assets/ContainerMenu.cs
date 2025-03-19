using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContainerMenu : MonoBehaviour
{
    [SerializeField] GameObject prefabItemButton;
    [SerializeField] GameObject prefabItemDescription;
    [SerializeField] GameObject prefabItemName;
    [SerializeField] List<GameObject> itemButtonLocations;
    [SerializeField] List<GameObject> ItemDescription;
    [SerializeField] List<GameObject> ItemNameLoc;
    [SerializeField] EventSystem UIEventSystem;
    List<GameObject> Buttons = new List<GameObject>();

    private List<GameObject> Items = new();

    public void CreateLootButtons(List<Item> items)
    {
        var buttonObj = Instantiate(prefabItemButton, itemButtonLocations[0].transform);
        var itemButton = buttonObj.GetComponent<ItemOptionButton>();
        itemButton.SetItem(items[0]);
        Buttons.Add(buttonObj);
        Items.Add(buttonObj);

        var spriteImageComponent = buttonObj.gameObject.GetComponentInChildren<ItemOptionButtonImage>();
        spriteImageComponent.SetImage(items[0].image);

        var descriptionObj = Instantiate(prefabItemDescription, ItemDescription[0].transform);
        descriptionObj.GetComponentInChildren<ItemDescriptionContainer>().SetItem(items[0].description);
        Items.Add(descriptionObj);

        var nameObj = Instantiate(prefabItemName, ItemNameLoc[0].transform);
        nameObj.GetComponentInChildren<ItemNameContainer>().SetItem(items[0].name);
        Items.Add(nameObj);

        UIEventSystem.SetSelectedGameObject(this.Buttons[0]);
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
    public Item GetSelectedItem()
    {
        var selectedItemButton = this.Buttons.First(x => x.GetComponent<ItemOptionButton>().isSelected);
        var optionButton = selectedItemButton.GetComponent<ItemOptionButton>();
        return optionButton.Item;
    }

    // Update is called once per frame
    void Update()
    {
        // todo: statemachine
    }
}
