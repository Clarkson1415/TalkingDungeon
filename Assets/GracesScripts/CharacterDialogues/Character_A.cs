using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_A : UsesProfilePic, IHasDialogue
{
    protected DialogueSlide FirstDialogue { get; private set; }

    public DialogueSlide GetFirstDialogueSlide()
    {
        return this.FirstDialogue;
    }

    // Start is called before the first frame update
    void Start()
    {
        //// Slide1Option1_NextSlide_Option1_NextSlide
        //var Slide1Option1_NextSlide_Option1_NextSlide = gameObject.AddComponent<DialogueSlide>();
        //Slide1Option1_NextSlide_Option1_NextSlide.SetValues(this.profilePic, "ookkkayyyyy... maybe we need to go to the medical tent");

        ////Slide1Option1_NextSlide options
        //var Slide1Option1_NextSlideOption1 = gameObject.AddComponent<DialogueOptionButton>();
        //Slide1Option1_NextSlideOption1.SetValues("Larping?", Slide1Option1_NextSlide_Option1_NextSlide);

        ////Slide1Option1_NextSlide
        //var Slide1Option1_NextSlide = gameObject.AddComponent<DialogueSlide>();
        //Slide1Option1_NextSlide.SetValues(this.profilePic, "you alright there bud? Hit your head larping?", new List<DialogueOptionButton>() { Slide1Option1_NextSlideOption1 });

        //// Slide1Option2_NextSlide_Option2_NextSlide Options
        //var Slide1Option2_NextSlide_Option2_NextSlide_Option1 = gameObject.AddComponent<DialogueOptionButton>();
        //Slide1Option2_NextSlide_Option2_NextSlide_Option1.SetValues("Where Am I?", Slide1Option1_NextSlide_Option1_NextSlide);

        //// Slide1Option2_NextSlide_Option2_NextSlide
        //var Slide1Option2_NextSlide_Option2_NextSlide = gameObject.AddComponent<DialogueSlide>();
        //Slide1Option2_NextSlide_Option2_NextSlide.SetValues(this.profilePic, "ha___.____.____. ha___.____.____. ha.", new List<DialogueOptionButton> { Slide1Option2_NextSlide_Option2_NextSlide_Option1 });

        //// Slide1Option2_NextSlide_Option1_NextSlide Options
        //var Slide1Option2_NextSlide_Option1_NextSlide_Option1 = gameObject.AddComponent<DialogueOptionButton>();
        //Slide1Option2_NextSlide_Option1_NextSlide_Option1.SetValues("Where Am I?", Slide1Option1_NextSlide_Option1_NextSlide);

        //// Slide1Option2_NextSlide_Option1_NextSlide
        //var Slide1Option2_NextSlide_Option1_NextSlide = gameObject.AddComponent<DialogueSlide>();
        //Slide1Option2_NextSlide_Option1_NextSlide.SetValues(this.profilePic, "ha ok.", new List<DialogueOptionButton> { Slide1Option2_NextSlide_Option1_NextSlide_Option1 });

        //// Slide1Option2_NextSlide Options
        //var Slide1Option2_NextSlide_Option2 = gameObject.AddComponent<DialogueOptionButton>();
        //Slide1Option2_NextSlide_Option2.SetValues("It was my father's I took it after I killed him.", Slide1Option2_NextSlide_Option2_NextSlide);

        //var Slide1Option2_NextSlide_Option1 = gameObject.AddComponent<DialogueOptionButton>();
        //Slide1Option2_NextSlide_Option1.SetValues("yeah, I took if from ur mums house after last night.", Slide1Option2_NextSlide_Option1_NextSlide);

        //// Slide1Option2_NextSlide
        //var Slide1Option2_NextSlide = gameObject.AddComponent<DialogueSlide>();
        //Slide1Option2_NextSlide.SetValues(this.profilePic, "oh cool, family heirloom?", new List<DialogueOptionButton>() { Slide1Option2_NextSlide_Option1, Slide1Option2_NextSlide_Option2 });

        //// Slide 1 Options
        //var Slide1Option1 = gameObject.AddComponent<DialogueOptionButton>();
        //Slide1Option1.SetValues("*stay silent*", Slide1Option1_NextSlide);

        //var Slide1Option2 = gameObject.AddComponent<DialogueOptionButton>();
        //Slide1Option2.SetValues("it is", Slide1Option2_NextSlide);

        //var Slide1Option3 = gameObject.AddComponent<DialogueOptionButton>();
        //Slide1Option3.SetValues("Whats going on here?", Slide1Option1_NextSlide);

        //// Slide 1
        //var Slide1 = gameObject.AddComponent<DialogueSlide>();
        //Slide1.SetValues(this.profilePic, "Hey nice suit of armour, looks real!", new List<DialogueOptionButton>() { Slide1Option1, Slide1Option2, Slide1Option3});

        // this.FirstDialogue = Slide1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
