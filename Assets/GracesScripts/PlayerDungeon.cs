using Assets.GracesScripts;
using Cinemachine;
using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDungeon : MonoBehaviour
{
    [SerializeField] DialogueTextBox dialogueBox;
    private IInteracble interactableInRange = null;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 direction;
    [SerializeField] private float movementSpeed = 1f;
    private KnightState state = KnightState.PLAYERCANMOVE;
    private bool InteractFlagSet;

    private enum KnightState
    {
        INTERACTING,
        PLAYERCANMOVE,
    }

    private void Awake()
    {
        this.animator = this.GetComponent<Animator>();
        this.rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        this.state = KnightState.PLAYERCANMOVE;
    }

    private void FixedUpdate()
    {
        switch (this.state)
        {
            case KnightState.PLAYERCANMOVE:
                if (this.InteractFlagSet)
                {
                    this.InteractFlagSet = false;
                    if (this.interactableInRange is IHasDialogue interactableWithDialogue)
                    {
                        // if the object you start talking to also moves our and causes on trigger exit player wont be able to spacebar out of dialogue.
                        try
                        {
                            var movingNPC = (WalkingBackAndForthNPC)this.interactableInRange;
                            movingNPC.IsInDialogue = true;
                        }
                        catch (Exception ex)
                        {
                            // not talking to a moving interacable
                        }

                        this.dialogueBox.gameObject.SetActive(true);
                        dialogueBox.PlayerInteractFlagSet = true;
                        this.dialogueBox.NewInteractionBegan(interactableWithDialogue.GetFirstDialogueSlide());
                    }

                    // TODO: add more interactable features if I need e.g. if IInteractble could be a moving lever or something. that does not have Dialogue.
                }
                else if(this.dialogueBox.State == DialogueTextBox.BoxState.WRITINGSLIDE || this.dialogueBox.State == DialogueTextBox.BoxState.WAITINGONSLIDE)
                {
                    this.state = KnightState.INTERACTING;
                }
                else
                {
                    // regular movement logic stuff
                    this.rb.velocity = direction * movementSpeed;

                    
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
                    this.state = KnightState.PLAYERCANMOVE;
                   
                    // if the object you start talking to also moves our and causes on trigger exit player wont be able to spacebar out of dialogue.
                    try
                    {
                        var movingNPC = (WalkingBackAndForthNPC)this.interactableInRange;
                        movingNPC.IsInDialogue = false;
                    }
                    catch (Exception ex)
                    {
                        // not talking to a moving interacable
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
        this.animator.SetFloat("YVel", this.direction.y);
        this.animator.SetFloat("XVel", this.direction.x);

        if (context.started)
        {
            Log.Print("on move started");            
        }
        else if (context.performed)
        {

        }
        else if (context.canceled)
        {
            Log.Print("on move cancelled");
        }

        // true when started and in performed state, false on cancel (finish or relase key)
        Log.Print($"context: {!context.canceled}");
        this.animator.SetBool("Moving", !context.canceled);
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
        this.animator.SetBool("Running", !context.canceled);
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
