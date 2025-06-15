using Assets.GracesScripts;
using Assets.GracesScripts.Data;
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
public class PlayerDungeon : DungeonUnit
{
    [SerializeField] private float movementSpeed = 1f;
    private Rigidbody2D? rb;
    private Vector2 direction;
    [SerializeField] private KnightState startingState = KnightState.PLAYERCANMOVE;
    [HideInInspector] private KnightState state;
    private AudioSource? footstepsSound;

    [Header("Menus")]
    [SerializeField] GameObject deathScreenPrefab;
    private PauseMenu? pauseMenu;
    private Menu? menuToUseNext;
    private InventoryMenu? inventoryMenu;

    [Header("Death")]
    [SerializeField] AudioSource LevelMusic;
    [SerializeField] AudioSource DeathSFX;

    public List<string> scenesTraversed = new();

    [HideInInspector] public IInteracble? InteractableInRange;

    [Header("EnemyBattleLoader")]
    [SerializeField] private GameObject enemyLoaderPrefab;
    [HideInInspector] public DoStuffOnSceneLoad sceneLoadEvents;

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
        INTERACTING,
        PLAYERCANMOVE,
        INPAUSEMENU,
        ININVENTORY,
        INTURNBASED,
    }

    private PlayerDungeonData LastSavedData;

    public override object CaptureState()
    {
        var newPlayerData = new PlayerDungeonData()
        {
            CurrentHealth = this.currentHealth,
            MaxHealth = this.maxHealth,
            EquippedWeapon = this.equippedWeapon,
            EquippedItem = this.equippedSpecialItem,
            Inventory = this.Inventory,
            Position = this.transform.position,
            LastSavedData = this.LastSavedData,
        };

        // If saving in battle scene to transition out of we want to NOT save position.
        // becuase last saved position from the scene before battle needs to be loaded instead.
        if (SceneManager.GetActiveScene().name != TalkingDungeonScenes.Battle)
        {
            // dont save position in battle just resave the last saved pos from previous not battle scene
            // Or if last saved data is empty (first time saving) then save current position.
            newPlayerData.Position = LastSavedData == null ? this.transform.position : LastSavedData.Position;
        }

        this.LastSavedData = newPlayerData;
        return newPlayerData;
    }

    private void RestoreState(string state, bool restorePosition)
    {
        Debug.Log("Restoring state from JSON: " + state);
        var data = JsonUtility.FromJson<PlayerDungeonData>(state);

        this.currentHealth = data.CurrentHealth;
        this.maxHealth = data.MaxHealth;
        this.equippedWeapon = data.EquippedWeapon;
        this.equippedSpecialItem = data.EquippedItem;
        this.Inventory = data.Inventory;
        this.LastSavedData = data.LastSavedData;

        if (restorePosition)
        {
            this.transform.position = data.Position;
        }
    }

    /// <summary>
    /// Restores Players State not position. When entering battle scene we dont want to.
    /// </summary>
    /// <param name="state"></param>
    public override void RestoreState(string state)
    {
        // if we are restoring in a scene the same as last saved in then we are loading save and should load position
        if (SceneManager.GetActiveScene().name == PlayerPrefs.GetString(SaveKeys.LastScene))
        {
            this.RestoreState(state, true);
        }
        // if we are restoring in a battle scene dont load position.
        else if (SceneManager.GetActiveScene().name != TalkingDungeonScenes.Battle)
        {
            this.RestoreState(state, false);
        }
        // if we are restoring in a scene different from last dont load position its a new scene.
        else if (SceneManager.GetActiveScene().name != PlayerPrefs.GetString(SaveKeys.LastScene))
        {
            this.RestoreState(state, false);
        }
        // if restoring in a scene after a battle scene load position.
        else if (PlayerPrefs.GetString(SaveKeys.LastScene) == TalkingDungeonScenes.Battle)
        {
            this.RestoreState(state, false);
        }
        else
        {
            this.RestoreState(state, true);
        }
    }

    private void Start()
    {
        var enemyLoaders = FindObjectsByType<DoStuffOnSceneLoad>(FindObjectsSortMode.None);
        if (enemyLoaders.Length == 0)
        {
            sceneLoadEvents = Instantiate(enemyLoaderPrefab).GetComponent<DoStuffOnSceneLoad>();
        }
        else
        {
            this.sceneLoadEvents = enemyLoaders[0];
        }

        this.state = startingState;

        // This should be happening after DostuffOnSceneLoad.OnSceneLoaded
        this.LoadPlayerComponents();

        if (Abilities.Count < 1)
        {
            throw new ArgumentException("canont have less than 1 ability at least would have push on hands.");
        }

        if (this.startingState == KnightState.INTURNBASED)
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
        MyGuard.IsNotNull(this.rb, "Rigidbody2D null in PlayerDungeon.LoadPlayerComponents()");
        footstepsSound = GetComponentInChildren<AudioSource>();
        var menuReferences = FindObjectOfType<MenuReferences>();
        this.pauseMenu = menuReferences.PauseMenu;
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
        MyGuard.IsNotNull(this.InteractableInRange, "Interactable null in Start Interaction!");
        this.InteractableInRange.Interact();
        this.StopMovement();
        this.state = KnightState.INTERACTING;
    }

    private void StopMovement()
    {
        // Stop animations
        MyGuard.IsNotNull(animatedLayers);
        this.animatedLayers.SetBools("Moving", false);

        // stop movement
        MyGuard.IsNotNull(footstepsSound);
        footstepsSound.Stop();
        this.rb!.velocity = Vector2.zero;
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

    private void HandleState()
    {
        switch (this.state)
        {
            case KnightState.PLAYERCANMOVE:
                this.rb!.velocity = direction * movementSpeed;
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
            case KnightState.INTERACTING:
                if (this.InteractableInRange!.FinishedInteraction)
                {
                    this.state = KnightState.PLAYERCANMOVE;
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
                    
                }
                break;
            case KnightState.INTURNBASED:
                // controlled by BattleUI and never exits.
                // We change states next when battle is won and the new scene is loaded.
                break;
            default:
                this.state = KnightState.PLAYERCANMOVE;
                break;
        }
    }

    public void RemoveFromPlayerEquipped(InventorySlot itemButton)
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
    public void AddToPlayerEquipped(DungeonItem itemToEquip)
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
            this.animatedLayers.SetFloats("LastXDir", this.direction.x);
            this.animatedLayers.SetFloats("LastYDir", this.direction.y);
        }
        else if (context.performed)
        {

        }
        else if (context.canceled)
        {
            footstepsSound.Stop();
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
