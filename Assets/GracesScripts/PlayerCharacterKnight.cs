using Assets.GracesScripts;
using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterKnight : MonoBehaviour
{
    [SerializeField] DialogueTextBox dialogueBox;
    private IInteracble interactableInRange = null;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 direction;
    [SerializeField] private float movementSpeed = 1f;
    private KnightState state = KnightState.PLAYERCONTROL;
    private bool InteractFlagSet;

    private enum KnightState
    {
        INTERACTING,
        PLAYERCONTROL,
    }

    private void Awake()
    {
        this.animator = this.GetComponent<Animator>();
        this.rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        this.state = KnightState.PLAYERCONTROL;
    }

    private void FixedUpdate()
    {
        switch (this.state)
        {
            case KnightState.PLAYERCONTROL:
                if (this.InteractFlagSet)
                {
                    this.InteractFlagSet = false;
                    if (this.interactableInRange is IHasDialogue interactableWithDialogue)
                    {
                        this.dialogueBox.gameObject.SetActive(true);
                        dialogueBox.PlayerInteractFlagSet = true;
                        this.dialogueBox.NewInteractionBegan(interactableWithDialogue.GetFirstDialogueSlide());
                    }

                    // TODO: add more interactable features if I need
                }
                else if(this.dialogueBox.State == DialogueTextBox.BoxState.WRITINGSLIDE || this.dialogueBox.State == DialogueTextBox.BoxState.WAITINGONSLIDE)
                {
                    this.state = KnightState.INTERACTING;
                }
                else
                {
                    this.rb.velocity = direction * movementSpeed;
                    FlipSprite(this.direction.x);
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
                    this.state = KnightState.PLAYERCONTROL;
                }
                break;
            default:
                this.state = KnightState.PLAYERCONTROL;
                //Log.Print("Freemovement from default");
                break;
        }

        bool isMoving = this.rb.velocity.x != 0 || this.rb.velocity.y != 0;
        this.animator.SetBool("Running", isMoving);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        this.direction = context.ReadValue<Vector2>();

        if (context.started)
        {
        }
        else if (context.performed)
        {

        }
        else if (context.canceled)
        {
        }
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
        if (collision.TryGetComponent<IHasDialogue>(out var dialogueObject))
        {
            Log.Print("can interact with" + collision.name);
            this.interactableInRange = dialogueObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IHasDialogue>(out var dialogueObject) && (dialogueObject == this.interactableInRange))
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
