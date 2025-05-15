using Assets.GracesScripts;
using Assets.GracesScripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private BookTab selectedTab;
    [SerializeField] BookTab OnFirstOpenInventorySelectedTab;
    [SerializeField] GameObject AnimatedBookInventoryBackground;
    private Animator bookAnimator;

    [Header("Menu Sections")]
    [SerializeField] private GearPages gearPages;
    [SerializeField] private ItemPages ItemPages;
    [SerializeField] private SaveMenuPages saveMenuPages;
    [SerializeField] private SettingsPages settingsMenuPages;

    [Header("Player")]
    private Item playerEquippedWeapon;
    private Item? playerEquippedItem;
    [SerializeField] private Item HandsWeapon;

    private void Awake()
    {
        this.bookAnimator = this.AnimatedBookInventoryBackground.GetComponent<Animator>();
    }

    public override void Close()
    {
        this.bookAnimator.SetTrigger("Close");
        StartCoroutine(DisableInventoryAfterBookAnim());
    }

    public void SelectTab(BookTab selectedTab)
    {
        MyGuard.IsNotNull(selectedTab);
        this.UIEventSystem.SetSelectedGameObject(null);
        this.selectedTab = selectedTab;


        // change sprite to the selected tab sprite so it stays (appearing) selected when selecting items.
        selectedTab.ForceTabSelectionAnim(true);

        // Make sure all other Tabs return to normal.
        var tabs = FindObjectsByType<BookTab>(FindObjectsSortMode.None);
        var notSelectedTabs = tabs.Where(x => x != selectedTab).ToList();
        foreach (var tab in notSelectedTabs)
        {
            tab.ForceTabSelectionAnim(false);
        }

        // Show the correct windows 
        ChangeBookWindows();
    }

    /// <summary>
    /// When a tab is select this method changes what is displayed inside the book pages
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void ChangeBookWindows()
    {
        // TODO page turn animation
        // then content appears


        if (this.selectedTab.tabType == BookTab.TabType.Gear)
        {
            this.gearPages.FlipToPage();
            this.gearPages.FillItemSlots(this.AllInventoryItems.Where(x => x.Type == ItemType.Weapon).ToList(), this.playerEquippedWeapon, this.playerEquippedItem);
        }
        else if (this.selectedTab.tabType == BookTab.TabType.Items)
        {
            this.ItemPages.FlipToPage();
            this.ItemPages.FillItemSlots(this.AllInventoryItems.Where(x => x.Type == ItemType.SpecialItem).ToList(), this.playerEquippedWeapon, this.playerEquippedItem);
        }
        else
        {
            throw new NotImplementedException("cant change to that tab yet");
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
    public void OpenInventory(List<Item> playerItems, Item playerEquippedWeapon, Item? playerEquippedItem)
    {
        AllInventoryItems = playerItems;

        this.playerEquippedWeapon = playerEquippedWeapon;
        this.playerEquippedItem = playerEquippedItem;

        this.bookAnimator.SetTrigger("Open");
        Debug.Log("todo add book slide and open sound effect. then close then slide sfx also");

        StartCoroutine(EnableInventoryAfterBookAnim());
    }

    public void OnButtonClicked(InventorySlot slotClicked)
    {
        if (this.selectedTab.tabType == BookTab.TabType.Gear || this.selectedTab.tabType == BookTab.TabType.Items)
        {
            throw new NotImplementedException("not done yet");
        }
        else
        {
            throw new NotImplementedException("not done yet. probably do nothing the button Onclick method should do it. this is for like save buttons and stuff.");
        }
    }

    private IEnumerator EnableInventoryAfterBookAnim()
    {
        while (!this.bookAnimator.GetCurrentAnimatorStateInfo(0).IsName("StayOpen"))
        {
            yield return null;
        }

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

    private GameObject lastHighlightedItem;

    /// <summary>
    /// When a raycast enabled image is highlighted with mouse.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        var gameobjectSlot = eventData.hovered.FirstOrDefault(x => x.gameObject.transform.parent.TryGetComponent<InventorySlot>(out _));

        var highlightedTabItem = eventData.hovered.FirstOrDefault(x => x.TryGetComponent<BookTab>(out _));

        if (gameobjectSlot != null)
        {
            if (gameobjectSlot == lastHighlightedItem)
            {
                return;
            }

            lastHighlightedItem = gameobjectSlot;

            var itemButtonComp = gameobjectSlot.GetComponentInParent<InventorySlot>();

            if (this.selectedTab.tabType == BookTab.TabType.Gear)
            {
                this.gearPages.UpdateItemView(itemButtonComp);
            }
            else if (this.selectedTab.tabType == BookTab.TabType.Items)
            {
                this.ItemPages.UpdateItemView(itemButtonComp);
            }
        }
        else if (highlightedTabItem != null)
        {
            var bookTab = highlightedTabItem.GetComponent<BookTab>();
            bookTab.PlayHighlightOptionChangedSound();
        }
    }

    public void AddToPlayerEquipped(InventorySlot selectedSlot)
    {
        MyGuard.IsNotNull(selectedSlot.Item);

        if (selectedSlot.Item.Type == ItemType.Weapon)
        {
            this.playerEquippedWeapon = selectedSlot.Item;
        }
        else
        {
            this.playerEquippedItem = selectedSlot.Item;
        }

        this.gearPages.RemoveEquippedItem(selectedSlot.Item);
        this.ItemPages.RemoveEquippedItem(selectedSlot.Item);
        this.gearPages.EquipItem(selectedSlot.Item);
        this.ItemPages.EquipItem(selectedSlot.Item);
    }

    public void RemoveFromPlayerEquipped(InventorySlot selectedSlot)
    {
        MyGuard.IsNotNull(selectedSlot.Item);

        if (selectedSlot.Item.Type == ItemType.Weapon)
        {
            this.playerEquippedWeapon = this.HandsWeapon;
        }
        else
        {
            this.playerEquippedItem = null;
        }

        this.gearPages.RemoveEquippedItem(selectedSlot.Item);
        this.ItemPages.RemoveEquippedItem(selectedSlot.Item);
    }
}
