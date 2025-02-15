using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterA : NPC, IHasDialogue
{
    // Start is called before the first frame update
    void Start()
    {
        var Slide2_lastSlide = new DialogueSlide("k bye", true);
        var Slide1Option1 = new DialogueOption("what?", Slide2_lastSlide);
        var Slide1Option2 = new DialogueOption("Fuck you", Slide2_lastSlide);
        var Slide1 = new DialogueSlide("HI or something longer to test the dialogue printing.", new List<DialogueOption>() 
        { Slide1Option1, Slide1Option2});

        // HI
        // options: what? , Fuck you
        // both lead to: k bye.

        dialogueSlides = new List<DialogueSlide>()
        {
            Slide1, Slide2_lastSlide,
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
