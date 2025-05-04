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
    // instead of this
    // have in invnetory menu:
    // gear panel, ability panel, item panel that takes up the left top side of the screen
    // and a new class for each panel to control it. an InventoryMenu.cs juts controls them and tell them what to update and when
    // we also will have settings pages container, save pages container, dialogue log pages container that will hold and control their information.

    [Header("InventoryMenu")]
    private List<Item> AllInventoryItems = new();
    private List<Ability> PlayerAbilities = new();
    private BookTab selectedTab;
    [SerializeField] BookTab OnFirstOpenInventorySelectedTab;
    [SerializeField] GameObject AnimatedBookInventoryBackground;
    private Animator bookAnimator;

    [Header("Both Pages Containers")]
    private SaveMenu saveMenu;
    private SettingsMenu settingsMenu;

    [Header("Left Side Panel Containers")]
    private gearPanel gearPanel;
    private ItemPanel gearPanel;
    private playerStatsPanel playerPanel;
    private abilityPanel abilityPanel;

    /// <summary>
    /// Inventory Slots
    /// </summary>
    [Header("Right Side Panel Containers")]
    [SerializeField] GameObject gearSlotsContainer;
    [SerializeField] GameObject itemSlotsContainer;
    [SerializeField] GameObject InnateAbilitySlotsContainer;
    [SerializeField] GameObject GearAbilitySlotsContainer;

    // to go in player inventory panel.cs
    //[Header("Player Stuff")]
    //[SerializeField] List<GameObject> itemSlots;
    //[SerializeField] private GameObject equippedWeaponSlot;
    //[SerializeField] private GameObject equippedClothingSlot;
    //[SerializeField] private GameObject equippedSpecialItemSlot;
    //[SerializeField] private List<GameObject> abilitySlots;
    //[SerializeField] private TMP_Text playerPowerStatText;
    //[SerializeField] private TMP_Text playerDefenceStatText;
    //[SerializeField] private TMP_Text playerWellBeingText;


    ///// <summary>
    ///// <see cref="ItemViewContainer"/> Is the parent game object of all the stuff thats shown in the item view section
    ///// </summary>
    //[Header("Left Panel Gear View")]
    //[SerializeField] private GameObject ItemViewContainer;
    //[SerializeField] private GameObject itemDescriptionObject;
    //private ItemDescriptionContainer itemdescriptionContainer;
    //[SerializeField] private GameObject itemNameContainerObject;
    //private ItemNameContainer itemNameContainer;
    //[SerializeField] private TMP_Text powerValueText;
    //[SerializeField] private TMP_Text defenceValueText;
    //[SerializeField] private Image ItemTypeIndicatorIndicator;
    //[SerializeField] private Image ItemAbilityImage;
    //[SerializeField] private Sprite specialItemImage;
    //[SerializeField] private Sprite ArmourItemImage;
    //[SerializeField] private Sprite WeaponItemImage;
    

    private void Awake()
    {
        this.bookAnimator = this.AnimatedBookInventoryBackground.GetComponent<Animator>();

        itemdescriptionContainer = this.itemDescriptionObject.GetComponent<ItemDescriptionContainer>();
        itemNameContainer = this.itemNameContainerObject.GetComponent<ItemNameContainer>();
    }

    public override void Close()
    {
        this.bookAnimator.SetTrigger("Close");
        this.gearSlotsContainer.SetActive(false);
        StartCoroutine(DisableInventoryAfterBookAnim());
    }

    public void SelectTab(BookTab selectedTab)
    {
        this.UIEventSystem.SetSelectedGameObject(null);
        this.selectedTab = selectedTab;

        MyGuard.IsNotNull(selectedTab);

        // change sprite to the selected tab sprite so it stays (appearing) selected when selecting items.
        selectedTab.ForceTabSelectionAnim(true);

        // all other tabs false
        var tabs = FindObjectsByType<BookTab>(FindObjectsSortMode.None);
        var notSelectedTabs = tabs.Where(x => x != selectedTab).ToList();
        foreach (var tab in notSelectedTabs)
        {
            tab.ForceTabSelectionAnim(false);
        }

        // TODO 
        Debug.Log("todo change inventory screen shown and play page turn animation");

        // swap shown inventory items to the right category.
        // and highlight is by changeing the tab.SwapTabSprite() on it and all others false
        // store as current selected tab to remember upon re opening inventory
        // only show items in current selectd category e.g. weaponsb.
        this.UpdateItemsButtons();
    }

    private GameObject lastHighlightedItem;

    // When a raycast enabled image is highlighted with mouse.
    public void OnPointerEnter(PointerEventData eventData)
    {
        var highlightedWithItem = eventData.hovered.FirstOrDefault(x => x.TryGetComponent<InventorySlot>(out _));

        var highlightedWithTab = eventData.hovered.FirstOrDefault(x => x.TryGetComponent<BookTab>(out _));

        if (highlightedWithItem != null)
        {
            if (highlightedWithItem == lastHighlightedItem)
            {
                return;
            }

            lastHighlightedItem = highlightedWithItem;

            var itemButtonComp = highlightedWithItem.GetComponent<InventorySlot>();

            if (itemButtonComp.Item != null || itemButtonComp.Ability != null)
            {
                itemButtonComp.PlayHighlightOptionChangedSound();
                UpdateItemView(itemButtonComp);
            }
            else
            {
                SetItemViewToEmptyItem();
            }
        }
        else if (highlightedWithTab != null)
        {
            var bookTab = highlightedWithTab.GetComponent<BookTab>();
            bookTab.PlayHighlightOptionChangedSound();
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
    public void OpenInventory(List<Item> playerItems, List<Ability> playerAbilities)
    {
        AllInventoryItems = playerItems;
        this.PlayerAbilities = playerAbilities;

        this.gearSlotsContainer.SetActive(false);
        this.bookAnimator.SetTrigger("Open");
        Debug.Log("todo add book slide and open sound effect. then close then slide sfx also");

        StartCoroutine(EnableInventoryAfterBookAnim());
    }

    /// <summary>
    /// Updates item buttons to current category selected from this.selectedTab. TODO this better by maybe could make booktabs have a enum type that encompasses abilityes and items and have a parent class for abilities and items?
    /// </summary>
    private void UpdateItemsButtons()
    {
        // tab selected is either Ability Tab or item type.
        // if ability tab is selected tab, show abilities in slots not items.
        if (this.selectedTab.tabType == BookTab.TabType.Abilities)
        {
            // else if selected tab is an item type show all matching items
            // get all items matching selected tabs type
            var ItemsInCategory = this.PlayerAbilities;

            this.InventorySlots.Clear();

            for (int i = 0; i < itemSlots.Count; i++)
            {
                var itemButtonOld = itemSlots[i].GetComponentInChildren<InventorySlot>();

                // add items to buttons if there is enough number of items to fill up to the slot
                if (i < ItemsInCategory.Count)
                {
                    itemButtonOld.SetAbilityAndImage(ItemsInCategory[i]);
                }
                else
                {
                    itemButtonOld.ReplaceSlotWithBlanks();
                }

                this.InventorySlots.Add(itemButtonOld.gameObject);
            }

            // make sure players equipped items have equipped highlight on and so does the corresponding inventory item.
            var equippedAbilities = this.abilitySlots.Select(x => x.GetComponentInChildren<InventorySlot>().Ability);
            var inventorySlots = this.InventorySlots.Select(x => x.GetComponent<InventorySlot>()).ToList();

            foreach (var slot in inventorySlots)
            {
                if (slot.Item == null)
                {
                    slot.ToggleEquipGraphic(false);
                }
                else
                {
                    slot.ToggleEquipGraphic(equippedAbilities.Contains(slot.Ability));
                }
            }
        }
        else if (this.selectedTab.tabType == BookTab.TabType.Equipment)
        {
            // else if selected tab is an item type show all matching items
            // get all items matching selected tabs type
            var ItemsInCategory = AllInventoryItems.Where(x => x.Type == this.selectedTab.itemCategory).ToList();

            this.InventorySlots.Clear();

            for (int i = 0; i < itemSlots.Count; i++)
            {
                var itemButtonOld = itemSlots[i].GetComponentInChildren<InventorySlot>();

                // add items to buttons if there is enough number of items to fill up to the slot
                if (i < ItemsInCategory.Count)
                {
                    itemButtonOld.SetItemAndImage(ItemsInCategory[i]);
                }
                else
                {
                    itemButtonOld.ReplaceSlotWithBlanks();
                }

                this.InventorySlots.Add(itemButtonOld.gameObject);
            }

            // make sure players equipped items have equipped highlight on and so does the corresponding inventory item.
            GameObject equippedSlotForCategory = this.selectedTab.itemCategory switch
            {
                ItemType.Weapon => this.equippedWeaponSlot,
                ItemType.Clothing => this.equippedClothingSlot,
                ItemType.SpecialItem => this.equippedSpecialItemSlot,
                _ => throw new NotImplementedException()
            };

            var equippedThing = equippedSlotForCategory.GetComponentInChildren<InventorySlot>().Item;
            var inventorySlots = this.InventorySlots.Select(x => x.GetComponent<InventorySlot>()).ToList();

            foreach (var slot in inventorySlots)
            {
                if (slot.Item == null)
                {
                    slot.ToggleEquipGraphic(false);
                }
                else
                {
                    slot.ToggleEquipGraphic(slot.Item == equippedThing);
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

        this.gearSlotsContainer.SetActive(true);

        // TODO tab on opens to last open tab otherwise initiaalise to Items
        // and highlight is by changeing the tab.SwapTabSprite() on it and all others false
        // store as current selected tab to remember upon re opening inventory
        // only show items in current selectd category e.g. weaponsb.

        if (this.selectedTab == null)
        {
            this.selectedTab = this.OnFirstOpenInventorySelectedTab;
        }

        SelectTab(selectedTab);
    }

    private List<GameObject> equippedItemsSlots => new() { this.equippedWeaponSlot, this.equippedSpecialItemSlot, this.equippedClothingSlot };

    public void AddItem(Item item)
    {
        var itemButtonToUpdate = this.InventorySlots.Find(x => x.GetComponent<InventorySlot>().Item != null);

        itemButtonToUpdate.GetComponent<InventorySlot>().SetItemAndImage(item);
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
        itemdescriptionContainer.SetDescription("Blank");
        itemNameContainer.SetName("Empty Slot");

        this.powerValueText.text = "0";
        this.defenceValueText.text = "0";

        this.ItemTypeIndicatorIndicator.sprite = emptySlotImage;
        this.ItemAbilityImage.sprite = emptySlotImage;
    }

    private void UpdateItemView(InventorySlot itemButtonComp)
    {
        this.ItemTypeIndicatorIndicator.transform.parent.gameObject.SetActive(true);
        this.defenceValueText.gameObject.SetActive(true);
        this.defenceWord.gameObject.SetActive(true);
        this.ItemAbilityImage.sprite = this.emptySlotImage;

        if (itemButtonComp.Item == null && itemButtonComp.Ability == null)
        {
            SetItemViewToEmptyItem();
            return;
        }

        if (itemButtonComp.Ability != null)
        {
            this.ItemTypeIndicatorIndicator.transform.parent.gameObject.SetActive(false);
            this.defenceValueText.gameObject.SetActive(false);
            this.defenceWord.gameObject.SetActive(false);

            this.ItemAbilityImage.sprite = itemButtonComp.Ability.image;

            itemdescriptionContainer.SetDescription(itemButtonComp.Ability.description);
            itemNameContainer.SetName(itemButtonComp.Ability.Name);

            this.powerValueText.text = itemButtonComp.Ability.attackPower.ToString();
        }
        else if (itemButtonComp.Item != null)
        {
            itemdescriptionContainer.SetDescription(itemButtonComp.Item.description);
            itemNameContainer.SetName(itemButtonComp.Item.name);

            this.powerValueText.text = itemButtonComp.Item.PowerStat.ToString();
            this.defenceValueText.text = itemButtonComp.Item.DefenceStat.ToString();

            Sprite typeSprite = itemButtonComp.Item.Type switch
            {
                ItemType.Weapon => this.WeaponItemImage,
                ItemType.Clothing => this.ArmourItemImage,
                ItemType.SpecialItem => this.specialItemImage,
                _ => throw new ArgumentOutOfRangeException($"no Item Type found {itemButtonComp.Item.Type}")
            };

            this.ItemTypeIndicatorIndicator.sprite = typeSprite;

            // TODO smoehow show what ability the item has but I need to plan out the menu better.
            //this.ItemAbilityImageSlot.sprite = itemButtonComp.Item.ability.image
        }
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

        var oldItem = equipmentSlot.GetComponentInChildren<InventorySlot>();
        MyGuard.IsNotNull(oldItem);

        // toggle graphic on the inventory slot
        foreach (var thingo in this.InventorySlots)
        {
            var itemOption = thingo.GetComponentInChildren<InventorySlot>();
            if (itemOption.Item == oldItem.Item)
            {
                itemOption.ToggleEquipGraphic(false);
            }
        }

        oldItem.ReplaceSlotWithBlanks();
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
            _ => throw new ArgumentOutOfRangeException($"no Item Type found for type: {newItem.Type} on item: {newItem.Name}")
        };

        var currentEquipped = equippedSlot.GetComponentInChildren<InventorySlot>();

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
        foreach (var button in this.InventorySlots)
        {
            var itemOption = button.GetComponentInChildren<InventorySlot>();
            if (ItemToMatch == itemOption.Item)
            {
                itemOption.ToggleEquipGraphic(OnOff);
            }
        }
    }
}
