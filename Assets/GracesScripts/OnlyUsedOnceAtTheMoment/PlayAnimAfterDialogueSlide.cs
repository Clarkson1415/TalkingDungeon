using UnityEngine;
#nullable enable

public class PlayAnimAfterDialogueSlide : MonoBehaviour
{
    private GameObject dialogueBox;
    [SerializeField] private Animator animator;
    private TorchState state = TorchState.waiting;
    private bool playerInRange = false;

    private enum TorchState
    {
        waiting,
        talking,
        finishedTalking,
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerDungeon>(out var p))
        {
            playerInRange = true;
        }
    }

    private void Update()
    {
        if (!playerInRange)
        {
            return;
        }

        switch (this.state)
        {
            case TorchState.waiting:
                if (this.dialogueBox == null)
                {
                    var diaBox = FindObjectOfType<DialogueTextBox>();
                    if (diaBox != null)
                    {
                        this.dialogueBox = diaBox.gameObject;
                    }
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
