using Assets.GracesScripts;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Player guy
/// </summary>
[RequireComponent(typeof(UseAnimatedLayers))]
public class PlayerDungeon : MonoBehaviour
{
    [SerializeField] DialogueTextBox dialogueBox;
    [SerializeField] ContainerMenu ContainerMenu;
    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] InventoryMenu inventoryMenu;
    public GameObject currentMenuOpen;

    private IInteracble interactableInRange = null;
    private Rigidbody2D rb;
    private Vector2 direction;
    private Vector2 lastMovingDirection;
    [SerializeField] private float movementSpeed = 1f;
    private KnightState state = KnightState.PLAYERCANMOVE;
    private AudioSource footstepsSound;
    private UseAnimatedLayers animatedLayers;


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
    }

    private void Awake()
    {
        animatedLayers = GetComponent<UseAnimatedLayers>();
        this.rb = GetComponent<Rigidbody2D>();
        footstepsSound = GetComponentInChildren<AudioSource>();
        this.currentMenuOpen = this.pauseMenu.gameObject;
    }

    // Start is called before the first frame update
    private void Start()
    {
        this.state = KnightState.PLAYERCANMOVE;
    }

    private void StartInteraction()
    {
        this.InteractFlagSet = false;
        if (this.interactableInRange is IHasDialogue interactableWithDialogue && interactableWithDialogue != null)
        {
            // if the object you start talking to is moving it can move out of range and causes on trigger exit player wont be able to spacebar out of dialogue.
            // stop moving on start interaction and finish on end interaction
            if (this.interactableInRange is WalkingBackAndForth movingNPC)
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
            this.ContainerMenu.CreateLootButtons(chest.loot);
            this.state = KnightState.InItemContainer;
        }

        // TODO: add more interactables here

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
                if (iKeyFlag)
                {
                    iKeyFlag = false;
                    this.currentMenuOpen = inventoryMenu.gameObject;
                    this.currentMenuOpen.SetActive(true);
                    this.state = KnightState.ININVENTORY;
                }
                this.rb.velocity = direction * movementSpeed;
                if (this.InteractFlagSet)
                {
                    StartInteraction();
                }
                break;
            case KnightState.INDIALOGUE:
                if (this.InteractFlagSet)
                {
                    dialogueBox.PlayerInteractFlagSet = true;
                    this.InteractFlagSet = false;
                }
                else if (this.dialogueBox.State == DialogueTextBox.BoxState.WAITINGFORINTERACTION)
                {
                    EndMovingDialogue();
                }
                break;
            case KnightState.InItemContainer:
                if (this.InteractFlagSet)
                {
                    this.InteractFlagSet = false;
                    var item = this.ContainerMenu.OnButtonSelected();

                    if (item == null)
                    {
                        return;
                    }

                    Debug.Log($"selected {item.Name}");
                }
                break;
            case KnightState.INPAUSEMENU:
                if (this.InteractFlagSet)
                {
                    this.InteractFlagSet = false;
                    var button = this.pauseMenu.GetSelectedButton();

                    var option = button.GetComponent<ButtonMenuOption>();

                    Debug.Log($"selected {option}");
                }
                if (!this.pauseMenu.isActiveAndEnabled)
                {
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
                    this.currentMenuOpen.SetActive(false);
                    this.currentMenuOpen = pauseMenu.gameObject;
                    this.state = KnightState.PLAYERCANMOVE;
                }
                if (this.InteractFlagSet)
                {
                    this.InteractFlagSet = false;
                    var button = this.pauseMenu.GetSelectedButton();

                    var option = button.GetComponent<ButtonMenuOption>();

                    Debug.Log($"selected {option}");
                }
                break;
            default:
                this.state = KnightState.PLAYERCANMOVE;
                break;
        }
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

        if (currentMenuOpen.TryGetComponent<PauseMenu>(out var menu))
        {
            escKeyFlag = false;
            this.pauseMenu.gameObject.SetActive(!this.pauseMenu.gameObject.activeSelf);
            this.pauseMenu.StartPauseMenu();
            this.state = KnightState.INPAUSEMENU;
            this.currentMenuOpen = this.pauseMenu.gameObject;
        }
        else if (currentMenuOpen.TryGetComponent<ContainerMenu>(out var containerMenu))
        {
            escKeyFlag = false;
            this.ContainerMenu.gameObject.SetActive(false);
            this.ContainerMenu.ClearItems();

            if(this.interactableInRange is ItemContainer chestOrSomething)
            {
                chestOrSomething.GetComponent<Animator>().SetTrigger("Closed");
            }

            this.state = KnightState.PLAYERCANMOVE;
            this.currentMenuOpen.GetComponent<ContainerMenu>().Close();
            this.currentMenuOpen = this.pauseMenu.gameObject;
        }
        // TODO other menus

        // default is pause menu
    }

    private void EndMovingDialogue()
    {
        // if the object you start talking to also moves our and causes on trigger exit player wont be able to spacebar out of dialogue.
        // this is to allow it to go back to moving state again.
        if (this.interactableInRange is WalkingBackAndForth movingNPC)
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

        if (interactableInRange == null)
        {
            return;
        }

        Log.Print("Interact flag set");
        this.InteractFlagSet = true;
    }
}
