using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Unit class, NOT the player
/// </summary>
public class Unit : MonoBehaviour, IInteracble, IHasDialogue
{
    public string unitName;
    public GameObject prefabToUseInBattle;
    public float currentHealth = 100;
    public float maxHealth = 100;

    /// <summary>
    /// Used for not in battle scene.
    /// </summary>
    public DialogueSlide firstDialogueSlide;

    /// <summary>
    /// For talking in battle Scene TODO not setup yet
    /// </summary>
    public DialogueSlide battleSceneDialogueSlide;

    public List<Ability> abilities;

    private void Awake()
    {
        if(this.unitName == null)
        {
            Debug.LogError($"this guy {this.gameObject.name} cannot have no Unitname on Unit.cs");
        }

        if (this.abilities.Count == 0)
        {
            Debug.LogError($"this guy: {this.gameObject.name} cannot have no abilities at least have default Push ability assign in inspector");
        }
    }

    public DialogueSlide GetFirstDialogueSlide()
    {
        MyGuard.IsNotNull(this.firstDialogueSlide);
        return this.firstDialogueSlide;
    }
}
