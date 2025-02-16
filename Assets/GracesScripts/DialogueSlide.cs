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
    public List<DialogueOptionButton>? options { get; private set; }
    public DialogueSlide? nextSlide { get; private set; }
    public bool lastSlideInSequence { get; private set; }
    
    /// <summary>
    /// TODO: font size setting, and this based on font size setting.
    /// max characters for text box
    /// </summary>
    static private int MaxChars = 212;

    /// <summary>
    /// Must pass in either options, last slideInSequence or nextSlide which is the slide played if there are no options.
    /// TODO: make other a few, more restrictive  SetValue functions to restrict me putting in the wrong values.
    /// </summary>
    /// <param name="dialogue"></param>
    /// <param name="isLastSlide"></param>
    /// <param name="options"></param>
    /// <param name="nextSlide"></param>
    public void SetValues(string dialogue, bool? isLastSlide, List<DialogueOptionButton>? options, DialogueSlide? nextSlide)
    {
        if(isLastSlide == false && options == null && nextSlide == null)
        {
            throw new ArgumentException("cannot have isLast Slide false, and no options or next slide to go to.");
        }

        if (isLastSlide == null && options == null && nextSlide == null)
        {
            throw new ArgumentException($"One of {nameof(isLastSlide)}, {nameof(options)}, {nameof(nextSlide)} has to be set");
        }

        this.dialogue = dialogue;
        this.lastSlideInSequence = isLastSlide ?? false; // if isLastSlide null set to false;
        this.options = options;
        this.nextSlide = nextSlide;
    }

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

    public DialogueSlide(string dialogue, List<DialogueOptionButton> options)
    {
        if (dialogue.Length > 120)
        {
            throw new ArgumentOutOfRangeException("cant ave more than 120 chars in current font size.");
        }

        this.dialogue = dialogue;
        this.options = options;
    }
}
