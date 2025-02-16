using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_A : SpeakingNPC
{
    // Start is called before the first frame update
    void Start()
    {
        var Slide1Option1_NextSlide = gameObject.AddComponent<DialogueSlide>();
        Slide1Option1_NextSlide.SetValues("k bye", true, null, null);

        var Slide1Option2_NextSlide = gameObject.AddComponent<DialogueSlide>();
        Slide1Option2_NextSlide.SetValues("fuck you too bitch", true, null, null);

        var Slide1Option1 = gameObject.AddComponent<DialogueOptionButton>();
        Slide1Option1.SetValues("what?", Slide1Option1_NextSlide);

        var Slide1Option2 = gameObject.AddComponent<DialogueOptionButton>();
        Slide1Option2.SetValues("FuckYou", Slide1Option2_NextSlide);

        var Slide1 = gameObject.AddComponent<DialogueSlide>();
        Slide1.SetValues("Hey girlll, ____what is this, ___like Undertale Text?", false, new List<DialogueOptionButton>() { Slide1Option1, Slide1Option2 }, null);

        this.dialogueSlide = Slide1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
