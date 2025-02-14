using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
#nullable enable

public class DialogueSlide : MonoBehaviour
{
    public string dialogue { get; private set; }
    public List<DialogueOption>? options { get; private set; }
    public DialogueSlide? nextSlide { get; private set; }
    public bool lastSlideInSequence { get; private set; }

    public DialogueSlide(string dialogue, DialogueSlide nextSlide)
    {
        this.dialogue = dialogue;
        this.nextSlide = nextSlide;
    }

    public DialogueSlide(string dialogue, bool? lastSlideInSequence = null)
    {
        this.dialogue = dialogue;
        this.lastSlideInSequence = true;
    }

    public DialogueSlide(string dialogue, List<DialogueOption> options)
    {
        this.dialogue = dialogue;
        this.options = options;
    }
}
