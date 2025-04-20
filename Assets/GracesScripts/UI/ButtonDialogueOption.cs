using Assets.GracesScripts;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
[System.Serializable]

public class DialogueOptionButton : DungeonButton
{
    public DialogueSlide NextDialogueSlide { get; set; }

    public string OptionText { get; set; }

    public void Start()
    {
    }

    public void UpdateButtonText()
    {
        this.GetComponentInChildren<TMP_Text>().text = this.OptionText;
    }

    /// <summary>
    /// Set all values in a dialogue option to the option passed in.
    /// </summary>
    /// <param name="option"></param>
    public void SetValues(string text, DialogueSlide nextSlide)
    {
        this.OptionText = text;
        this.NextDialogueSlide = nextSlide;
    }
}
