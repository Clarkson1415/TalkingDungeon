using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UsesProfilePic))]
public class Dog1 : WalkingBackAndForth, IHasDialogue
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
        //var Slide1 = this.gameObject.AddComponent<DialogueSlide>();
        //Slide1.SetValues(GetComponent<UsesProfilePic>().ProfilePic, "Must._____ Catch._____ Bug.");

        //this.FirstDialogue = Slide1;
    }
}
