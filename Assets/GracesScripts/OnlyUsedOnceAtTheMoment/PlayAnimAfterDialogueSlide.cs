using UnityEngine;
using UnityEngine.UIElements;
#nullable enable

public class PlayAnimAfterDialogueSlide : MonoBehaviour
{
    private GameObject? dialogueBox;
    [SerializeField] private Animator animator;
    private TorchState state = TorchState.waiting;

    private enum TorchState
    {
        waiting, 
        talking,
        finishedTalking,
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Torch touching player");
    }

    private void Update()
    {
        switch (this.state)
        {
            case TorchState.waiting:
                if (this.dialogueBox == null)
                {
                    var diaBox = FindObjectOfType<DialogueTextBox>();
                    this.dialogueBox = diaBox.gameObject;
                }
                else if (this.dialogueBox.activeSelf)
                {
                    this.state = TorchState.talking;
                }
                break;
            case TorchState.talking:
                if (!this.dialogueBox.activeSelf)
                {
                    this.state = TorchState.finishedTalking;
                    OnDialogueBoxClose();
                }
                break;
            case TorchState.finishedTalking:
                break;
        }

    }

    private void OnDialogueBoxClose()
    {
        Debug.Log("called on dialogue box close.");

        animator.SetTrigger("FinishedTalk");
    }
}
