using TMPro;
[System.Serializable]

public class DialogueOptionButton : DungeonButton
{
    public DialogueSlide NextDialogueSlide { get; set; }

    public string OptionText { get; set; }

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
