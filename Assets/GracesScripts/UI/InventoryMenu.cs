using Assets.GracesScripts;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#nullable enable

[RequireComponent(typeof(AudioSource))]
public class InventoryMenu : Menu
{
    [SerializeField] GameObject prefabItemButton;

    [SerializeField] List<GameObject> itemButtonLocations;
    [SerializeField] private GameObject equippedWeaponSlot;
    [SerializeField] private GameObject equippedClothingSlot;
    [SerializeField] private GameObject equippedSpecialItemSlot;


    [Header("Item UI description")]
    [SerializeField] private Image ItemTypeIndicatorImageSlot;
    [SerializeField] private Sprite specialItemImage;
    [SerializeField] private Sprite ArmourItemImage;
    [SerializeField] private Sprite WeaponItemImage;

    [SerializeField] private GameObject descriptionContainer;
    [SerializeField] private GameObject nameContainer;
    [SerializeField] private TMP_Text powerValueLoc;
    [SerializeField] private TMP_Text defenceValueLoc;

    [Header("player Stats")]
    [SerializeField] private TMP_Text playerPowerStatText;
    [SerializeField] private TMP_Text playerDefenceStatText;
    [SerializeField] private TMP_Text playerWellBeingText;

    List<GameObject> Buttons_NotIncludesEquippedITems = new();

    private void Awake()
    {
    }

    /// <summary>
    /// TODO: mahbe change so first selected item was the same as when it was last opened. instead of inistialising to button 0?
    /// and this only needs to be initialised once.
    /// </summary>
    /// <param name="Items"></param>
    public void OpenInventory(List<Item> Items)
    {
        // todo instead of clearning juts add any enw items curreently it keeps equipped correct
        Buttons_NotIncludesEquippedITems.Clear();

        for (int i = 0; i < itemButtonLocations.Count; i++)
        {
            // add all buttons of your items 
            // TODO could this be a Menu.cs method instead?
            var itemButtonOld = itemButtonLocations[i].GetComponentInChildren<ItemOptionButton>();
            this.Buttons_NotIncludesEquippedITems.Add(itemButtonOld.gameObject);

            // add items to buttons if there is an item in that slot
            if (i < Items.Count)
            {
                itemButtonOld.SetItemAndImage(Items[i]);
            }
        }

        UIEventSystem.SetSelectedGameObject(this.Buttons_NotIncludesEquippedITems[0]);
        UpdateItemView();
    }

    public void AddItem(Item item)
    {
        var itemButtonToUpdate = this.Buttons_NotIncludesEquippedITems.Find(x => x.GetComponent<ItemOptionButton>().Item != null);

        itemButtonToUpdate.GetComponent<ItemOptionButton>().SetItemAndImage(item);
    }

    public void UpdatePlayerStatsDisplay(float power, float defence)
    {
        this.playerPowerStatText.text = power.ToString();
        this.playerDefenceStatText.text = defence.ToString();
    }

    public void UpdatePlayerWellBeingDislpay(int wellBeing)
    {
        this.playerWellBeingText.text = wellBeing.ToString();
    }

    private void UpdateItemView()
    {
        this.currentlyShownItem = this.UIEventSystem.currentSelectedGameObject;
        MyGuard.IsNotNull(descriptionContainer);
        MyGuard.IsNotNull(nameContainer);

        var newItemButtonComponent = this.currentlyShownItem.TryGetComponent<ItemOptionButton>(out var itemButtonComp);

        if (itemButtonComp.Item == null)
        {
            descriptionContainer.GetComponentInChildren<ItemDescriptionContainer>().SetDescription("Blank");
            nameContainer.GetComponentInChildren<ItemNameContainer>().SetName("Empty Slot");

            this.powerValueLoc.text = "0";
            this.defenceValueLoc.text = "0";

            this.ItemTypeIndicatorImageSlot.sprite = emptySlotImage;
            return;
        }

        descriptionContainer.GetComponentInChildren<ItemDescriptionContainer>().SetDescription(itemButtonComp.Item.description);
        nameContainer.GetComponentInChildren<ItemNameContainer>().SetName(itemButtonComp.Item.name);

        this.powerValueLoc.text = itemButtonComp.Item.PowerStat.ToString();
        this.defenceValueLoc.text = itemButtonComp.Item.DefenceStat.ToString();

        Sprite typeSprite = itemButtonComp.Item.Type switch
        {
            ItemType.Weapon => this.WeaponItemImage,
            ItemType.Clothing => this.ArmourItemImage,
            ItemType.SpecialItem => this.specialItemImage,
            _ => throw new ArgumentOutOfRangeException($"no Item Type found {itemButtonComp.Item.Type}")
        };

        this.ItemTypeIndicatorImageSlot.sprite = typeSprite;
    }

    public void RemoveEquippedItem(Item item)
    {
        GameObject equipmentSlot = item.Type switch
        {
            ItemType.Weapon => this.equippedWeaponSlot,
            ItemType.Clothing => this.equippedClothingSlot,
            ItemType.SpecialItem => this.equippedSpecialItemSlot,
            _ => throw new ArgumentOutOfRangeException($"no Item Type found {item.Type}")
        };

        var oldItem = equipmentSlot.GetComponentInChildren<ItemOptionButton>();
        if (oldItem != null)
        {
            oldItem.ReplaceItemWithBlank();
        }
    }

    /// <summary>
    /// TODO dont instantiate new one. add one in inspector then update the item instead.
    /// </summary>
    /// <param name="newItem"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void AddOrReplaceEquipped(Item newItem)
    {
        GameObject equipmentSlot = newItem.Type switch
        {
            ItemType.Weapon => this.equippedWeaponSlot,
            ItemType.Clothing => this.equippedClothingSlot,
            ItemType.SpecialItem => this.equippedSpecialItemSlot,
            _ => throw new ArgumentOutOfRangeException($"no Item Type found {newItem.Type.ToString()}")
        };

        var currentEquipped = equipmentSlot.GetComponentInChildren<ItemOptionButton>();

        if (currentEquipped.Item != null)
        {
            // deselect old item UI Highlight
            foreach (var button in this.Buttons_NotIncludesEquippedITems)
            {
                var itemOption = button.GetComponentInChildren<ItemOptionButton>();
                if (itemOption.Item == currentEquipped.Item)
                {
                    itemOption.ToggleEquipGraphic();
                }
            }

            currentEquipped.SetItemAndImage(newItem);
        }
        else
        {
            currentEquipped.SetItemAndImage(newItem);
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
            this.UIEventSystem.SetSelectedGameObject(this.Buttons_NotIncludesEquippedITems[0]);
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
