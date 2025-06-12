using Assets.GracesScripts.ScriptableObjects;
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
public class InventoryMenu : MenuWithItemSlots, IPointerEnterHandler
{
    [Header("InventoryMenu")]
    private List<DungeonItem> AllInventoryItems = new();
    private BookTab? selectedTab;
    [SerializeField] BookTab OnFirstOpenInventorySelectedTab;
    [SerializeField] GameObject BookInOutAnimator;
    [SerializeField] private GameObject BookBackGround;
    private Image? bookBackgroundImage;
    private Animator? bookSlideInOutAnimator;

    [Header("Menu Sections")]
    [SerializeField] private GearPages gearPages;
    [SerializeField] private ItemPages ItemPages;
    [SerializeField] private SaveMenuPages saveMenuPages;
    [SerializeField] private SettingsPages settingsMenuPages;
    private List<BookPage> Pages => new() { gearPages, ItemPages, saveMenuPages, settingsMenuPages };
    [SerializeField] GameObject PageFlipper;
    private Animator? flipPageAnimator;

    [Header("Player")]
    private Weapon? _playerEquippedWeapon;
    private Weapon PlayerEquippedWeapon 
    {
        get => _playerEquippedWeapon != null ? _playerEquippedWeapon : DefaultWeaponHands; 
        set => _playerEquippedWeapon = value;
    }

    private SpecialItem? playerEquippedItem;
    protected Weapon DefaultWeaponHands => SaveGameUtility.GetDefaultHands();


    protected override void UpdateItemView(InventorySlot slot)
    {
        MyGuard.IsNotNull(this.selectedTab);
        if (this.selectedTab.tabType == BookTab.TabType.Gear)
        {
            this.gearPages.UpdateItemView(slot);
        }
        else if (this.selectedTab.tabType == BookTab.TabType.Items)
        {
            this.ItemPages.UpdateItemView(slot);
        }
    }

    /// <summary>
    /// TODO: mahbe change so first selected item was the same as when it was last opened. instead of inistialising to button 0?
    /// and this only needs to be initialised once.
    /// </summary>
    /// <param name="Items"></param>
    public void OpenInventory(List<DungeonItem> playerItems, Weapon playerEquippedWeapon, SpecialItem? playerEquippedItem)
    {
        AllInventoryItems = playerItems;

        this.PlayerEquippedWeapon = playerEquippedWeapon;
        this.playerEquippedItem = playerEquippedItem;
        MyGuard.IsNotNull(this.bookSlideInOutAnimator);
        this.bookSlideInOutAnimator.SetTrigger("Open");
        Debug.Log("todo add book slide and open sound effect. then close then slide sfx also");

        this.BookBackGround.SetActive(false);
        DeactivateAllPages();
        StartCoroutine(WaitForBookAnimThenSetup());
    }

    public void OnTabClick()
    {
        var selectedTab = this.lastHighlightedItem.GetComponentInParent<BookTab>();
        MyGuard.IsNotNull(selectedTab);
        this.selectedTab = selectedTab;
        DeactivateAllPages();
        this.EnableBookPage();
        StartCoroutine(PageFlip(1));
    }

    public void OnInventorySlotClicked()
    {
        var buttonGameObject = this.GetSelectedButton();
        var player = FindObjectOfType<PlayerDungeon>();
        MyGuard.IsNotNull(player, "PlayerDungeon is null in InventoryMenu.OnInventorySlotClicked");

        if (buttonGameObject == null)
        {
            Debug.Log("clicked on nothing");
            return;
        }
        if (!buttonGameObject.TryGetComponent<InventorySlot>(out var selectedItemOp))
        {
            // clicked on something else
            return;
        }

        if (selectedItemOp.Item == null)
        {
            return;
        }
        else if (player.EquippedItems.Contains(selectedItemOp.Item))
        {
            // if want to unequip hands it does not. so we do not play select sound.
            if (selectedItemOp.Item != DefaultWeaponHands)
            {
                selectedItemOp.PlaySelectSound();
            }

            player.RemoveFromPlayerEquipped(selectedItemOp);
            this.RemoveFromPlayerEquipped(selectedItemOp);
        }
        else
        {
            selectedItemOp.PlaySelectSound();
            this.ChangePlayerEquippedSLot(selectedItemOp);
            player.AddToPlayerEquipped(selectedItemOp.Item);
        }
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
        foreach (var p in this.Pages)
        {
            p.TogglePageComponents(false);
            p.gameObject.SetActive(false);
        }
    }

    public override void Close()
    {
        this.PageFlipper.SetActive(true);
        MyGuard.IsNotNull(this.flipPageAnimator);
        MyGuard.IsNotNull(this.bookSlideInOutAnimator);
        this.flipPageAnimator.SetTrigger("Close");
        this.bookSlideInOutAnimator.SetTrigger("Close");
        StartCoroutine(DisableInventoryAfterBookAnim());
    }

    IEnumerator PageFlip(int numberOfFlips)
    {
        this.PageFlipper.SetActive(true);
        for (int i = 0; i < numberOfFlips; i++)
        {
            MyGuard.IsNotNull(this.flipPageAnimator);
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
        MyGuard.IsNotNull(this.bookBackgroundImage);
        MyGuard.IsNotNull(this.selectedTab);
        this.bookBackgroundImage.sprite = this.selectedTab.SelectedSprite;
        var Page = GetCurrentPage();
        Page.gameObject.SetActive(true);
        Page.TogglePageComponents(true);

        if (Page is PageWithSlots pageWithSlots)
        {
            pageWithSlots.FillItemSlots(this.AllInventoryItems.Where(x => x.GetType() == pageWithSlots.TypeInPageSlots).ToList(), this.PlayerEquippedWeapon, this.playerEquippedItem, this.DefaultWeaponHands);
        }
    }

    private BookPage GetCurrentPage()
    {
        MyGuard.IsNotNull(this.selectedTab);

        if (this.selectedTab.tabType == BookTab.TabType.Gear)
        {
            return this.gearPages;
        }
        else if (this.selectedTab.tabType == BookTab.TabType.Items)
        {
            return this.ItemPages;
        }
        else if (this.selectedTab.tabType == BookTab.TabType.Save)
        {
            return this.saveMenuPages;
        }
        else
        {
            throw new NotImplementedException("cant change to that tab yet");
        }
    }

    private IEnumerator DisableInventoryAfterBookAnim()
    {
        this.GetCurrentPage().TogglePageComponents(false);
        this.BookBackGround.SetActive(false);
        Debug.Log("TODO Make the flip right page animation. and make the page with stuff on it on the right transparent so looks like its closing it over the actual book contents.");

        MyGuard.IsNotNull(this.bookSlideInOutAnimator);
        while (!this.bookSlideInOutAnimator.GetCurrentAnimatorStateInfo(0).IsName("OffscreenClosed"))
        {
            yield return null;
        }

        base.Close();
    }

    private IEnumerator WaitForBookAnimThenSetup()
    {
        MyGuard.IsNotNull(this.bookSlideInOutAnimator);

        // wait for slide in anim to finish then flip page and tabs open at the same time
        while (!this.bookSlideInOutAnimator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        if (this.selectedTab == null)
        {
            this.selectedTab = this.OnFirstOpenInventorySelectedTab;
        }

        this.BookBackGround.SetActive(true);
        this.EnableBookPage();
        StartCoroutine(PageFlip(1));
    }

    public void ChangePlayerEquippedSLot(InventorySlot selectedSlot)
    {
        MyGuard.IsNotNull(selectedSlot.Item);

        if (selectedSlot.Item is Weapon weapon)
        {
            this.gearPages.RemoveEquippedWeapon(this.PlayerEquippedWeapon, DefaultWeaponHands);
            this.PlayerEquippedWeapon = weapon;
            this.gearPages.EquipWeapon(weapon);
        }
        else if (selectedSlot.Item is SpecialItem special)
        {
            if (this.playerEquippedItem != null)
            {
                this.ItemPages.RemoveEquippedItem(this.playerEquippedItem);
            }

            this.playerEquippedItem = special;
            this.ItemPages.EquipSpecialItem(special);
        }
    }

    public void RemoveFromPlayerEquipped(InventorySlot selectedSlot)
    {
        MyGuard.IsNotNull(selectedSlot.Item);

        if (selectedSlot.Item is Weapon weapon)
        {
            this.PlayerEquippedWeapon = this.DefaultWeaponHands;
            Debug.Log("make sure to update player weapon also am i doing that?");
            this.gearPages.RemoveEquippedWeapon(weapon, this.DefaultWeaponHands);
        }
        else
        {
            this.playerEquippedItem = null;
            var item = selectedSlot.Item as SpecialItem;
            MyGuard.IsNotNull(item, "Item cannot be null here.");
            this.ItemPages.RemoveEquippedItem(item);
        }
    }
}
