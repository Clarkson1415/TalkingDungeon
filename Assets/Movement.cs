using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Movement class. To be used for all entities in game. This Project will use the new unity Input system.
/// </summary>
public class Movement : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 direction;

    [SerializeField]
    private float movementSpeed = 1f;

    void Awake()
    {
        this.animator = GetComponent<Animator>();
        this.rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void FixedUpdate()
    {
        this.rb.velocity = direction * movementSpeed;
        FlipSprite(this.direction.x);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        this.direction = context.ReadValue<Vector2>();

        if (context.started)
        {
            this.animator.SetBool("Running", true);
        }
        else if (context.performed)
            Debug.Log("OnMove was performed");
        else if (context.canceled)
        {
            Debug.Log("OnMove was cancelled");
            this.animator.SetBool("Running", false);
        }

    }

    void FlipSprite(float xDirection)
    {
        // if going left and facing right flip
        if(xDirection < 0 && this.transform.localScale.x > 0)
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x * -1, this.transform.localScale.y, this.transform.localScale.z);
        }
        else if(xDirection > 0 && this.transform.localScale.x < 0) // going right and facing left flip
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x * -1, this.transform.localScale.y, this.transform.localScale.z);
        }
    }
}
