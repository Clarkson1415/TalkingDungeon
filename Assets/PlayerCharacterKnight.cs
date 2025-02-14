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
    protected bool movementDisabled;

    [SerializeField]
    private float movementSpeed = 1f;

    void Awake()
    {
        this.animator = this.GetComponent<Animator>();
        this.rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        // TODO: state machine, this.state == IDLE 
        if (this.movementDisabled)
        {
            this.rb.velocity = Vector2.zero;
            return;
        }
        else
        {
            this.rb.velocity = direction * movementSpeed;
            FlipSprite(this.direction.x);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
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

        // TODO: disable movement until finished interaction? yes, put knight in IDLE_INTERACTING state. instead
        this.movementDisabled = true;

        // TODO: DialogueBox state machine setup instead of this shit here and instead just fires event in dialogue box if knight interacts with object. reguardless if it's the first time or not.
        if (this.interactableInRange is IHasDialogue interactableWithDialogue)
        {

            if (isDialogueSlidePrinting && dialogueBox.HasShownLastSlide)
            {
                this.movementDisabled = false;
                return;
            }

            if (hasSkippedDialogue)
            {
                dialogueBox.gameObject.SetActive(false);
                return;
            }

            if (isDialogueSlidePrinting)
            {
                Debug.Log("pressed space another time.");
                dialogueBox.SkipToEnd();
                hasSkippedDialogue = true;
                return;
            }

            // TODO: if Dialogue finished when press space should clear last slide.
            Debug.Log("speaking");
            isDialogueSlidePrinting = true;
            dialogueBox.gameObject.SetActive(true);
            dialogueBox.OnNewSpeaker(interactableWithDialogue);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<NPC>() as IHasDialogue == this.interactableInRange)
        {
            interactableInRange = null;
        }
    }
}
