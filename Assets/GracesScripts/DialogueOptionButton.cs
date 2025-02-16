using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
[System.Serializable]

public class DialogueOptionButton : MonoBehaviour
{
    public OLDDialogueSlide nextDialogueSlide { get; set; }
    public string optionText { get; set; }

    public bool isSelected;

    public void ButtonClicked()
    {
        Debug.Log("button clicked");
        isSelected = true;
    }

    /// <summary>
    /// Set all values in a dialogue option to the option passed in.
    /// </summary>
    /// <param name="option"></param>
    public void SetValues(DialogueOptionButton option)
    {
        this.optionText = option.optionText;
        this.nextDialogueSlide = option.nextDialogueSlide;
        this.isSelected = option.isSelected;
    }

    public void SetValues(string optionText, OLDDialogueSlide nextDSlide, bool isSelected = false)
    {
        this.optionText = optionText;
        this.nextDialogueSlide = nextDSlide;
        this.isSelected = isSelected;
    }

    void Start()
    {
    }

    public DialogueOptionButton(string optionText, OLDDialogueSlide nextslide)
    {
        this.optionText = optionText;
        this.nextDialogueSlide = nextslide;
    }
}
