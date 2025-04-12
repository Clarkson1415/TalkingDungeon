using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.Rendering;
using UnityEngine;
#nullable enable

[CreateAssetMenu(fileName = "DialogueOption", menuName = "ScriptableObjects/Dialogue/DialogueOption", order = 1)]
public class DialogueOption : ScriptableObject
{
    public string optionText;

    public DialogueSlide? nextSlide;
}