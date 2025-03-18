using Assets.GracesScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterB : UsesProfilePic, IHasDialogue
{
    protected DialogueSlide firstDialogue { get; private set; }

    public DialogueSlide GetFirstDialogueSlide()
    {
        return this.firstDialogue;
    }

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        // need to get profile pics of other NPC talking to
        Sprite characterAPicture = FindObjectOfType<Character_A>().ProfilePic;

        var slide4 = gameObject.AddComponent<DialogueSlide>();
        slide4.SetValues(characterAPicture, "It's July...");

        var slide3 = gameObject.AddComponent<DialogueSlide>();
        slide3.SetValues(this.profilePic, "new year new me.", slide4);

        var SlideAfter2 = gameObject.AddComponent<DialogueSlide>();
        SlideAfter2.SetValues(characterAPicture, "but you're scared of horses", slide3);

        var Slide1 = gameObject.AddComponent<DialogueSlide>();
        Slide1.SetValues(this.profilePic, "ooo a jousting competition I'm gonna sign up", SlideAfter2);

        this.firstDialogue = Slide1;

        this.firstDialogue = Slide1;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
