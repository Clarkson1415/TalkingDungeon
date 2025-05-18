using Assets.GracesScripts;
using Assets.GracesScripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private BookTab selectedTab;
    [SerializeField] BookTab OnFirstOpenInventorySelectedTab;
    [SerializeField] GameObject BookInOutAnimator;
    [SerializeField] private GameObject BookBackGround;
    private Image bookBackgroundImage;
    private Animator bookSlideInOutAnimator;

    [Header("Menu Sections")]
    [SerializeField] private GearPages gearPages;
    [SerializeField] private ItemPages ItemPages;
    [SerializeField] private SaveMenuPages saveMenuPages;
    [SerializeField] private SettingsPages settingsMenuPages;
    private List<BookPage> Pages => new() { gearPages, ItemPages, saveMenuPages, settingsMenuPages };
    [SerializeField] GameObject PageFlipper;
    private Animator flipPageAnimator;

    [Header("Player")]
    private Item playerEquippedWeapon;
    private Item? playerEquippedItem;
    [SerializeField] private Item HandsWeapon;


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

        this.bookSlideInOutAnimator.SetTrigger("Open");
        Debug.Log("todo add book slide and open sound effect. then close then slide sfx also");

        this.BookBackGround.SetActive(false);
        DeactivateAllPages();
        StartCoroutine(WaitForBookAnimThenSetup());
    }

    public void OnTabClick(BookTab selectedTab)
    {
        MyGuard.IsNotNull(selectedTab);
        this.selectedTab = selectedTab;
        DeactivateAllPages();
        this.EnableBookPage();
        StartCoroutine(PageFlip(1));
    }

    private void Awake()
    {
        this.bookSlideInOutAnimator = this.BookInOutAnimator.GetComponent<Animator>();
        this.bookBackgroundImage = this.BookBackGround.GetComponent<Image>();
        this.flipPageAnimator = this.PageFlipper.GetComponent<Animator>();
        DeactivateAllPages();
    }

    private void DeactivateAllPages()
    {
        foreach(var p in this.Pages)
        {
            p.TogglePageComponents(false);
            p.gameObject.SetActive(false);
        }
    }

    public override void Close()
    {
        this.bookSlideInOutAnimator.SetTrigger("Close");
        StartCoroutine(DisableInventoryAfterBookAnim());
    }

    IEnumerator PageFlip(int numberOfFlips)
    {
        this.PageFlipper.SetActive(true);
        for(int i = 0; i < numberOfFlips; i++)
        {
            this.flipPageAnimator.SetTrigger("Open");
            yield return new WaitForSeconds(0.22f);
        }

        this.PageFlipper.SetActive(false);
    }

    /// <summary>
    /// When a tab is select this method changes what is displayed inside the book pages
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void EnableBookPage()
    {
        this.bookBackgroundImage.sprite = this.selectedTab.SelectedSprite;
        var Page = GetCurrentPage();
        Page.gameObject.SetActive(true);
        Page.TogglePageComponents(true);

        if (Page is PageWithSlots pageWithSlots)
        {
            pageWithSlots.FillItemSlots(this.AllInventoryItems.Where(x => x.Type == ItemType.Weapon).ToList(), this.playerEquippedWeapon, this.playerEquippedItem);
        }
    }

    private BookPage GetCurrentPage()
    {
        if (this.selectedTab.tabType == BookTab.TabType.Gear)
        {
            return this.gearPages;
        }
        else if (this.selectedTab.tabType == BookTab.TabType.Items)
        {
            return this.ItemPages;
        }
        else
        {
            throw new NotImplementedException("cant change to that tab yet");
        }
    }

    private IEnumerator DisableInventoryAfterBookAnim()
    {
        StartCoroutine(PageFlip(1));
        this.GetCurrentPage().TogglePageComponents(false);
        this.BookBackGround.SetActive(false);

        while (!this.bookSlideInOutAnimator.GetCurrentAnimatorStateInfo(0).IsName("OffscreenClosed"))
        {
            yield return null;
        }

        base.Close();
    }

    public void OnButtonClicked(InventorySlot slotClicked)
    {
        // todo for other buttons save menu and stuff.i
    }

    private IEnumerator WaitForBookAnimThenSetup()
    {
        // wait for slide in anim to finish then flip page and tabs open at the same time
        while (!this.bookSlideInOutAnimator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(1);

        if (this.selectedTab == null)
        {
            this.selectedTab = this.OnFirstOpenInventorySelectedTab;
        }

        this.BookBackGround.SetActive(true);
        this.EnableBookPage();
        StartCoroutine(PageFlip(1));
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

        if (highlightedTabItem != null)
        {
            var bookTab = highlightedTabItem.GetComponent<BookTab>();
            bookTab.PlayHighlightOptionChangedSound();
        }

        if (gameobjectSlot != null && this.selectedTab != null)
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
            Debug.Log("make sure to update player weapon also am i doing that?");
        }
        else
        {
            this.playerEquippedItem = null;
        }

        this.gearPages.RemoveEquippedItem(selectedSlot.Item);
        this.ItemPages.RemoveEquippedItem(selectedSlot.Item);
    }
}
