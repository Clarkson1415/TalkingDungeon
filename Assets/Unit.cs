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
        return this.firstDialogueSlide;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
