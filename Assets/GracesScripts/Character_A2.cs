using Assets.GracesScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_A2 : NPC, IHasDialogue
{
    protected DialogueSlide firstDialogue { get; private set; }
    ProfilePics pics;

    public DialogueSlide GetFirstDialogueSlide()
    {
        return this.firstDialogue;
    }

    private void Awake()
    {
        this.pics = FindObjectOfType<ProfilePics>();
    }

    // Start is called before the first frame update
    void Start()
    {
        var slide4 = gameObject.AddComponent<DialogueSlide>();
        slide4.SetValues(this.pics.characterA, "It's July...");

        var slide3 = gameObject.AddComponent<DialogueSlide>();
        slide3.SetValues(this.pics.characterB, "new year new me.", slide4);

        var SlideAfter2 = gameObject.AddComponent<DialogueSlide>();
        SlideAfter2.SetValues(this.pics.characterA, "but you're scared of horses", slide3);

        var Slide1 = gameObject.AddComponent<DialogueSlide>();
        Slide1.SetValues(this.pics.characterB, "ooo a jousting competition I'm gonna sign up", SlideAfter2);

        this.firstDialogue = Slide1;

        this.firstDialogue = Slide1;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
