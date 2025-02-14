using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class do not add to unity component
/// </summary>
public abstract class NPC : MonoBehaviour, IHasDialogue
{
    protected List<DialogueSlide> dialogueSlides;

    public DialogueSlide GetFirstDialogueSlide()
    {
        return this.dialogueSlides[0];
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
