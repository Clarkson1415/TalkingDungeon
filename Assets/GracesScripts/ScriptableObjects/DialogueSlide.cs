using Assets.GracesScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#nullable enable
[System.Serializable]

[CreateAssetMenu(fileName = "DialogueSlide", menuName = "ScriptableObjects/Dialogue/DialogueSlide", order = 1)]
public class DialogueSlide : ScriptableObject
{
    public bool startFight;

    public string dialogue;

    public List<DialogueOption> dialogueOptions = new();

    public DialogueSlide? nextSlide;

    public Sprite? SpeakerPic;

    /// <summary>
    /// TODO: change so this is this based on font size setting.
    /// max characters for text box
    /// </summary>
    private static readonly int MaxChars = 212;

    private void Awake()
    {
        if(this.dialogue.Length > MaxChars)
        {
            Debug.LogError($"problem in slide {this.dialogue} too many characters");
        }

        if (dialogueOptions.Count() > 4)
        {
            throw new NotImplementedException($"dialouge text box does not have more than 4 positions!!! check dialogue slide {this.dialogue}");
        }
    }

    ///// <summary>
    ///// Must pass in either options, last slideInSequence or nextSlide which is the slide played if there are no options.
    ///// TODO: make other a few, more restrictive  SetValue functions to restrict me putting in the wrong values.
    ///// </summary>
    ///// <param name="dialogue"></param>
    ///// <param name="options"></param>
    //public void SetValues(Sprite speakerPicture,string dialogue, List<DialogueOptionButton> options)
    //{
    //    if (dialogue.Length > MaxChars)
    //    {
    //        throw new ArgumentOutOfRangeException("cant ave more than 120 chars in current font size.");
    //    }

    //    //Guard.ArgumentNotNull(options);
    //    this.SpeakerPic = speakerPicture;
    //    this.dialogue = dialogue;
    //    this.dialogueOptionButtons = options;
    //    if(this.dialogueOptionButtons.Count == 0)
    //    {
    //        Log.Print($"did you mean to have no option on the slide? for: pic: {speakerPicture}, \n Dialogue: {dialogue} \n this.name: {this.name}");
    //        this.islastSlideInSequence = true;
    //    }
    //}

    //public void SetValues(Sprite speakerPicture, string dialogue, DialogueSlide nextSlide)
    //{
    //    if (dialogue.Length > MaxChars)
    //    {
    //        throw new ArgumentOutOfRangeException("cant ave more than 120 chars in current font size.");
    //    }

    //    MyGuard.IsNotNull(nextSlide);
    //    this.SpeakerPic = speakerPicture;
    //    this.islastSlideInSequence = (nextSlide == null);
    //    this.dialogue = dialogue;
    //    this.nextSlide = nextSlide;
    //}

    //public void SetValues(Sprite speakerPicture, string dialogue)
    //{
    //    if (dialogue.Length > MaxChars)
    //    {
    //        throw new ArgumentOutOfRangeException("cant ave more than 120 chars in current font size.");
    //    }

    //    this.SpeakerPic = speakerPicture;
    //    this.dialogue = dialogue;
    //    this.islastSlideInSequence = true;
    //}
}
