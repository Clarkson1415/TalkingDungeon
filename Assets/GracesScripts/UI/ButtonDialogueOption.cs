using TMPro;
using UnityEngine;
[System.Serializable]

public class DialogueOptionButton : DungeonButton
{
    public DialogueSlide NextDialogueSlide { get; set; }

    public string OptionText { get; set; }
    [SerializeField] TMP_Text textObj;

    public void UpdateButtonText()
    {
        textObj.text = this.OptionText;
    }

    /// <summary>
    /// Set all values in a dialogue option to the option passed in.
    /// </summary>
    /// <param name="option"></param>
    public void SetValues(string text, DialogueSlide nextSlide)
    {
        this.OptionText = text;
        UpdateButtonText();
        this.NextDialogueSlide = nextSlide;
    }
}
