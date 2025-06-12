using Assets.GracesScripts;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// See Dog Prefab for how to use. sets objects animations to true when velocity high, and filps sprite, "Running" is the animator parameter set. Only for left and right movement at the moment.
/// Works with animated layers component also now? or should it be separate? idk
/// ALSO: uses bools specific for the animation layer: the bool string is "Running"
/// </summary>
[RequireComponent(typeof(UseAnimatedLayers))]
public class WalkingBackAndForthUnit : Unit_NPC
{
    [SerializeField] int runSpeed = 6;
    [SerializeField] int runTime = 3;
    [SerializeField] int idleWaitTime = 2;
    private Vector2 velocity = new(1, 0);
    MovingDialogueNPCState state;
    private Rigidbody2D rb;
    private float runStartTime;
    private float idleStartTime;
    [SerializeField] float leftBound = -36;
    [SerializeField] float rightBound = -22;
    [SerializeField] private bool ShouldSpriteFlipOnDirectionChange;

    private enum MovingDialogueNPCState
    {
        INTERACTING,
        MOVING,
        IDLEING,
    }

    protected override void Die()
    {
        Debug.Log("DIE? do i need todo?");
        throw new NotImplementedException("walking dog dies");
    }

    // Start is called before the first frame update
    void Start()
    {
        this.rb = GetComponent<Rigidbody2D>();
        state = MovingDialogueNPCState.MOVING;
        this.animatedLayers.SetBools("Running", true);
        runStartTime = Time.time;
    }


    // Update is called once per frame
    void Update()
    {
        if (ShouldSpriteFlipOnDirectionChange)
        {
            FlipSprite(this.velocity.x);
        }

        if (!IsInBounds() && canFlipAgain)
        {
            SwapDirectionGoing();
        }

        switch (state)
        {
            case MovingDialogueNPCState.INTERACTING:
                this.rb.velocity = Vector2.zero;
                if (!this.FinishedInteraction)
                {
                    velocity = new Vector2(1, 0);
                    this.animatedLayers.SetBools("Running", true);
                    this.state = MovingDialogueNPCState.MOVING;
                    this.runStartTime = Time.time;
                    Log.Print("stationary changed to moving");
                }
                break;
            case MovingDialogueNPCState.MOVING:
                this.rb.velocity = runSpeed * velocity;
                if (this.FinishedInteraction)
                {
                    Log.Print("Moving changed to stationary");
                    this.animatedLayers.SetBools("Running", false);
                    this.state = MovingDialogueNPCState.INTERACTING;
                }
                if (Time.time >= runStartTime + runTime)
                {
                    this.animatedLayers.SetBools("Running", false);
                    this.idleStartTime = Time.time;
                    this.state = MovingDialogueNPCState.IDLEING;
                }
                break;
            case MovingDialogueNPCState.IDLEING:
                this.rb.velocity = Vector2.zero;
                if (this.FinishedInteraction)
                {
                    Log.Print("Moving changed to stationary");
                    this.idleStartTime = Time.time;
                    this.state = MovingDialogueNPCState.INTERACTING;
                }
                if (Time.time >= idleStartTime + idleWaitTime)
                {
                    velocity = new Vector2(1, 0);
                    this.animatedLayers.SetBools("Running", true);
                    this.runStartTime = Time.time;
                    this.state = MovingDialogueNPCState.MOVING;
                }
                break;
            default:
                break;
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

    private bool IsInBounds()
    {
        // if less than left bound and going left, then needs to change direction
        if (this.gameObject.transform.position.x < leftBound && this.velocity.x < 0)
        {
            return false;
        }

        // if greater than right bound and going right, then needs to change direction
        if (this.gameObject.transform.position.x < leftBound || this.gameObject.transform.position.x > rightBound)
        {
            return false;
        }

        return true;
    }

    private void SwapDirectionGoing()
    {
        this.velocity.x *= -1;
    }

    private bool canFlipAgain = true;
    IEnumerator FlipDelay()
    {
        canFlipAgain = false;
        yield return new WaitForSeconds(0.5f);
        canFlipAgain = true;
    }
}
