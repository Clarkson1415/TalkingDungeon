using Assets.GracesScripts;
using Assets.GracesScripts.ScriptableObjects;
using System;
using System.Collections;
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
[RequireComponent(typeof(UseAnimatedLayers))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDungeon : Unit
{
    private Weapon defaultWeaponHands;
    [SerializeField] private float movementSpeed = 1f;
    private Rigidbody2D rb;
    private Vector2 direction;
    [SerializeField] private KnightState startingState = KnightState.PLAYERCANMOVE;
    [HideInInspector] private KnightState state;
    private AudioSource? footstepsSound;
    private UseAnimatedLayers? animatedLayers;

    [Header("Menus")]
    [SerializeField] GameObject deathScreenPrefab;
    private DialogueTextBox? dialogueBox;
    private ContainerMenu? ContainerMenu;
    private PauseMenu? pauseMenu;
    private GameObject? menuToUseNext;
    private InventoryMenu? inventoryMenu;

    [Header("Death")]
    [SerializeField] AudioSource LevelMusic;
    [SerializeField] AudioSource DeathSFX;

# if UNITY_EDITOR
    [Header("For While Testing In Unity Editor its made public but do not remove just make hide in inspector.")]
    public List<string> scenesTraversed = new();
# endif
    [HideInInspector] public IInteracble? InteractableInRange { get; private set; } = null;

    [Header("EnemyBattleLoader")]
    [SerializeField] private GameObject enemyLoaderPrefab;
    [HideInInspector] public EnemyLoader enemyLoader;

    /// <summary>
    /// Flag Set to true ONLY WHEN there is an interactable in range. <see cref="OnInteract(InputAction.CallbackContext)"/>
    /// </summary>
    private bool InteractFlagSet;
    private bool onPointerClickFlag;
    private bool iKeyFlag;
    private bool escKeyFlag;

    private void ResetFlags()
    {
        InteractFlagSet = false;
        onPointerClickFlag = false;
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

    private void Awake()
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

        StartCoroutine(WaitForSceneLoadedThenLoadComponents());

#if UNITY_EDITOR // save whats set in the inspector then load it 
        if (SceneManager.GetActiveScene().name != TalkingDungeonScenes.Battle)
        {
            SaveGameUtility.SaveGame(this);
        }
# endif

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(SaveKeys.LastScene))) // if loading from save load otherwise its not got player prefs for a new game
        {
            SaveGameUtility.LoadSaveDataFromLastScene(this);
        }

        // do not save if in battle scene though
        if (SceneManager.GetActiveScene().name != TalkingDungeonScenes.Battle)
        {
            SaveGameUtility.SaveGame(this);
        }

        this.healthBarFill.fillAmount = this.currentHealth / this.maxHealth;

        if (Abilities.Count < 1)
        {
            throw new ArgumentException("canont have less than 1 ability at least would have push on hands.");
        }
    }

    /// <summary>
    /// Called after all the save data has been loaded.
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForSceneLoadedThenLoadComponents()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        var dialogueBox = FindObjectOfType<DialogueTextBox>();

        while ((currentScene.name != "TurnBased") && dialogueBox == null)
        {
            if (currentScene.name == "TurnBased")
            {
                yield return null;
                break;
            }

            // if loading from save the dialogue Box will be null in awake. and if loading for the first time for a player playing the game it will not be null and can continue;
            ActivateAllCanvasObjects();
            dialogueBox = FindObjectOfType<DialogueTextBox>();
            // if it is a turn based scene we should load everything as turn based scenes are alwyas loaded from another scene before not save data
            yield return null;
        }

        this.InitializeMenusAndSaveText();
        this.LoadPlayerComponents();
    }

    /// <summary>
    /// If the scene was loaded from a save when update runs rb will still be null beacuse setupPlayer hasnt been called in awake. so wait for this to be true to do update stuff. As setupPlayer will be aclled y menubutton after scene has loaded.
    /// </summary>
    /// <returns></returns>
    private bool AreComponentsLoaded()
    {
        if (this.rb == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void ActivateAllCanvasObjects()
    {
        var canvas = FindFirstObjectByType<Canvas>();

        foreach (Transform child in canvas.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void InitializeMenusAndSaveText()
    {
        // has to be active to find it so set all menus active then off saves setting player serialised files in every scene and idk if it i could even do that now I have persistant data 
        ActivateAllCanvasObjects();

        Scene currentScene = SceneManager.GetActiveScene();

        this.dialogueBox = FindFirstObjectByType<DialogueTextBox>();
        if (this.dialogueBox == null && (currentScene.name == "TurnBased"))
        {
            Debug.Log("no Inventory Menu in Turn Based Scene");
        }
        else
        {
            MyGuard.IsNotNull(this.dialogueBox);
            this.dialogueBox.gameObject.SetActive(false);
        }

        this.ContainerMenu = FindFirstObjectByType<ContainerMenu>();
        if (this.ContainerMenu == null && (currentScene.name == "TurnBased"))
        {
            Debug.Log("As expected no Inventory Menu in Turn Based Scene");
        }
        else
        {
            MyGuard.IsNotNull(this.ContainerMenu);
            this.ContainerMenu.gameObject.SetActive(false);
        }

        pauseMenu = FindFirstObjectByType<PauseMenu>();
        this.pauseMenu.gameObject.SetActive(false);

        inventoryMenu = FindFirstObjectByType<InventoryMenu>();
        if (inventoryMenu == null && (currentScene.name == "TurnBased"))
        {
            Debug.Log("no Inventory Menu in Turn Based Scene");
        }
        else
        {
            MyGuard.IsNotNull(this.inventoryMenu);
            this.inventoryMenu.gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    private void LoadPlayerComponents()
    {
        animatedLayers = GetComponent<UseAnimatedLayers>();
        this.rb = GetComponent<Rigidbody2D>();
        footstepsSound = GetComponentInChildren<AudioSource>();
        MyGuard.IsNotNull(this.pauseMenu);
        this.menuToUseNext = this.pauseMenu.gameObject;
        this.healthBarFill = FindFirstObjectByType<HealthBarFill>().GetComponent<Image>();
        this.HealthBarObject = this.healthBarFill.transform.parent.gameObject;
        this.defaultWeaponHands = Resources.Load<Weapon>("Items/Weapon/Hands");
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

            MyGuard.IsNotNull(this.dialogueBox);
            menuToUseNext = this.dialogueBox.gameObject;
            this.dialogueBox.gameObject.SetActive(true);
            dialogueBox.PlayerInteractFlagSet = true;
            this.dialogueBox.BeginDialogue(interactableWithDialogue.GetFirstDialogueSlide(), interactableWithDialogue as Unit_NPC);
            this.state = KnightState.INDIALOGUE;
        }
        else if (this.InteractableInRange is ItemContainer chest && chest != null)
        {
            MyGuard.IsNotNull(ContainerMenu);
            menuToUseNext = this.ContainerMenu.gameObject;
            this.ContainerMenu.gameObject.SetActive(true);
            chest.GetComponent<Animator>().SetTrigger("Opened");
            chest.PlayOpenSound();
            this.ContainerMenu.PopulateContainer(chest.Loot);
            this.state = KnightState.InItemContainer;
        }

        this.StopMovement();
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

        switch (this.state)
        {
            case KnightState.PLAYERCANMOVE:
                this.rb.velocity = direction * movementSpeed;

                if (escKeyFlag)
                {
                    escKeyFlag = false;
                    MyGuard.IsNotNull(this.pauseMenu);
                    this.pauseMenu.gameObject.SetActive(true);
                    this.pauseMenu.StartPauseMenu();
                    this.state = KnightState.INPAUSEMENU;
                    StopMovement();
                    this.menuToUseNext = this.pauseMenu.gameObject;
                }
                if (iKeyFlag)
                {
                    MyGuard.IsNotNull(this.inventoryMenu);
                    iKeyFlag = false;
                    this.menuToUseNext = inventoryMenu.gameObject;
                    this.menuToUseNext.SetActive(true);
                    inventoryMenu.OpenInventory(Inventory, this.equippedWeapon, this.equippedSpecialItem);
                    StopMovement();
                    this.state = KnightState.ININVENTORY;
                }
                if (this.InteractFlagSet || onPointerClickFlag)
                {
                    this.InteractFlagSet = false;
                    this.onPointerClickFlag = false;

                    if (this.InteractableInRange == null)
                    {
                        return;
                    }

                    StartInteraction();
                }
                break;
            case KnightState.INDIALOGUE: // TODO TEST this state I think i fucked it
                MyGuard.IsNotNull(dialogueBox);
                if (this.InteractFlagSet || onPointerClickFlag)
                {
                    this.InteractFlagSet = false;
                    onPointerClickFlag = false;
                    dialogueBox.PlayerInteractFlagSet = true;
                }
                else if (this.dialogueBox.finishedInteractionFlag)
                {
                    this.dialogueBox.finishedInteractionFlag = false;
                    EndDialogue();
                }
                break;
            case KnightState.InItemContainer:
                MyGuard.IsNotNull(this.ContainerMenu);
                if (escKeyFlag)
                {
                    escKeyFlag = false;
                    this.ContainerMenu.Close();

                    if (this.InteractableInRange is ItemContainer chest && chest != null)
                    {
                        chest.PlayClosedSound();
                    }

                    this.state = KnightState.PLAYERCANMOVE;
                    MyGuard.IsNotNull(this.pauseMenu);
                    this.menuToUseNext = this.pauseMenu.gameObject;
                }
                else if (this.onPointerClickFlag)
                {
                    this.onPointerClickFlag = false;
                    var itemOpButton = this.ContainerMenu.GetSelectedButton().GetComponentInParent<InventorySlot>();

                    if (itemOpButton == null || itemOpButton.Item == null)
                    {
                        return;
                    }

                    itemOpButton.PlaySelectSound();

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
                    this.menuToUseNext = pauseMenu.gameObject;
                    this.state = KnightState.PLAYERCANMOVE;
                }
                if (this.onPointerClickFlag)
                {
                    this.onPointerClickFlag = false;
                    MyGuard.IsNotNull(this.inventoryMenu);
                    var buttonGameObject = this.inventoryMenu.GetSelectedButton();

                    if (buttonGameObject == null)
                    {
                        Debug.Log("clicked on nothing");
                        return;
                    }

                    if (buttonGameObject.TryGetComponent<BookTab>(out var selectedTab))
                    {
                        this.inventoryMenu.OnTabClick(selectedTab);
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
                        RemoveFromPlayerEquipped(selectedItemOp);
                        this.inventoryMenu.RemoveFromPlayerEquipped(selectedItemOp);
                    }
                    else if (IsValidEquip(selectedItemOp.Item))
                    {
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
                // controlled by BattleUI and never exits. We change states when the next scene after the battle is loaded and the player in that scene will be used with its starting state.
                break;
            default:
                this.state = KnightState.PLAYERCANMOVE;
                break;
        }

        ResetFlags();
    }

    /// <summary>
    /// Idk if comparing name string is neccessary though I
    /// </summary>
    /// <param name="itemToRemove"></param>
    private void RemoveFromPlayerEquipped(InventorySlot itemButton)
    {
        var itemToRemove = itemButton.Item;
        MyGuard.IsNotNull(itemToRemove);
        MyGuard.IsNotNull(inventoryMenu);

        if (itemToRemove.name == equippedWeapon.name)
        {
            equippedWeapon = defaultWeaponHands;
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

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }

        Log.Print("ClickFlagSet flag set");
        this.onPointerClickFlag = true;
    }
}
