using Assets.GracesScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#nullable enable

[RequireComponent(typeof(AudioSource))]
public class InventoryMenu : Menu, IPointerEnterHandler
{
    [SerializeField] List<GameObject> itemButtonLocations;
    [SerializeField] private GameObject equippedWeaponSlot;
    [SerializeField] private GameObject equippedClothingSlot;
    [SerializeField] private GameObject equippedSpecialItemSlot;

    [Header("Item UI description")]
    [SerializeField] private Image ItemTypeIndicatorImageSlot;
    [SerializeField] private Sprite specialItemImage;
    [SerializeField] private Sprite ArmourItemImage;
    [SerializeField] private Sprite WeaponItemImage;

    [SerializeField] GameObject itemDescriptionObject;
    [SerializeField] GameObject nameContainerObject;

    private ItemDescriptionContainer descriptionContainer;
    private ItemNameContainer nameContainer;
    
    [SerializeField] private TMP_Text powerValueLoc;
    [SerializeField] private TMP_Text defenceValueLoc;

    [Header("player Stats")]
    [SerializeField] private TMP_Text playerPowerStatText;
    [SerializeField] private TMP_Text playerDefenceStatText;
    [SerializeField] private TMP_Text playerWellBeingText;

    List<GameObject> Buttons_NotIncludesEquippedITems = new();

    [Header("InventoryAnimation")]
    [SerializeField] GameObject AnimatedBookInventoryBackground;
    private Animator bookAnimator;
    [SerializeField] GameObject Inventory;

    private void Awake()
    {
        this.bookAnimator = this.AnimatedBookInventoryBackground.GetComponent<Animator>();

        descriptionContainer = this.itemDescriptionObject.GetComponent<ItemDescriptionContainer>();
        nameContainer = this.nameContainerObject.GetComponent<ItemNameContainer>();
    }

    public override void Close()
    {
        this.bookAnimator.SetTrigger("Close");
        this.Inventory.SetActive(false);
        StartCoroutine(DisableInventoryAfterBookAnim());
    }

    public void ChangeTabs(BookTab selectedTab)
    {
        selectedTab.PlayHighlightOptionChangedSound();
        // TODO change sprite to the selected tab sprite so it stays (appearing) selected
        selectedTab.SwapTabSprite(true);

        // all other tabs false
        var tabs = FindObjectsByType<BookTab>(FindObjectsSortMode.None);
        var notSelectedTabs = tabs.Where(x => x != selectedTab);
        foreach (var tab in notSelectedTabs)
        {
            tab.SwapTabSprite(false);
        }

        // TODO 
        // swap shown inventory items to the right category.
        // and highlight is by changeing the tab.SwapTabSprite() on it and all others false
        // store as current selected tab to remember upon re opening inventory
        // only show items in current selectd category e.g. weaponsb.
    }

    private GameObject lastHighlightedItem;

    // When a raycast enabled image is highlighted with mouse.
    public void OnPointerEnter(PointerEventData eventData)
    {
        var highlighted = eventData.hovered.FirstOrDefault(x => x.TryGetComponent<ItemOptionButton>(out _));

        if (highlighted == lastHighlightedItem)
        {
            return;
        }

        lastHighlightedItem = highlighted;

        // only update item view if the mouse hovered over an item option button otherwise could have hovered over some other we dont care about and should be empty
        if (highlighted != null)
        {
            var hasItemButtonComponent = highlighted.TryGetComponent<ItemOptionButton>(out var itemButtonComp);
            itemButtonComp.PlayHighlightOptionChangedSound();
            UpdateItemView(itemButtonComp);
        }
        else
        {
            SetItemViewToEmptyItem();
        }
    }

    private IEnumerator DisableInventoryAfterBookAnim()
    {
        while (!this.bookAnimator.GetCurrentAnimatorStateInfo(0).IsName("OffscreenClosed"))
        {
            yield return null;
        }

        base.Close();
    }

    /// <summary>
    /// TODO: mahbe change so first selected item was the same as when it was last opened. instead of inistialising to button 0?
    /// and this only needs to be initialised once.
    /// </summary>
    /// <param name="Items"></param>
    public void OpenInventory(List<Item?> Items)
    {
        this.Inventory.SetActive(false);
        this.bookAnimator.SetTrigger("Open");
        StartCoroutine(EnableInventoryAfterBookAnim());

        // TODO tab on opens to last open tab otherwise initiaalise to Items
        // and highlight is by changeing the tab.SwapTabSprite() on it and all others false
        // store as current selected tab to remember upon re opening inventory
        // only show items in current selectd category e.g. weaponsb.


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

        // make sure players equipped items have equipped highlight on and so does the corresponding inventory item.
        foreach (var GO in this.equippedItemsSlots)
        {
            var button = GO.GetComponentInChildren<ItemOptionButton>();
            if (button.Item == null)
            {
                continue;
            }

            foreach (var inventoryButton in this.Buttons_NotIncludesEquippedITems)
            {
                var slot = inventoryButton.GetComponentInChildren<ItemOptionButton>();

                if (button.Item == slot.Item)
                {
                    slot.ToggleEquipGraphic(true);
                }
            }
        }
    }

    private IEnumerator EnableInventoryAfterBookAnim()
    {
        while (!this.bookAnimator.GetCurrentAnimatorStateInfo(0).IsName("StayOpen"))
        {
            yield return null;
        }

        this.Inventory.SetActive(true);
    }

    private List<GameObject> equippedItemsSlots => new() { this.equippedWeaponSlot, this.equippedSpecialItemSlot, this.equippedClothingSlot };

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

    private void SetItemViewToEmptyItem()
    {
        descriptionContainer.SetDescription("Blank");
        nameContainer.SetName("Empty Slot");

        this.powerValueLoc.text = "0";
        this.defenceValueLoc.text = "0";

        this.ItemTypeIndicatorImageSlot.sprite = emptySlotImage;
    }

    private void UpdateItemView(ItemOptionButton itemButtonComp)
    {
        if (itemButtonComp.Item == null)
        {
            SetItemViewToEmptyItem();
            return;
        }

        descriptionContainer.SetDescription(itemButtonComp.Item.description);
        nameContainer.SetName(itemButtonComp.Item.name);

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
        MyGuard.IsNotNull(oldItem);

        // toggle graphic on the inventory slot
        foreach (var thingo in this.Buttons_NotIncludesEquippedITems)
        {
            var itemOption = thingo.GetComponentInChildren<ItemOptionButton>();
            if (itemOption.Item == oldItem.Item)
            {
                itemOption.ToggleEquipGraphic(false);
            }
        }

        oldItem.ReplaceItemWithBlank();
        oldItem.ToggleEquipGraphic(false);
    }

    /// <summary>
    /// add an item to its relevant equipped slot.
    /// </summary>
    /// <param name="newItem"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void AddOrReplaceEquipped(Item newItem)
    {
        MyGuard.IsNotNull(newItem);
        GameObject equippedSlot = newItem.Type switch
        {
            ItemType.Weapon => this.equippedWeaponSlot,
            ItemType.Clothing => this.equippedClothingSlot,
            ItemType.SpecialItem => this.equippedSpecialItemSlot,
            _ => throw new ArgumentOutOfRangeException($"no Item Type found {newItem.Type}")
        };

        var currentEquipped = equippedSlot.GetComponentInChildren<ItemOptionButton>();

        // if had item equipped before replacing, remove the equipped highlight from the inventory item
        if (currentEquipped.Item != null)
        {
            ToggleEquipImageOnInventoryItem(currentEquipped.Item, false);
        }

        // set equipment slot and toggle graphic on the equipped item slot
        currentEquipped.SetItemAndImage(newItem);
        currentEquipped.ToggleEquipGraphic(true);

        MyGuard.IsNotNull(currentEquipped.Item);
        // toggle graphic on the inventory slot to true
        ToggleEquipImageOnInventoryItem(currentEquipped.Item, true);
    }

    private void ToggleEquipImageOnInventoryItem(Item ItemToMatch, bool OnOff)
    {
        foreach (var button in this.Buttons_NotIncludesEquippedITems)
        {
            var itemOption = button.GetComponentInChildren<ItemOptionButton>();
            if (ItemToMatch == itemOption.Item)
            {
                itemOption.ToggleEquipGraphic(OnOff);
            }
        }
    }
}
