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
        inItemContainer,
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
            this.state = KnightState.inItemContainer;
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
            case KnightState.inItemContainer:
                if (this.InteractFlagSet)
                {
                    this.InteractFlagSet = false;
                    var button = this.ContainerMenu.GetSelectedButton();

                    var itemOption = button.GetComponent<ItemOptionButton>();
                    var item = itemOption.Item;

                    if (item == null)
                    {
                        return;
                    }

                    Debug.Log($"selected {item.Name}");
                }
                break;
            default:
                this.state = KnightState.PLAYERCANMOVE;
                break;
        }
    }

    public GameObject currentMenuOpen;

    public void OnMenuCancel(InputAction.CallbackContext context)
    {
        if(!context.started)
        {
            return;
        }

        if(currentMenuOpen.TryGetComponent<PauseMenu>(out var menu))
        {
            this.pauseMenu.gameObject.SetActive(!this.pauseMenu.gameObject.activeSelf);
        }
        else if (currentMenuOpen.TryGetComponent<ContainerMenu>(out var containerMenu))
        {
            this.ContainerMenu.gameObject.SetActive(false);
            this.ContainerMenu.ClearItems();

            if(this.interactableInRange is ItemContainer chestOrSomething)
            {
                chestOrSomething.GetComponent<Animator>().SetTrigger("Closed");
            }

            this.state = KnightState.PLAYERCANMOVE;
        }

        // TODO other menus

        // default is pause menu
        this.currentMenuOpen = this.pauseMenu.gameObject;
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
