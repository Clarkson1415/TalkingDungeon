using Assets.GracesScripts;
using Assets.GracesScripts.ScriptableObjects;
using Assets.GracesScripts.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static SaveGameUtility;
#nullable enable

/// <summary>
/// Player guy
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDungeon : Unit
{
    [SerializeField] private float movementSpeed = 1f;
    private Rigidbody2D? rb;
    private Vector2 direction;
    [SerializeField] private KnightState startingState = KnightState.PLAYERCANMOVE;
    [HideInInspector] private KnightState state;
    private AudioSource? footstepsSound;

    [Header("Menus")]
    [SerializeField] GameObject deathScreenPrefab;
    private DialogueTextBox? dialogueBox;
    private ContainerMenu? ContainerMenu;
    private PauseMenu? pauseMenu;
    private Menu? menuToUseNext;
    private InventoryMenu? inventoryMenu;

    [Header("Death")]
    [SerializeField] AudioSource LevelMusic;
    [SerializeField] AudioSource DeathSFX;

    public List<string> scenesTraversed = new();

    [HideInInspector] public IInteracble? InteractableInRange { get; private set; } = null;

    [Header("EnemyBattleLoader")]
    [SerializeField] private GameObject enemyLoaderPrefab;
    [HideInInspector] public EnemyLoader enemyLoader;

    /// <summary>
    /// Flag Set to true ONLY WHEN there is an interactable in range. <see cref="OnInteract(InputAction.CallbackContext)"/>
    /// </summary>
    private bool InteractFlagSet;
    private bool iKeyFlag;
    private bool escKeyFlag;

    private void ResetFlags()
    {
        InteractFlagSet = false;
        iKeyFlag = false;
        escKeyFlag = false;
    }

    private enum KnightState
    {
        INDIALOGUE,
        PLAYERCANMOVE,
        InItemContainer,
        INPAUSEMENU,
        ININVENTORY,
        InTurnBased,
    }

    private void Start()
    {
        var enemyLoaders = FindObjectsByType<EnemyLoader>(FindObjectsSortMode.None);
        if (enemyLoaders.Length == 0)
        {
            enemyLoader = Instantiate(enemyLoaderPrefab).GetComponent<EnemyLoader>();
        }
        else
        {
            this.enemyLoader = enemyLoaders[0];
        }

        this.state = startingState;

        switch (PlayerPrefs.GetString(SaveKeys.GameState))
        {
            case SaveGameState.RegularSceneChange:
                SaveGameUtility.LoadSaveNotPosition(this);
                break;
            case SaveGameState.LoadingSave:
                SaveGameUtility.LoadSaveNotPosition(this);
                SaveGameUtility.LoadPlayerPosition(this);
                break;
            case SaveGameState.BattleLost:
                SaveGameUtility.LoadSaveNotPosition(this);
                SaveGameUtility.LoadPlayerPosition(this);
                break;
            case SaveGameState.QuittingToTitle:
                break;
            case SaveGameState.BattleWon:
                SaveGameUtility.LoadSaveNotPosition(this);
                break;
            case SaveGameState.NewGame:
                break;
            case SaveGameState.BattleRunAwaySuccess:
                SaveGameUtility.LoadSaveNotPosition(this);
                SaveGameUtility.LoadPlayerPosition(this);
                break;
            case SaveGameState.StartedBattle:
                SaveGameUtility.LoadSaveNotPosition(this);
                break;
            default:
                Debug.LogWarning("Game state not valid. If starting from a scene not Title screen than ignore this.");
                break;
        }

        // Save upon entering new scene do not save if in battle scene though
        if (SceneManager.GetActiveScene().name != TalkingDungeonScenes.Battle)
        {
            SaveGameUtility.SaveGame(this);
        }

        this.LoadPlayerComponents();

        if (Abilities.Count < 1)
        {
            throw new ArgumentException("canont have less than 1 ability at least would have push on hands.");
        }

        if (this.startingState == KnightState.InTurnBased)
        {
            MyGuard.IsNotNull(animatedLayers);
            animatedLayers.SetTriggers("StartFight");
        }
    }

    /// <summary>
    /// If the scene was loaded from a save when update runs rb will still be null beacuse setupPlayer hasnt been called in awake. so wait for this to be true to do update stuff. As setupPlayer will be aclled y menubutton after scene has loaded.
    /// </summary>
    /// <returns></returns>
    private bool AreComponentsLoaded()
    {
        return this.rb != null;
    }

    private void LoadPlayerComponents()
    {
        this.rb = GetComponent<Rigidbody2D>();
        footstepsSound = GetComponentInChildren<AudioSource>();
        var menuReferences = FindObjectOfType<MenuReferences>();
        this.dialogueBox = menuReferences.dialogueTextBox;
        this.pauseMenu = menuReferences.PauseMenu;
        this.ContainerMenu = menuReferences.containerMenu;
        this.inventoryMenu = menuReferences.Inventory;
        MyGuard.IsNotNull(this.pauseMenu);
        this.menuToUseNext = this.pauseMenu;
        this.healthBarFill = FindFirstObjectByType<PlayerHealthBarFill>().GetComponent<Image>();
        MyGuard.IsNotNull(healthBarFill);
        this.HealthBarObject = this.healthBarFill.transform.parent.gameObject;
        this.healthBarFill.fillAmount = this.currentHealth / this.maxHealth;
    }

    private void StartInteraction()
    {
        if (this.InteractableInRange is IHasDialogue interactableWithDialogue && interactableWithDialogue != null)
        {
            // if the object you start talking to is moving it can move out of range and causes on trigger exit player wont be able to spacebar out of dialogue.
            // stop moving on start interaction and finish on end interaction
            if (this.InteractableInRange is WalkingBackAndForthUnit movingNPC)
            {
                movingNPC.IsInDialogue = true;
            }

            menuToUseNext = this.dialogueBox;
            var npc = interactableWithDialogue as Unit_NPC;
            MyGuard.IsNotNull(npc);
            MyGuard.IsNotNull(this.dialogueBox);
            this.dialogueBox.gameObject.SetActive(true);
            this.dialogueBox.BeginDialogue(interactableWithDialogue.GetFirstDialogueSlide(), npc);
            this.state = KnightState.INDIALOGUE;
            this.StopMovement();
        }
        else if (this.InteractableInRange is ItemContainer chest && chest != null)
        {
            MyGuard.IsNotNull(ContainerMenu);
            menuToUseNext = this.ContainerMenu;
            this.ContainerMenu.gameObject.SetActive(true);
            chest.GetComponent<Animator>().SetTrigger("Opened");
            chest.PlayOpenSound();
            this.ContainerMenu.PopulateContainer(chest.Loot);
            this.state = KnightState.InItemContainer;
            this.StopMovement();
        }
    }

    private void StopMovement()
    {
        // Stop animations
        MyGuard.IsNotNull(animatedLayers);
        this.animatedLayers.SetBools("Moving", false);

        // stop movement
        MyGuard.IsNotNull(footstepsSound);
        MyGuard.IsNotNull(this.rb);
        footstepsSound.Stop();
        this.rb.velocity = Vector2.zero;
        this.direction = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (!AreComponentsLoaded())
        {
            return;
        }

        this.HandleState();

        ResetFlags();
    }

    private void OpenPauseMenu()
    {
        MyGuard.IsNotNull(this.pauseMenu);
        this.pauseMenu.gameObject.SetActive(true);
        this.pauseMenu.StartPauseMenu();
        this.state = KnightState.INPAUSEMENU;
        StopMovement();
        this.menuToUseNext = this.pauseMenu;
    }

    private void OpenInventory()
    {
        MyGuard.IsNotNull(this.inventoryMenu);
        iKeyFlag = false;
        this.menuToUseNext = inventoryMenu;
        this.menuToUseNext.gameObject.SetActive(true);
        inventoryMenu.OpenInventory(Inventory, this.equippedWeapon, this.equippedSpecialItem);
        StopMovement();
        this.state = KnightState.ININVENTORY;
    }

    private void ExitChest()
    {
        MyGuard.IsNotNull(this.ContainerMenu);
        this.ContainerMenu.Close();

        if (this.InteractableInRange is ItemContainer chest && chest != null)
        {
            chest.PlayClosedSound();
        }

        this.state = KnightState.PLAYERCANMOVE;
        MyGuard.IsNotNull(this.pauseMenu);
        this.menuToUseNext = this.pauseMenu;
    }

    private void HandleState()
    {
        switch (this.state)
        {
            case KnightState.PLAYERCANMOVE:
                this.rb.velocity = direction * movementSpeed;
                if (escKeyFlag)
                {
                    OpenPauseMenu();
                }
                else if (iKeyFlag)
                {
                    OpenInventory();
                }
                else if (this.InteractFlagSet && this.InteractableInRange != null)
                {
                    StartInteraction();
                }
                break;
            case KnightState.INDIALOGUE:
                MyGuard.IsNotNull(dialogueBox);
                if (this.InteractFlagSet)
                {
                    this.dialogueBox.InteractFlag = true;
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
                    ExitChest();
                }
                else if (this.ContainerMenu.TellPlayerContainerButtonClicked)
                {
                    this.ContainerMenu.TellPlayerContainerButtonClicked = false;
                    var selected = this.ContainerMenu.GetSelectedButton();
                    var itemOpButton = selected.GetComponent<InventorySlot>();
                    itemOpButton.PlaySelectSound();
                    if (itemOpButton.Item == null)
                    {
                        return;
                    }

                    if (this.InteractableInRange is ItemContainer chest && chest != null)
                    {
                        chest.Loot.Remove(itemOpButton.Item);
                        this.Inventory.Add(itemOpButton.Item);
                        this.ContainerMenu.RemoveOldItem(itemOpButton);
                    }
                }
                break;
            case KnightState.INPAUSEMENU:
                if (escKeyFlag)
                {
                    escKeyFlag = false;
                    MyGuard.IsNotNull(this.pauseMenu);
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
                    MyGuard.IsNotNull(menuToUseNext);
                    MyGuard.IsNotNull(pauseMenu);
                    this.menuToUseNext.GetComponent<InventoryMenu>().Close();
                    this.menuToUseNext = pauseMenu;
                    this.state = KnightState.PLAYERCANMOVE;
                }
                if (this.InteractFlagSet)
                {
                    this.InteractFlagSet = false;
                    MyGuard.IsNotNull(this.inventoryMenu);
                    var buttonGameObject = this.inventoryMenu.GetSelectedButton();

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
                    else if (this.EquippedItems.Contains(selectedItemOp.Item))
                    {
                        // if want to unequip hands it does not. so we do not play select sound.
                        if (selectedItemOp.Item != DefaultWeaponHands)
                        {
                            selectedItemOp.PlaySelectSound();
                        }

                        RemoveFromPlayerEquipped(selectedItemOp);
                        this.inventoryMenu.RemoveFromPlayerEquipped(selectedItemOp);
                    }
                    else if (IsValidEquip(selectedItemOp.Item))
                    {
                        selectedItemOp.PlaySelectSound();
                        this.inventoryMenu.AddToPlayerEquipped(selectedItemOp);
                        AddToPlayerEquipped(selectedItemOp.Item);
                    }
                    else
                    {
                        throw new ArgumentException("Item invalid??");
                    }
                }
                break;
            case KnightState.InTurnBased:
                // controlled by BattleUI and never exits.
                // We change states next when battle is won and the new scene is loaded.
                break;
            default:
                this.state = KnightState.PLAYERCANMOVE;
                break;
        }
    }

    private void RemoveFromPlayerEquipped(InventorySlot itemButton)
    {
        var itemToRemove = itemButton.Item;
        MyGuard.IsNotNull(itemToRemove);
        MyGuard.IsNotNull(inventoryMenu);

        if (itemToRemove.name == equippedWeapon.name)
        {
            equippedWeapon = DefaultWeaponHands;
        }
        else if (equippedSpecialItem != null && (itemToRemove.name == equippedSpecialItem.name))
        {
            equippedSpecialItem = null;
        }
    }

    /// <summary>
    /// TODO a better way for using this equipped items and inventory have a looksie
    /// </summary>
    /// <param name="itemToEquip"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void AddToPlayerEquipped(DungeonItem itemToEquip)
    {
        MyGuard.IsNotNull(this.inventoryMenu);

        if (itemToEquip is Weapon w)
        {
            this.equippedWeapon = w;
        }
        else if (itemToEquip is SpecialItem special)
        {
            this.equippedSpecialItem = special;
        }
    }

    /// <summary>
    /// TODO, was thinkging of having player stat /level blocks for items but now probs not
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private bool IsValidEquip(DungeonItem item)
    {
        MyGuard.IsNotNull(item);
        bool isValid = true;
        return isValid;
    }

    protected override void Die()
    {
        var canvas = FindObjectOfType<Canvas>();
        Instantiate(this.deathScreenPrefab, canvas.transform);
        this.DeathSFX.Play();
        this.LevelMusic.Stop();
    }

    public void OnIKey(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }

        iKeyFlag = true;
    }

    /// <summary>
    /// When esc key is pressed.
    /// </summary>
    /// <param name="context"></param>
    public void OnMenuCancel(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }

        escKeyFlag = true;
    }

    private void EndDialogue()
    {
        // if the object you start talking to also moves our and causes on trigger exit player wont be able to spacebar out of dialogue.
        // this is to allow it to go back to moving state again.
        if (this.InteractableInRange is WalkingBackAndForthUnit movingNPC)
        {
            movingNPC.IsInDialogue = false;
        }

        this.state = KnightState.PLAYERCANMOVE;
    }

    public void OnNavigateOrMove(InputAction.CallbackContext context)
    {
        if (this.state != KnightState.PLAYERCANMOVE)
        {
            return;
        }

        // if in interactin dont update and Return early this stops animation from playing when you're in dialogue
        // another option is to diable and enable the PLayer Move action map and re enable.
        this.direction = context.ReadValue<Vector2>();

        // TODO for an Running as well. need to add an IsRunning boolean triggered in the OnRun function. do I want the player to have to be running first? then press sprint?
        MyGuard.IsNotNull(footstepsSound);
        MyGuard.IsNotNull(animatedLayers);

        if (context.started)
        {
            footstepsSound.Play();
        }
        else if (context.performed)
        {

        }
        else if (context.canceled)
        {
            footstepsSound.Stop();
            this.animatedLayers.SetFloats("LastXDir", this.direction.x);
            this.animatedLayers.SetFloats("LastYDir", this.direction.y);
            //Log.Print("on move cancelled");
        }

        UpdateAnimationFloats();

        // true when started and in performed state, false on cancel (finish or relase key)
        //Log.Print($"context: {!context.canceled}");
        this.animatedLayers.SetBools("Moving", !context.canceled);
    }

    private void UpdateAnimationFloats()
    {
        MyGuard.IsNotNull(animatedLayers);
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

        MyGuard.IsNotNull(animatedLayers);
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
            this.InteractableInRange = NPCWithDialogue;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IInteracble>(out var interactable) && (interactable == this.InteractableInRange))
        {
            // empty interactable so cant be retriggered when out of range.
            this.InteractableInRange = null;
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
    }
}
