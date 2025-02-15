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
    
    /// <summary>
    /// TODO: font size setting, and this based on font size setting.
    /// max characters for text box
    /// </summary>
    static private int MaxChars = 212;

    public DialogueSlide(string dialogue, DialogueSlide nextSlide)
    {
        if(dialogue.Length > 212)
        {
            throw new ArgumentOutOfRangeException("cant ave more than 120 chars in current font size.");
        }

        this.dialogue = dialogue;
        this.nextSlide = nextSlide;
    }

    public DialogueSlide(string dialogue, bool? lastSlideInSequence = null)
    {
        if (dialogue.Length > 120)
        {
            throw new ArgumentOutOfRangeException("cant ave more than 120 chars in current font size.");
        }

        this.dialogue = dialogue;
        this.lastSlideInSequence = true;
    }

    public DialogueSlide(string dialogue, List<DialogueOption> options)
    {
        if (dialogue.Length > 120)
        {
            throw new ArgumentOutOfRangeException("cant ave more than 120 chars in current font size.");
        }

        this.dialogue = dialogue;
        this.options = options;
    }
}
