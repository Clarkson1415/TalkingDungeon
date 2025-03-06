using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// See Dog for how to use. sets objects animations to true when velocity high, and filps sprite, "Running" is the animator parameter set. Only for left and right movement at the moment.
/// TODO: could be split into 2 components, 1 for layered animaterions in just idle, and one for movement with layered animations as an extension of that
/// </summary>
public class NPCWalkingBackAndForthWithAnimatedLayers : MonoBehaviour
{
    [SerializeField] List<Animator> OverlayingAnimations;
    [SerializeField] int runVelocity = 6;
    [SerializeField] int runTime = 3;
    private Coroutine currentRoutine;
    private Vector2 direction;

    // Start is called before the first frame update
    void Start()
    {
        currentRoutine = StartCoroutine(IdleCoroutine());
    }

    IEnumerator IdleCoroutine()
    {
        direction = Vector2.zero;
        ChangeAnimation();
        yield return new WaitForSeconds(4);

        currentRoutine = StartCoroutine(RunCoroutine());
    }

    IEnumerator RunCoroutine()
    {
        direction = new Vector2(-runVelocity, 0);
        ChangeAnimation();
        yield return new WaitForSeconds(runTime);
        direction = new Vector2(runVelocity, 0);
        yield return new WaitForSeconds(runTime);
        direction = new Vector2(-runVelocity, 0);
        yield return new WaitForSeconds(runTime);
        direction = new Vector2(runVelocity, 0);
        yield return new WaitForSeconds(runTime);

        currentRoutine = StartCoroutine(IdleCoroutine());
    }
    
    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponent<Rigidbody2D>().velocity = this.direction;
        FlipSprite(this.direction.x);
    }

    private void ChangeAnimation()
    {
        bool isRunning = (this.direction.x != 0) || (this.direction.y != 0);
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
}
