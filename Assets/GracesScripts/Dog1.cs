using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog1 : WalkingBackAndForthNPC, IHasDialogue
{
    protected DialogueSlide FirstDialogue { get; private set; }

    public DialogueSlide GetFirstDialogueSlide()
    {
        return this.FirstDialogue;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Slide 1
        var Slide1 = this.gameObject.AddComponent<DialogueSlide>();
        Slide1.SetValues(this.profilePic, "Must._____ Catch._____ Bug.");

        this.FirstDialogue = Slide1;
    }
}
