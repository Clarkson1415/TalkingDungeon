using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : UsesProfilePic, IHasDialogue
{
    protected DialogueSlide firstDialogue { get; private set; }

    public DialogueSlide GetFirstDialogueSlide()
    {
        return this.firstDialogue;
    }

    // Start is called before the first frame update
    void Start()
    {
        var Slide1Option1_NextSlide = gameObject.AddComponent<DialogueSlide>();
        Slide1Option1_NextSlide.SetValues(this.profilePic, "omg yay!");

        var Slide1Option1 = gameObject.AddComponent<DialogueOptionButton>();
        Slide1Option1.SetValues("yes.", Slide1Option1_NextSlide);

        // Slide1Option2_NextSlide
        var Slide1Option2_NextSlide = gameObject.AddComponent<DialogueSlide>();
        Slide1Option2_NextSlide.SetValues(this.profilePic, "Go fuck yourself.");

        var Slide1Option2 = gameObject.AddComponent<DialogueOptionButton>();
        Slide1Option2.SetValues("no", Slide1Option2_NextSlide);

        // Slide 1
        var Slide1 = gameObject.AddComponent<DialogueSlide>();
        Slide1.SetValues(this.profilePic, "You want to buy?", new List<DialogueOptionButton>() { Slide1Option1, Slide1Option2});

        this.firstDialogue = Slide1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
