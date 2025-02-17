using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
[System.Serializable]

public class DialogueSlide : MonoBehaviour
{
    [SerializeField] public string? dialogue { get; set; }
    [SerializeField] public List<DialogueOptionButton>? options { get; set; }
    [SerializeField] public DialogueSlide? nextSlide { get; set; }
    public bool islastSlideInSequence { get; private set; } = false;

    /// <summary>
    /// TODO: font size setting, and this based on font size setting.
    /// max characters for text box
    /// </summary>
    static private int MaxChars = 212;

    private void Awake()
    {
    }

    /// <summary>
    /// Must pass in either options, last slideInSequence or nextSlide which is the slide played if there are no options.
    /// TODO: make other a few, more restrictive  SetValue functions to restrict me putting in the wrong values.
    /// </summary>
    /// <param name="dialogue"></param>
    /// <param name="isLastSlide"></param>
    /// <param name="options"></param>
    /// <param name="nextSlide"></param>
    public void SetValues(string dialogue, List<DialogueOptionButton> options)
    {
        if (dialogue.Length > MaxChars)
        {
            throw new ArgumentOutOfRangeException("cant ave more than 120 chars in current font size.");
        }

        //Guard.ArgumentNotNull(options);
        this.dialogue = dialogue;
        this.options = options;
    }

    public void SetValues(string dialogue, DialogueSlide nextSlide)
    {
        if (dialogue.Length > MaxChars)
        {
            throw new ArgumentOutOfRangeException("cant ave more than 120 chars in current font size.");
        }

        MyGuard.IsNotNull(nextSlide);
        this.islastSlideInSequence = (nextSlide == null);
        this.dialogue = dialogue;
        this.nextSlide = nextSlide;
    }

    public void SetValues(string dialogue)
    {
        if (dialogue.Length > MaxChars)
        {
            throw new ArgumentOutOfRangeException("cant ave more than 120 chars in current font size.");
        }

        this.dialogue = dialogue;
        this.islastSlideInSequence = true;
    }
}
