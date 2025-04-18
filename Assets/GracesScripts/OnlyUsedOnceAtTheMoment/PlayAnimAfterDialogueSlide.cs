using UnityEngine;
#nullable enable

public class PlayAnimAfterDialogueSlide : MonoBehaviour
{
    private GameObject? dialogueBox;
    [SerializeField] private Animator animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Torch touching player");
    }

    private void Update()
    {
        if (dialogueBox != null) // if is not null it is found.
        {
            Debug.Log("dialogueBox != null");

            if (!dialogueBox.activeSelf) // if no longer active
            {
                Debug.Log("dialogue de activated.");
                OnDialogueBoxClose();
                this.dialogueBox = null;
            }
        }
        else
        {
            Debug.Log("dialogueBox == null");

            var diaBox = FindObjectOfType<DialogueTextBox>();

            if (diaBox != null)
            {
                Debug.Log("dialogue box found");
                this.dialogueBox = diaBox.gameObject;
            }
        }
    }

    private void OnDialogueBoxClose()
    {
        Debug.Log("called on dialogue box close.");

        animator.SetTrigger("FinishedTalk");
    }
}
