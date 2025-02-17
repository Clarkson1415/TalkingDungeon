using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpeakingNPC : NPC, IHasDialogue
{
    public DialogueSlide dialogueSlide;

    public DialogueSlide GetFirstDialogueSlide()
    {
        return dialogueSlide;
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
