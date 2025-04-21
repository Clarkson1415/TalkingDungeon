using Assets.GracesScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PlayerDataUtility;
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
    private DialogueTextBox? dialogueBox;
    private ContainerMenu? ContainerMenu;
    private PauseMenu? pauseMenu;
    private GameObject? currentMenuOpen;

    [Header("Death")]
    [SerializeField] AudioSource LevelMusic;
    [SerializeField] AudioSource DeathSFX;


    [Header("Inventory")]
    private InventoryMenu? inventoryMenu;
    [SerializeField] AudioClip InventoryOpenSound;
    [SerializeField] AudioClip InventoryClosedSound;
    public List<Item?> Inventory = new();
    [SerializeField] private AudioSource audioSourceForInventorySounds;
    private List<Item?> EquippedItems => new() { this.equippedClothing, this.equippedWeapon, this.equippedSpecialItem };
    public Item? equippedWeapon;
    public Item? equippedClothing;
    public Item? equippedSpecialItem;

    [Header("Stats")]
    public float maxWellbeing = 100;
    public float currentWellbeing = 100;
    public Image healthBarFillImage;
    public List<Ability> abilities = new();
    public float Power => this.EquippedItems.Sum(x => x != null ? x.PowerStat : 0);
    public float Defence => this.EquippedItems.Sum(x => x != null ? x.DefenceStat : 0);

# if UNITY_EDITOR
    [Header("For While Testing In Unity Editor")]
    public List<string> scenesTraversed = new();
# endif

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 1f;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    // reason: RB is located in setup.
    private Rigidbody2D rb;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private Vector2 direction;
    [SerializeField] private KnightState startingState = KnightState.PLAYERCANMOVE;
    [HideInInspector] private KnightState state;
    private AudioSource? footstepsSound;
    private UseAnimatedLayers? animatedLayers;

    [Header("Interactions")]
    
    /// <summary>
    /// Flag Set to true ONLY WHEN there is an interactable in range. <see cref="OnInteract(InputAction.CallbackContext)"/>
    /// </summary>
    private bool InteractFlagSet;

    [HideInInspector] public bool isHealthBarDoingAnim;
    [HideInInspector] public IInteracble? InteractableInRange { get; private set; } = null;

    [Header("EnemyBattleLoader")]
    [SerializeField] private GameObject enemyLoaderPrefab;
    [HideInInspector] public EnemyLoader enemyLoader;

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
        
        SetupPlayer();

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(SaveKeys.LastScene))) // if loading from save load otherwise its not got player prefs for a new game
        {
            PlayerDataUtility.LoadSaveDataFromLastScene(this);
        }

        // do not save if in battle scene though
        if (SceneManager.GetActiveScene().name != TalkingDungeonScenes.Battle)
        {
            PlayerDataUtility.SaveGame(this);
        }
    }

    /// <summary>
    /// Called after all the save data has been loaded.
    /// </summary>
    public void SetupPlayer()
    {
        StartCoroutine(WaitForSceneLoadedThenSetup());
    }

    IEnumerator WaitForSceneLoadedThenSetup()
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

        this.InitializeMenus();
        this.LoadPlayer();
    }

    /// <summary>
    /// If the scene was loaded from a save when update runs rb will still be null beacuse setupPlayer hasnt been called in awake. so wait for this to be true to do update stuff. As setupPlayer will be aclled y menubutton after scene has loaded.
    /// </summary>
    /// <returns></returns>
    private bool ShouldUpdateRun()
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

    private void InitializeMenus()
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
    private void LoadPlayer()
    {
        animatedLayers = GetComponent<UseAnimatedLayers>();
        this.rb = GetComponent<Rigidbody2D>();
        footstepsSound = GetComponentInChildren<AudioSource>();
        MyGuard.IsNotNull(this.pauseMenu);
        this.currentMenuOpen = this.pauseMenu.gameObject;
        this.healthBarFillImage = FindFirstObjectByType<HealthBarFill>().GetComponent<Image>();

        this.healthBarFillImage.fillAmount = this.currentWellbeing / this.maxWellbeing;

        if (abilities.Count < 1)
        {
            Debug.LogError("cannot have less than 1 ability at least have basic push");
        }

        foreach (var item in this.EquippedItems)
        {
            if (item == null)
            {
                continue;
            }

            AddToPlayerEquipped(item);
        }
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
            currentMenuOpen = this.dialogueBox.gameObject;
            this.dialogueBox.gameObject.SetActive(true);
            dialogueBox.PlayerInteractFlagSet = true;
            this.dialogueBox.BeginDialogue(interactableWithDialogue.GetFirstDialogueSlide());
            this.state = KnightState.INDIALOGUE;
        }
        else if (this.InteractableInRange is ItemContainer chest && chest != null)
        {
            MyGuard.IsNotNull(ContainerMenu);
            currentMenuOpen = this.ContainerMenu.gameObject;
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
        this.animatedLayers.SetFloats("YVel", 0);
        this.animatedLayers.SetFloats("XVel", 0);
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
        if (!ShouldUpdateRun())
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
                    this.currentMenuOpen = this.pauseMenu.gameObject;
                }
                if (iKeyFlag)
                {
                    MyGuard.IsNotNull(this.inventoryMenu);
                    iKeyFlag = false;
                    this.currentMenuOpen = inventoryMenu.gameObject;
                    this.currentMenuOpen.SetActive(true);

                    inventoryMenu.OpenInventory(Inventory);

                    audioSourceForInventorySounds.clip = this.InventoryOpenSound;
                    audioSourceForInventorySounds.Play();
                    
                    this.inventoryMenu.UpdatePlayerStatsDisplay(this.Power, this.Defence);
                    this.inventoryMenu.UpdatePlayerWellBeingDislpay((int)this.currentWellbeing);

                    StopMovement();
                    this.state = KnightState.ININVENTORY;
                }
                if (this.InteractFlagSet)
                {
                    this.InteractFlagSet = false;

                    if (this.InteractableInRange == null)
                    {
                        return;
                    }

                    StartInteraction();
                }
                break;
            case KnightState.INDIALOGUE: // TODO TEST this state I think i fucked it
                MyGuard.IsNotNull(dialogueBox);
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

                    if (this.InteractableInRange is ItemContainer chest && chest != null)
                    {
                        chest.Loot.Remove(itemOpButton.Item);
                        this.Inventory.Add(itemOpButton.Item);
                    }

                    this.ContainerMenu.RemoveOldItem(itemOpButton);
                }
                break;
            case KnightState.INPAUSEMENU:
                if (this.InteractFlagSet)
                {
                    MyGuard.IsNotNull(this.pauseMenu);

                    this.InteractFlagSet = false;
                    var button = this.pauseMenu.GetSelectedButton();

                    var option = button.GetComponent<MenuButton>();

                    Debug.Log($"selected {option}");
                }
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
                    MyGuard.IsNotNull(currentMenuOpen);
                    MyGuard.IsNotNull(pauseMenu);
                    this.currentMenuOpen.GetComponent<InventoryMenu>().Close();
                    audioSourceForInventorySounds.clip = this.InventoryClosedSound;
                    audioSourceForInventorySounds.Play();
                    this.currentMenuOpen = pauseMenu.gameObject;
                    this.state = KnightState.PLAYERCANMOVE;
                }
                if (this.InteractFlagSet)
                {
                    this.InteractFlagSet = false;
                    MyGuard.IsNotNull(this.inventoryMenu);
                    var buttonGameObject = this.inventoryMenu.GetSelectedButton();
                    var selectedItemOp = buttonGameObject.GetComponent<ItemOptionButton>();

                    if (selectedItemOp.Item == null)
                    {
                        return;
                        // nothing happens if empty square selected.
                    }
                    else if (this.EquippedItems.Contains(selectedItemOp.Item))
                    {
                        RemoveFromPlayerEquipped(selectedItemOp);
                    }
                    else if (IsValidEquip(selectedItemOp.Item))
                    {
                        AddToPlayerEquipped(selectedItemOp.Item);
                    }
                    else
                    {
                        Log.Print("can't equip that");
                        return;
                    }
                }
                break;
            case KnightState.InTurnBased:
                // controlled by BattleUI and never exits. We change states when the next scene after the battle is loaded and the player in that scene will be used with its starting state.
                if (escKeyFlag)
                {
                    escKeyFlag = false;
                    var BattleUI = FindObjectOfType<BattleUI>();
                    MyGuard.IsNotNull(BattleUI);
                    BattleUI.PlayerEscKeyFlag = true;
                }
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
    private void RemoveFromPlayerEquipped(ItemOptionButton itemButton)
    {
        var itemToRemove = itemButton.Item;
        MyGuard.IsNotNull(itemToRemove);
        MyGuard.IsNotNull(inventoryMenu);
        this.inventoryMenu.RemoveEquippedItem(itemToRemove);

        if (equippedWeapon != null && (itemToRemove.name == equippedWeapon.name))
        {
            equippedWeapon = null;
        }
        else if (equippedClothing != null && (itemToRemove.name == equippedClothing.name))
        {
            equippedClothing = null;
        }
        else if (equippedSpecialItem != null && (itemToRemove.name == equippedSpecialItem.name))
        {
            equippedSpecialItem = null;
        }

        this.inventoryMenu.UpdatePlayerStatsDisplay(this.Power, this.Defence);
        this.inventoryMenu.UpdatePlayerWellBeingDislpay((int)this.currentWellbeing);
    }

    /// <summary>
    /// TODO a better way for using this equipped items and inventory have a looksie
    /// </summary>
    /// <param name="itemToEquip"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void AddToPlayerEquipped(Item itemToEquip)
    {
        MyGuard.IsNotNull(this.inventoryMenu);

        this.inventoryMenu.AddOrReplaceEquipped(itemToEquip);

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

        this.inventoryMenu.UpdatePlayerStatsDisplay(this.Power, this.Defence);
        this.inventoryMenu.UpdatePlayerWellBeingDislpay((int)this.currentWellbeing);
    }

    /// <summary>
    /// TODO, was thinkging of having player stat /level blocks for items but now probs not
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private bool IsValidEquip(Item item)
    {
        MyGuard.IsNotNull(item);
        bool isValid = true;
        return isValid;
    }

    public void TakeDamage(float damage)
    {
        this.currentWellbeing -= damage;
        // if current damage will kill player make it go fast
        if (this.currentWellbeing <= 0)
        {
            StartCoroutine(AnimateHealthLoss(0.1f, damage));
            var canvas = FindObjectOfType<Canvas>();
            Instantiate(this.deathScreenPrefab, canvas.transform);

            // play death sound
            this.DeathSFX.Play();
            this.LevelMusic.Stop();
        }
        else
        {
            StartCoroutine(AnimateHealthLoss(0.5f, damage));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="speedToFinish">seconds time for health bar to go to where after damage puts it</param>
    /// <returns></returns>
    private IEnumerator AnimateHealthLoss(float speedToFinish, float damage)
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
        if (this.InteractableInRange is WalkingBackAndForthUnit movingNPC)
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
        this.InteractFlagSet = true;
    }
}
