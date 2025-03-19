using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerMenu : MonoBehaviour
{
    [SerializeField] GameObject prefabItemButton;
    [SerializeField] GameObject prefabItemDescription;
    [SerializeField] GameObject prefabItemImage;
    [SerializeField] List<GameObject> itemButtonLocations;
    [SerializeField] List<GameObject> ItemDescription;
    [SerializeField] List<GameObject> ItemSpriteLocations;
    [SerializeField] EventSystem UIEventSystem;
    List<GameObject> Buttons = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CreateLootButtons(List<Item> items)
    {
        var buttonObj = Instantiate(prefabItemButton, itemButtonLocations[0].transform);
        var itemButton = buttonObj.GetComponent<ItemOptionButton>();
        itemButton.SetItem(items[0]);
        itemButton.UpdateButtonText();
        Buttons.Add(buttonObj);

        var descriptionObj = Instantiate(prefabItemDescription, ItemDescription[0].transform);
        descriptionObj.GetComponent<ItemDescriptionContainer>().SetItem(items[0].description);

        var spriteObj = Instantiate(prefabItemImage, ItemSpriteLocations[0].transform);
        spriteObj.GetComponent<ItemSpriteContainer>().SetItem(items[0].image);

        UIEventSystem.SetSelectedGameObject(this.Buttons[0]);
    }

    /// <summary>
    /// TODO: really slow too many get components
    /// </summary>
    public Item GetSelectedItem()
    {
        var selectedDiaOption = this.Buttons.First(x => x.GetComponent<ItemOptionButton>().isSelected);
        var optionButton = selectedDiaOption.GetComponent<ItemOptionButton>();
        return optionButton.Item;
    }

    // Update is called once per frame
    void Update()
    {
        // todo: statemachine
    }
}
