using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mildlyInterestingRock : MonoBehaviour, IHasDialogue
{
    protected OLDDialogueSlide dialogueSlide;

    public OLDDialogueSlide GetFirstDialogueSlide()
    {
        return dialogueSlide;
    }

    // Start is called before the first frame update
    void Start()
    {
        var Slide1Option1_NextSlide = gameObject.AddComponent<OLDDialogueSlide>();
        Slide1Option1_NextSlide.SetValues("*CLANG* ___ OW! fuck off.");

        var Slide1Option2_NextSlide = gameObject.AddComponent<OLDDialogueSlide>();
        Slide1Option2_NextSlide.SetValues("no");

        var Slide1Option1 = gameObject.AddComponent<DialogueOptionButton>();
        Slide1Option1.SetValues("open up! *strike the rock with your sword*", Slide1Option1_NextSlide);

        var Slide1Option2 = gameObject.AddComponent<DialogueOptionButton>();
        Slide1Option2.SetValues("May I please Look Underneath", Slide1Option2_NextSlide);

        var Slide1 = gameObject.AddComponent<OLDDialogueSlide>();
        Slide1.SetValues("... ___ugh...___what?", new List<DialogueOptionButton>() { Slide1Option1, Slide1Option2 });

        this.dialogueSlide = Slide1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
