using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

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

    // this to go in Unit in a battle
    public GameObject HealthBarObject;
    public Image enemyHealthFill;

    public void SetupUnitForBattle()
    {
        Debug.Log("EVERYTHING BELOW GETFIRSTDIALOGUE SLIDE TO GO IN UNITFORBATTEL.CS an extension of this class for the turn based battle.");
        MyGuard.IsNotNull(enemyHealthFill);
        MyGuard.IsNotNull(HealthBarObject);
        this.enemyHealthFill.fillAmount = this.currentHealth / this.maxHealth;
    }

    /// <summary>
    /// Shake health bar and animate health bar fill.
    /// </summary>
    public void TakeDamage(float damage)
    {
        Debug.Log("take damage is the same in player and unit in battle.cs so should be an interface.");
        this.HealthBarObject.GetComponent<ShakeObject>().StartShake(1f, 5f);
        this.currentHealth -= damage;
        StartCoroutine(AnimateEnemyHealthLoss());
    }

    private IEnumerator AnimateEnemyHealthLoss()
    {
        float damagePerSecond = 2f;

        while (this.enemyHealthFill.fillAmount > (this.currentHealth / this.maxHealth))
        {
            this.enemyHealthFill.fillAmount -= (damagePerSecond / 100);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);
    }
}
