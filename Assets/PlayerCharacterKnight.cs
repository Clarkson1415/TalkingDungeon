using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
#nullable enable

public class PlayerCharacterKnight : MonoBehaviour
{
    [SerializeField] DialogueTextBox dialogueBox;
    private bool isDialogueSlidePrinting;
    private IInteracble? interactableInRange = null;
    private bool inDialogue;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 direction;

    [SerializeField] private float movementSpeed = 1f;
    private knightState state = knightState.FREEMOVEMENT;
    private bool InteractFlagSet;

    private enum knightState
    {
        INTERACTING,
        FREEMOVEMENT,
    }

    void Awake()
    {
        this.animator = this.GetComponent<Animator>();
        this.rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.state = knightState.FREEMOVEMENT;
    }

    private void FixedUpdate()
    {
        switch (this.state)
        {
            case knightState.FREEMOVEMENT:
                if (this.InteractFlagSet)
                {
                    Debug.Log("interact flag set in FREEMOVEMENT");
                    this.dialogueBox.gameObject.SetActive(true);
                    dialogueBox.PlayerInteractFlagSet = true;
                    this.InteractFlagSet = false;
                    if(this.interactableInRange is IHasDialogue interactableWithDialogue)
                    {
                        Debug.Log("Dialogue Box");
                        this.dialogueBox.NewInteractionBegan(interactableWithDialogue.GetFirstDialogueSlide());
                    }
                    this.state = knightState.INTERACTING;
                }
                this.rb.velocity = direction * movementSpeed;
                FlipSprite(this.direction.x);
                break;
            case knightState.INTERACTING:
                if (this.InteractFlagSet)
                {
                    dialogueBox.PlayerInteractFlagSet = true;
                    this.InteractFlagSet = false;
                }
                if (this.dialogueBox.State == DialogueTextBox.BoxState.invisibleInactive)
                {
                    Debug.Log("Freemovement from interacting");
                    this.state = knightState.FREEMOVEMENT;
                }
                break;
            default:
                this.state = knightState.FREEMOVEMENT;
                Debug.Log("Freemovement from default");
                break;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("On move fired");
        this.direction = context.ReadValue<Vector2>();

        if (context.started)
        {
            this.animator.SetBool("Running", true);
        }
        else if (context.performed)
        {

        }
        else if (context.canceled)
        {
            this.animator.SetBool("Running", false);
        }
    }

    void FlipSprite(float xDirection)
    {
        // if going left and facing right flip
        if (xDirection < 0 && this.transform.localScale.x > 0)
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x * -1, this.transform.localScale.y, this.transform.localScale.z);
        }
        else if (xDirection > 0 && this.transform.localScale.x < 0) // going right and facing left flip
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x * -1, this.transform.localScale.y, this.transform.localScale.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<NPC>() is IHasDialogue speaker)
        {
            Debug.Log("can interact with" + collision.name);
            this.interactableInRange = speaker as IInteracble;
        }
    }

    private bool hasSkippedDialogue;

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

        Debug.Log("Interact flag set");
        this.InteractFlagSet = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<NPC>() as IHasDialogue == this.interactableInRange)
        {
            interactableInRange = null;
        }
    }
}
