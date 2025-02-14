using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasDialogue
{
    public string[] GetDialogue();

    public string[][] GetDialogueOptions();  
}
