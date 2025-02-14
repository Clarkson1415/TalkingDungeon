using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DialogueOption
{
    public DialogueSlide nextDialogueSlide { get; private set; }
    public string optionText { get; private set; }

    public bool isSelected;

    public void ButtonClicked()
    {
        isSelected = true;
    }

    public DialogueOption(string optionText, DialogueSlide nextslide)
    {
        this.nextDialogueSlide = nextslide;
    }
}
