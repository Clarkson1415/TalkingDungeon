using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class DialogueOption : MonoBehaviour
{
    public DialogueSlide nextDialogueSlide { get; private set; }
    public string optionText { get; private set; }

    public bool isSelected;

    public void ButtonClicked()
    {
        Debug.Log("buttoncliecked");
        isSelected = true;
    }

    /// <summary>
    /// Set all values in a dialogue option to the option passed in.
    /// </summary>
    /// <param name="option"></param>
    public void SetValues(DialogueOption option)
    {
        this.optionText = option.optionText;
        this.nextDialogueSlide = option.nextDialogueSlide;
        this.isSelected = option.isSelected;
    }

    void Start()
    {
    }

    public DialogueOption(string optionText, DialogueSlide nextslide)
    {
        this.optionText = optionText;
        this.nextDialogueSlide = nextslide;
    }
}
