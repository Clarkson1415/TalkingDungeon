using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necromancer : UsesProfilePic, IHasDialogue, IInteracble
{
    protected DialogueSlide FirstDialogue { get; private set; }
    public DialogueSlide GetFirstDialogueSlide()
    {
        return this.FirstDialogue;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Slide1Option1_NextSlide
        var Slide1Option1_NextSlide = gameObject.AddComponent<DialogueSlide>();
        Slide1Option1_NextSlide.SetValues(this.profilePic, ".___ .___ .___ *he goes back to chanting* insert heiroglyphics here ", new List<DialogueOptionButton>() { });

        // Slide 1 Options
        var Slide1Option1 = gameObject.AddComponent<DialogueOptionButton>();
        Slide1Option1.SetValues("*stay silent*", Slide1Option1_NextSlide);

        var Slide1Option3 = gameObject.AddComponent<DialogueOptionButton>();
        Slide1Option3.SetValues("what?", Slide1Option1_NextSlide);

        // Slide 1
        var Slide1 = gameObject.AddComponent<DialogueSlide>();
        Slide1.SetValues(this.profilePic, "shhh! ah nevermind you just interrupted me. I was almost out of here.", new List<DialogueOptionButton>() { Slide1Option1, Slide1Option3 });

        this.FirstDialogue = Slide1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
