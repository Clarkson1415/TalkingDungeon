using Assets.GracesScripts;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
[System.Serializable]

public class DialogueOptionButton : Button
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
    public void SetValues(DialogueOptionButton option)
    {
        this.OptionText = option.OptionText;
        this.NextDialogueSlide = option.NextDialogueSlide;
        this.isSelected = option.isSelected;
    }

    public void SetValues(string optionText, DialogueSlide nextDSlide, bool isSelected = false)
    {
        this.OptionText = optionText;
        this.NextDialogueSlide = nextDSlide;
        this.isSelected = isSelected;
    }

    public DialogueOptionButton(string optionText, DialogueSlide nextslide)
    {
        this.OptionText = optionText;
        this.NextDialogueSlide = nextslide;
    }
}
