using UnityEngine;
#nullable enable

[CreateAssetMenu(fileName = "DialogueOption", menuName = "ScriptableObjects/Dialogue/DialogueOption", order = 1)]
public class DialogueOption : ScriptableObject
{
    public string optionText;

    public DialogueSlide? nextSlide;
}