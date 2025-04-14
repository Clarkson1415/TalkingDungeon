using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unit class, NOT the player
/// </summary>
public class Unit : MonoBehaviour, IInteracble, IHasDialogue
{
    public float currentHealth = 100;
    public float maxHealth = 100;

    public DialogueSlide firstDialogueSlide;

    public DialogueSlide GetFirstDialogueSlide()
    {
        MyGuard.IsNotNull(this.firstDialogueSlide);
        return this.firstDialogueSlide;
    }
}
