using Assets.GracesScripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDungeon : MonoBehaviour
{
    [SerializeField] DialogueTextBox dialogueBox;
    private IInteracble interactableInRange = null;
    // private Animator animator;
    private AnimatedLayers animatedLayers;
    private Rigidbody2D rb;
    private Vector2 direction;
    private Vector2 lastMovingDirection;
    [SerializeField] private float movementSpeed = 1f;
    private KnightState state = KnightState.PLAYERCANMOVE;

    /// <summary>
    /// Flag Set to true ONLY WHEN there is an interactable in range. <see cref="OnInteract(InputAction.CallbackContext)"/>
    /// </summary>
    private bool InteractFlagSet;

    private enum KnightState
    {
        INTERACTING,
        PLAYERCANMOVE,
    }

    private void Awake()
    {
        this.animatedLayers = GetComponent<AnimatedLayers>();
        this.rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        this.state = KnightState.PLAYERCANMOVE;
    }

    private void StartInteraction()
    {
        this.InteractFlagSet = false;
        if (this.interactableInRange is IHasDialogue interactableWithDialogue)
        {
            // if the object you start talking to is moving it can move out of range and causes on trigger exit player wont be able to spacebar out of dialogue.
            // stop moving on start interaction and finish on end interaction
            if (this.interactableInRange is WalkingBackAndForthNPC movingNPC)
            {
                movingNPC.IsStationary = true;
            }

            this.dialogueBox.gameObject.SetActive(true);
            dialogueBox.PlayerInteractFlagSet = true;
            this.dialogueBox.NewInteractionBegan(interactableWithDialogue.GetFirstDialogueSlide());

            // TODO: not sure if I maybe should disable move all the time on interaction
            this.GetComponent<PlayerInput>().actions.FindAction("Move").Disable();
        }

        // TODO: add more interactable features if I need e.g.
        // if IInteractble could be a moving lever or something. that does not have Dialogue.
        // e.g.
        // if (this.interactableInRange is IHasLever)
        // { than the lever is an IINteractable object and will do lever stuff. and then set the this.state
    }


    private void FixedUpdate()
    {
        switch (this.state)
        {
            case KnightState.PLAYERCANMOVE:
                // regular movement logic stuff
                this.rb.velocity = direction * movementSpeed;

                if (this.InteractFlagSet)
                {
                    StartInteraction();
                    this.state = KnightState.INTERACTING; 
                }
                break;
            case KnightState.INTERACTING:
                this.rb.velocity = Vector2.zero;
                if (this.InteractFlagSet)
                {
                    dialogueBox.PlayerInteractFlagSet = true;
                    this.InteractFlagSet = false;
                }
                else if ((this.dialogueBox.State == DialogueTextBox.BoxState.WAITINGFORINTERACTION))
                {
                    this.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
                    this.state = KnightState.PLAYERCANMOVE;

                    // TODO: What does this comment and the next two lines of code mean? i dont remember
                    // something to do with if its a moving npc like the DOG, if you move it out of range.
                    // if the object you start talking to also moves our and causes on trigger exit player wont be able to spacebar out of dialogue.
                    // this is to allow it to go back to moving state again.
                    if (this.interactableInRange is WalkingBackAndForthNPC movingNPC)
                    {
                        movingNPC.IsStationary = false;
                    }
                }
                break;
            default:
                this.state = KnightState.PLAYERCANMOVE;
                //Log.Print("Freemovement from default");
                break;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        this.direction = context.ReadValue<Vector2>();

        // TODO for an Running as well. need to add an IsRunning boolean triggered in the OnRun function. do I want the player to have to be running first? then press sprint?

        if (context.started)
        {
            Log.Print("on move started");
            Log.Print($"dir: {this.direction.x}, {this.direction.y}");
            this.animatedLayers.SetFloats("LastX", this.direction.x);
            this.animatedLayers.SetFloats("LastY", this.direction.y);
        }
        else if (context.performed)
        {

        }
        else if (context.canceled)
        {
            Log.Print("on move cancelled");
        }

        this.animatedLayers.SetFloats("YVel", this.direction.y);
        this.animatedLayers.SetFloats("XVel", this.direction.x);

        // true when started and in performed state, false on cancel (finish or relase key)
        Log.Print($"context: {!context.canceled}");
        this.animatedLayers.SetBools("Moving", !context.canceled);
    }

    public void OnRunKey(InputAction.CallbackContext context)
    {
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

    private void FlipSprite(float xDirection)
    {
        // if going left and facing right flip
        if (xDirection < 0 && this.transform.localScale.x > 0)
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x * -1, this.transform.localScale.y, this.transform.localScale.z);
        }
        else if (xDirection > 0 && this.transform.localScale.x < 0)
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x * -1, this.transform.localScale.y, this.transform.localScale.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IInteracble>(out var NPCWithDialogue))
        {
            Log.Print("can interact with" + collision.name);
            this.interactableInRange = NPCWithDialogue;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IInteracble>(out var NPCWithDialogue) && (NPCWithDialogue == this.interactableInRange))
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
