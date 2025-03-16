using Assets.GracesScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

/// <summary>
/// See Dog for how to use. sets objects animations to true when velocity high, and filps sprite, "Running" is the animator parameter set. Only for left and right movement at the moment.
/// TODO: could be split into 2 components, 1 for layered animaterions in just idle, and one for movement with layered animations as an extension of that
/// </summary>
public abstract class WalkingBackAndForthNPC : NPC
{
    [SerializeField] List<Animator> OverlayingAnimations;
    [SerializeField] int runSpeed = 6;
    [SerializeField] int runTime = 3;
    private Coroutine currentRoutine;
    private Vector2 velocity = new(1, 0);
    MovingDialogueNPCState state;
    public bool IsStationary { get; set; } = false;
    private int isStationary;

    private enum MovingDialogueNPCState
    {
        STATIONARY,
        MOVING,
    }

    // Start is called before the first frame update
    void Start()
    {
        state = MovingDialogueNPCState.MOVING;
        Log.Print("dog moving initial");
    }

    [SerializeField] int leftBound = -36;
    [SerializeField] int rightBound = -22;

    private IEnumerator RunLoop()
    {
        while (true)
        {
            isStationary = 1;
            ChangeAnimation();
            yield return new WaitForSeconds(runTime);

            // IDLE
            isStationary = 0;
            ChangeAnimation();
            yield return new WaitForSeconds(1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponent<Rigidbody2D>().velocity = isStationary * runSpeed * velocity;
        FlipSprite(this.velocity.x);

        if (!IsInBounds())
        {
            SwapDirectionGoing();
        }

        switch (state)
        {
            case MovingDialogueNPCState.STATIONARY:
                if (!IsStationary)
                {
                    this.state = MovingDialogueNPCState.MOVING;
                    currentRoutine = StartCoroutine(RunLoop());
                    Log.Print("stationary changed to moving");
                }
                break;
            case MovingDialogueNPCState.MOVING:
                if (IsStationary)
                {
                    StopAllCoroutines();
                    isStationary = 0;
                    ChangeAnimation();
                    Log.Print("Moving changed to stationary");
                    this.state = MovingDialogueNPCState.STATIONARY;
                }
                break;
            default:
                break;
        }
    }

    private void ChangeAnimation()
    {
        bool isRunning = isStationary != 0;
        foreach (var animator in this.OverlayingAnimations)
        {
            animator.SetBool("Running", isRunning);
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
        if (this.gameObject.transform.position.x < leftBound || this.gameObject.transform.position.x > rightBound)
        {
            return false;
        }

        return true;
    }

    bool velocityFlipDelayUp = true;

    private void SwapDirectionGoing()
    {
        if (velocityFlipDelayUp)
        {
            this.velocity.x *= -1;
            StartCoroutine(FlipVelocityDelay());
        }
    }

    /// <summary>
    /// If no delay after flipping velocity direction he gets stuck on the bounds edges.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlipVelocityDelay()
    {
        velocityFlipDelayUp = false;
        yield return new WaitForSeconds(0.5f);
        velocityFlipDelayUp = true;
    }
}
