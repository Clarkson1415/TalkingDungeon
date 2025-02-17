using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_A : NPC, IHasDialogue
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
        Slide1Option1_NextSlide.SetValues("nothin___ ...___ ur hot");

        var Slide1Option2_NextSlide = gameObject.AddComponent<DialogueSlide>();
        Slide1Option2_NextSlide.SetValues("fuck you too bitch");

        var Slide1Option1 = gameObject.AddComponent<DialogueOptionButton>();
        Slide1Option1.SetValues("what?", Slide1Option1_NextSlide);

        var Slide1Option2 = gameObject.AddComponent<DialogueOptionButton>();
        Slide1Option2.SetValues("FuckYou", Slide1Option2_NextSlide);

        var Slide1Option3 = gameObject.AddComponent<DialogueOptionButton>();
        Slide1Option3.SetValues("FuckYou", Slide1Option2_NextSlide);

        var Slide1Option4 = gameObject.AddComponent<DialogueOptionButton>();
        Slide1Option4.SetValues("FuckYou", Slide1Option2_NextSlide);

        var Slide1 = gameObject.AddComponent<DialogueSlide>();
        Slide1.SetValues("Hey girlll, ____what is this, ___like Undertale Text?", new List<DialogueOptionButton>() { Slide1Option1, Slide1Option2, Slide1Option3, Slide1Option4});

        this.firstDialogue = Slide1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
