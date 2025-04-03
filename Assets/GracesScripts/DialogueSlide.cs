using Assets.GracesScripts;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
[System.Serializable]

public class DialogueSlide : MonoBehaviour
{
    [SerializeField] public string? dialogue { get; set; }
    [SerializeField] public List<DialogueOptionButton> options { get; set; } = new();
    [SerializeField] public DialogueSlide? nextSlide { get; set; }
    public Sprite? SpeakerPic { get; set; }
    public bool islastSlideInSequence { get; private set; } = false;
    

    /// <summary>
    /// TODO: change so this is this based on font size setting.
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
    /// <param name="options"></param>
    public void SetValues(Sprite speakerPicture,string dialogue, List<DialogueOptionButton> options)
    {
        if (dialogue.Length > MaxChars)
        {
            throw new ArgumentOutOfRangeException("cant ave more than 120 chars in current font size.");
        }

        //Guard.ArgumentNotNull(options);
        this.SpeakerPic = speakerPicture;
        this.dialogue = dialogue;
        this.options = options;
        if(this.options.Count == 0)
        {
            Log.Print($"did you mean to have no option on the slide? for: pic: {speakerPicture}, \n Dialogue: {dialogue} \n this.name: {this.name}");
            this.islastSlideInSequence = true;
        }
    }

    public void SetValues(Sprite speakerPicture, string dialogue, DialogueSlide nextSlide)
    {
        if (dialogue.Length > MaxChars)
        {
            throw new ArgumentOutOfRangeException("cant ave more than 120 chars in current font size.");
        }

        MyGuard.IsNotNull(nextSlide);
        this.SpeakerPic = speakerPicture;
        this.islastSlideInSequence = (nextSlide == null);
        this.dialogue = dialogue;
        this.nextSlide = nextSlide;
    }

    public void SetValues(Sprite speakerPicture, string dialogue)
    {
        if (dialogue.Length > MaxChars)
        {
            throw new ArgumentOutOfRangeException("cant ave more than 120 chars in current font size.");
        }

        this.SpeakerPic = speakerPicture;
        this.dialogue = dialogue;
        this.islastSlideInSequence = true;
    }
}
