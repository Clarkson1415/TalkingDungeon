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

    /// <summary>
    /// TODO: change so this is this based on font size setting.
    /// max characters for text box
    /// </summary>
    private static readonly int MaxChars = 212;

    private void Awake()
    {
        if (this.dialogue.Length > MaxChars)
        {
            Debug.LogError($"problem in slide {this.dialogue} too many characters");
        }

        if (dialogueOptions.Count() > 4)
        {
            throw new NotImplementedException($"dialouge text box does not have more than 4 positions!!! check dialogue slide {this.dialogue}");
        }
    }
}
