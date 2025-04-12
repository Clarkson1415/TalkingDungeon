using Assets.GracesScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
#nullable enable

/// <summary>
/// Player guy
/// </summary>
[RequireComponent(typeof(UseAnimatedLayers))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDungeon : MonoBehaviour
{

    [Header("REMEMBER PLAYER STUFF IS LOADED FROM SAVE DATA MODIFY THAT")]

    [Header("Menus")]
    [SerializeField] GameObject deathScreenPrefab;
    private DialogueTextBox dialogueBox;
    private ContainerMenu ContainerMenu;
    private PauseMenu pauseMenu;
    private GameObject currentMenuOpen;

    [Header("Inventory")]
    private InventoryMenu inventoryMenu;
    [SerializeField] AudioClip InventoryOpenSound;
    [SerializeField] AudioClip InventoryClosedSound;
    public List<Item> Inventory;
    [SerializeField] private AudioSource audioSourceForInventorySounds;
    private List<Item?> equippedItems => new() { this.equippedClothing, this.equippedWeapon, this.equippedSpecialItem };
    public Item? equippedWeapon;
    public Item? equippedClothing;
    public Item? equippedSpecialItem;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 1f;
    private IInteracble? interactableInRange = null;
    private Rigidbody2D rb;
    private Vector2 direction;
    [SerializeField] private KnightState state = KnightState.PLAYERCANMOVE;
    private AudioSource footstepsSound;
    private UseAnimatedLayers animatedLayers;

    [Header("Stats")]
    public float maxWellbeing;
    public float currentWellbeing;
    public Image healthBarFillImage;
    public List<Ability> abilities;
    public float power => this.equippedItems.Contains(null) ? 0 : this.equippedItems.Sum(x => x.PowerStat);
    public float defence => this.equippedItems.Contains(null) ? 0 : this.equippedItems.Sum(x => x.DefenceStat);

    [SerializeField] AudioSource DeathSFX;

    /// <summary>
    /// Flag Set to true ONLY WHEN there is an interactable in range. <see cref="OnInteract(InputAction.CallbackContext)"/>
    /// </summary>
    private bool InteractFlagSet;

    private enum KnightState
    {
        INDIALOGUE,
        PLAYERCANMOVE,
        InItemContainer,
        INPAUSEMENU,
        ININVENTORY,
        InTurnBased,
    }

    private void Awake()
    {
        // has to be active to find it so set all menus active then off saves setting player serialised files in every scene and idk if it i could even do that now I have persistant data 
        var canvas = FindFirstObjectByType<Canvas>();

        foreach (Transform child in canvas.transform)
        {
            if (child?.gameObject == null)
            {
                continue;
            }

            child.gameObject.SetActive(true);
        }

        this.dialogueBox = FindFirstObjectByType<DialogueTextBox>();
        this.dialogueBox.gameObject.SetActive(false);

        this.ContainerMenu = FindFirstObjectByType<ContainerMenu>();
        this.ContainerMenu.gameObject.SetActive(false);

        pauseMenu = FindFirstObjectByType<PauseMenu>();
        this.pauseMenu.gameObject.SetActive(false);

        inventoryMenu = FindFirstObjectByType<InventoryMenu>();
        this.inventoryMenu.gameObject.SetActive(false);

        animatedLayers = GetComponent<UseAnimatedLayers>();
        this.rb = GetComponent<Rigidbody2D>();
        footstepsSound = GetComponentInChildren<AudioSource>();
        this.currentMenuOpen = this.pauseMenu.gameObject;

        this.healthBarFillImage = FindFirstObjectByType<HealthBarFill>().GetComponent<Image>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        // on awake see if there is another save data in the scene (if there is more than 1) this one also counts
        // if there is than this one called awake and is the duplicate set in the future scene that is now loaded
        // and destroy this object. and the save data from previous level should be used.
        // all levels have one of these so that I can playtest from each level.
        var saveDataObjects = FindObjectsByType<PersistantSaveData>(FindObjectsSortMode.None);

        // if only 1 then it's fine to load save data onto player it means I started in the scene if thers only 1 saveData upon start()
        if (saveDataObjects.Count() == 1)
        {
            return;
        }

        // keep the saveD with the highest scene load count - that is the one that's been used since started this playthough
        var orderedByLeastToMost = saveDataObjects.OrderBy(x => x.scenesGoneThrough);

        // lowest scenes gone though is first, and highest last
        var toKeep = orderedByLeastToMost.Last();
        foreach (var item in orderedByLeastToMost)
        {
            if (item != toKeep)
            {
                Destroy(item.gameObject);
            }
        }

        this.currentWellbeing = toKeep.currentWellbeing;
        this.maxWellbeing = toKeep.maxWellbeing;
        this.abilities = toKeep.abilities;
        this.Inventory = toKeep.Inventory;
        this.equippedClothing = toKeep.equippedClothing;
        this.equippedSpecialItem = toKeep.equippedSpecialItem;
        this.equippedWeapon = toKeep.equippedWeapon;

        this.healthBarFillImage.fillAmount = this.currentWellbeing / this.maxWellbeing;
    }

    private void StartInteraction()
    {
        if (this.interactableInRange is IHasDialogue interactableWithDialogue && interactableWithDialogue != null)
        {
            // if the object you start talking to is moving it can move out of range and causes on trigger exit player wont be able to spacebar out of dialogue.
            // stop moving on start interaction and finish on end interaction
            if (this.interactableInRange is WalkingBackAndForthUnit movingNPC)
            {
                movingNPC.IsInDialogue = true;
            }

            currentMenuOpen = this.dialogueBox.gameObject;
            this.dialogueBox.gameObject.SetActive(true);
            dialogueBox.PlayerInteractFlagSet = true;
            this.dialogueBox.BeginDialogue(interactableWithDialogue.GetFirstDialogueSlide());
            this.state = KnightState.INDIALOGUE;
        }
        else if (this.interactableInRange is ItemContainer chest && chest != null)
        {
            currentMenuOpen = this.ContainerMenu.gameObject;
            this.ContainerMenu.gameObject.SetActive(true);
            chest.GetComponent<Animator>().SetTrigger("Opened");
            chest.PlayOpenSound();
            this.ContainerMenu.PopulateContainer(chest.loot);
            this.state = KnightState.InItemContainer;
        }

        // TODO: add more interactables here

        this.StopMovement();
    }

    private void StopMovement()
    {
        // Stop animations
        this.animatedLayers.SetFloats("YVel", 0);
        this.animatedLayers.SetFloats("XVel", 0);
        this.animatedLayers.SetBools("Moving", false);

        // stop movement
        footstepsSound.Stop();
        this.rb.velocity = Vector2.zero;
        this.direction = Vector2.zero;
    }

    private void FixedUpdate()
    {
        switch (this.state)
        {
            case KnightState.PLAYERCANMOVE:
                this.rb.velocity = direction * movementSpeed;

                if (escKeyFlag)
                {
                    escKeyFlag = false;
                    this.pauseMenu.gameObject.SetActive(true);
                    this.pauseMenu.StartPauseMenu();
                    this.state = KnightState.INPAUSEMENU;
                    StopMovement();
                    this.currentMenuOpen = this.pauseMenu.gameObject;
                }
                if (iKeyFlag)
                {
                    iKeyFlag = false;
                    this.currentMenuOpen = inventoryMenu.gameObject;
                    this.currentMenuOpen.SetActive(true);
                    inventoryMenu.OpenInventory(Inventory);
                    audioSourceForInventorySounds.clip = this.InventoryOpenSound;
                    audioSourceForInventorySounds.Play();

                    this.inventoryMenu.UpdatePlayerStatsDisplay(this.power, this.defence);
                    this.inventoryMenu.UpdatePlayerWellBeingDislpay((int)this.currentWellbeing);

                    StopMovement();
                    this.state = KnightState.ININVENTORY;
                }
                if (this.InteractFlagSet)
                {
                    this.InteractFlagSet = false;

                    if (this.interactableInRange == null)
                    {
                        return;
                    }

                    StartInteraction();
                }
                break;
            case KnightState.INDIALOGUE: // TODO TEST this state I think i fucked it
                if (this.InteractFlagSet)
                {
                    this.InteractFlagSet = false;
                    dialogueBox.PlayerInteractFlagSet = true;
                }
                else if (this.dialogueBox.finishedInteractionFlag)
                {
                    this.dialogueBox.finishedInteractionFlag = false;
                    EndDialogue();
                }
                break;
            case KnightState.InItemContainer:
                if (escKeyFlag)
                {
                    escKeyFlag = false;
                    this.ContainerMenu.gameObject.SetActive(false);
                    this.ContainerMenu.Close();

                    if (this.interactableInRange is ItemContainer chest && chest != null)
                    {
                        chest.PlayClosedSound();
                    }

                    this.state = KnightState.PLAYERCANMOVE;
                    this.currentMenuOpen = this.pauseMenu.gameObject;
                }
                else if (this.InteractFlagSet)
                {
                    this.InteractFlagSet = false;
                    var itemOpButton = this.ContainerMenu.GetCurrentSelected();

                    if (itemOpButton == null)
                    {
                        return;
                    }

                    if (itemOpButton.Item == null)
                    {
                        return;
                    }

                    if (this.interactableInRange is ItemContainer chest && chest != null)
                    {
                        chest.loot.Remove(itemOpButton.Item);
                        this.Inventory.Add(itemOpButton.Item);
                    }

                    this.ContainerMenu.RemoveOldItem(itemOpButton);
                }
                break;
            case KnightState.INPAUSEMENU:
                if (this.InteractFlagSet)
                {
                    this.InteractFlagSet = false;
                    var button = this.pauseMenu.GetSelectedButton();

                    var option = button.GetComponent<MenuButton>();

                    Debug.Log($"selected {option}");
                }
                if (escKeyFlag)
                {
                    escKeyFlag = false;
                    this.pauseMenu.gameObject.SetActive(false);
                    this.pauseMenu.Close();
                    this.state = KnightState.PLAYERCANMOVE;
                }
                break;
            case KnightState.ININVENTORY:
                if (iKeyFlag)
                {
                    // only use esc to close menues for now
                    iKeyFlag = false;
                }
                if (escKeyFlag)
                {
                    escKeyFlag = false;
                    this.currentMenuOpen.GetComponent<InventoryMenu>().Close();
                    audioSourceForInventorySounds.clip = this.InventoryClosedSound;
                    audioSourceForInventorySounds.Play();
                    this.currentMenuOpen.SetActive(false);
                    this.currentMenuOpen = pauseMenu.gameObject;
                    this.state = KnightState.PLAYERCANMOVE;
                }
                if (this.InteractFlagSet)
                {
                    this.InteractFlagSet = false;
                    var buttonGameObject = this.inventoryMenu.GetSelectedButton();
                    var selectedItemOp = buttonGameObject.GetComponent<ItemOptionButton>();

                    if (selectedItemOp.Item == null)
                    {
                        return;
                        // nothing happens if empty square selected.
                    }
                    else if (this.equippedItems.Contains(selectedItemOp.Item))
                    {
                        selectedItemOp.ToggleEquipGraphic();
                        this.inventoryMenu.RemoveEquippedItem(selectedItemOp.Item);
                        RemoveFromPlayerEquipped(selectedItemOp.Item);
                    }
                    else if (IsValidEquip(selectedItemOp.Item))
                    {
                        selectedItemOp.ToggleEquipGraphic();
                        this.inventoryMenu.AddOrReplaceEquipped(selectedItemOp.Item);
                        AddToPlayerEquipped(selectedItemOp.Item);
                    }
                    else
                    {
                        Log.Print("can't equip that");
                        return;
                    }

                    // need to store in this script for battles
                    this.inventoryMenu.UpdatePlayerStatsDisplay(this.power, this.defence);
                    this.inventoryMenu.UpdatePlayerWellBeingDislpay((int)this.currentWellbeing);
                }
                break;
            case KnightState.InTurnBased:

                break;
            default:
                this.state = KnightState.PLAYERCANMOVE;
                break;
        }
    }

    /// <summary>
    /// Idk if comparing name string is neccessary though I
    /// </summary>
    /// <param name="itemToRemove"></param>
    private void RemoveFromPlayerEquipped(Item itemToRemove)
    {
        if (itemToRemove.name == equippedWeapon?.name)
        {
            equippedWeapon = null;
        }
        else if (itemToRemove.name == equippedClothing?.name)
        {
            equippedClothing = null;
        }
        else if (itemToRemove.name == equippedSpecialItem?.name)
        {
            equippedSpecialItem = null;
        }
    }

    private void AddToPlayerEquipped(Item itemToEquip)
    {
        switch (itemToEquip.Type)
        {
            case ItemType.Weapon:
                this.equippedWeapon = itemToEquip;
                break;
            case ItemType.Clothing:
                this.equippedClothing = itemToEquip;
                break;
            case ItemType.SpecialItem:
                this.equippedSpecialItem = itemToEquip;
                break;
            default:
                throw new ArgumentOutOfRangeException("No type found fo add to player quip.");
        }
    }

    /// <summary>
    /// TODO, was thinkging of having player stat /level blocks for items but now probs not
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private bool IsValidEquip(Item? item)
    {
        bool isValid = true;
        return isValid;
    }

    public bool isHealthBarDoingAnim;

    public void TakeDamage(float damage)
    {
        this.currentWellbeing -= damage;
        // if current damage will kill player make it go fast
        if (this.currentWellbeing <= 0)
        {
            StartCoroutine(animateHealthLoss(0.1f, damage));
            var canvas = FindObjectOfType<Canvas>();
            Instantiate(this.deathScreenPrefab, canvas.transform);
            // play death sound
            this.DeathSFX.Play();
        }
        else
        {
            StartCoroutine(animateHealthLoss(0.5f, damage));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="speedToFinish">seconds time for health bar to go to where after damage puts it</param>
    /// <returns></returns>
    IEnumerator animateHealthLoss(float speedToFinish, float damage)
    {
        Log.Print("started animating health loss player");
        this.isHealthBarDoingAnim = true;
        var timeIncrement = 0.1f;
        var damagePerTimeIncrement = damage / (speedToFinish / timeIncrement);
        while (this.healthBarFillImage.fillAmount > Mathf.Clamp((this.currentWellbeing / this.maxWellbeing), 0, 1))
        {
            this.healthBarFillImage.fillAmount -= (damagePerTimeIncrement / 100);
            yield return new WaitForSeconds(timeIncrement);
        }

        this.isHealthBarDoingAnim = false;
        Log.Print("finished animating health loss player");
    }

    public void OnIKey(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }

        iKeyFlag = true;
    }

    private bool iKeyFlag;
    private bool escKeyFlag;

    public void OnMenuCancel(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }

        // TODO: might want to change this later if want to open pause menu over dialogue but for now this is fastest solution
        // dont open pause menu in dialogue.
        if (this.state == KnightState.INDIALOGUE)
        {
            return;
        }

        escKeyFlag = true;
    }

    private void EndDialogue()
    {
        // if the object you start talking to also moves our and causes on trigger exit player wont be able to spacebar out of dialogue.
        // this is to allow it to go back to moving state again.
        if (this.interactableInRange is WalkingBackAndForthUnit movingNPC)
        {
            movingNPC.IsInDialogue = false;
        }

        this.state = KnightState.PLAYERCANMOVE;
    }

    public void OnNavigateOrMove(InputAction.CallbackContext context)
    {
        // TODO a better way would have each button fire their own OnNavigatedToo event when they are highlighted to play it's sound.
        if (this.state != KnightState.PLAYERCANMOVE)
        {
            return;
        }

        // if in interactin dont update and Return early this stops animation from playing when you're in dialogue
        // another option is to diable and enable the PLayer Move action map and re enable.

        this.direction = context.ReadValue<Vector2>();

        // TODO for an Running as well. need to add an IsRunning boolean triggered in the OnRun function. do I want the player to have to be running first? then press sprint?

        if (context.started)
        {
            footstepsSound.Play();

            Log.Print("on move started");
            Log.Print($"dir: {this.direction.x}, {this.direction.y}");
            this.animatedLayers.SetFloats("LastXDir", this.direction.x);
            this.animatedLayers.SetFloats("LastYDir", this.direction.y);
        }
        else if (context.performed)
        {

        }
        else if (context.canceled)
        {
            footstepsSound.Stop();
            Log.Print("on move cancelled");
        }

        UpdateAnimationFloats();

        // true when started and in performed state, false on cancel (finish or relase key)
        Log.Print($"context: {!context.canceled}");
        this.animatedLayers.SetBools("Moving", !context.canceled);
    }

    private void UpdateAnimationFloats()
    {
        this.animatedLayers.SetFloats("YDir", direction.y);
        this.animatedLayers.SetFloats("XDir", direction.x);
    }

    /// <summary>
    /// TODO Run animations are not gotten from the sprite sheet
    /// </summary>
    /// <param name="context"></param>
    public void OnRunKey(InputAction.CallbackContext context)
    {
        // if in interactin dont update and Return early this stops animation from playing when you're in dialogue
        // another option is to diable and enable the PLayer Move action map and re enable.
        if (this.state != KnightState.PLAYERCANMOVE)
        {
            return;
        }

        this.direction = context.ReadValue<Vector2>();

        if (context.started)
        {
            Log.Print("RUN started");
        }
        else if (context.performed)
        {

        }
        else if (context.canceled)
        {
            Log.Print("Run cancelled");
        }

        // true when started and in performed state, false on cancel (finish or relase key)
        this.animatedLayers.SetBools("Running", !context.canceled);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.state != KnightState.PLAYERCANMOVE)
        {
            return;
        }

        if (collision.TryGetComponent<IInteracble>(out var NPCWithDialogue))
        {
            Log.Print("can interact with" + collision.name);
            this.interactableInRange = NPCWithDialogue;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IInteracble>(out var interactable) && (interactable == this.interactableInRange))
        {
            // empty interactable so cant be retriggered when out of range.
            this.interactableInRange = null;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }

        Log.Print("Interact flag set");
        this.InteractFlagSet = true;

        if (interactableInRange == null)
        {
            return;
        }
    }
}
