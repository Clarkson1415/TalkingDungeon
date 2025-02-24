using Assets.GracesScripts;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
[System.Serializable]

public class DialogueOptionButton : MonoBehaviour
{
    public DialogueSlide NextDialogueSlide { get; set; }
    public string OptionText { get; set; }

    public bool isSelected;

    public void ButtonClicked()
    {
        Log.Print("button clicked");
        isSelected = true;
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

    void Start()
    {
    }

    public DialogueOptionButton(string optionText, DialogueSlide nextslide)
    {
        this.OptionText = optionText;
        this.NextDialogueSlide = nextslide;
    }
}
