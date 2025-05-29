using Assets.GracesScripts;
using Assets.GracesScripts.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI;
#nullable enable

/// <summary>
/// Unit class, Player and NPCs both have abilities an equipped weapon and item and health.
/// </summary>
public abstract class Unit : MonoBehaviour
{
    public float currentHealth = 100;
    public float maxHealth = 100;
    protected Weapon DefaultWeaponHands => GetDefaultHands();
    public List<Ability> Abilities => this.equippedWeapon == null ? DefaultWeaponHands.Abilities : this.equippedWeapon.Abilities;
    protected List<DungeonItem?> EquippedItems => new() { this.equippedWeapon, this.equippedSpecialItem };
    public Weapon equippedWeapon;
    public SpecialItem? equippedSpecialItem;
    public List<DungeonItem> Inventory = new();
    // this to go in Unit in a battle
    [HideInInspector] public GameObject HealthBarObject;
    [HideInInspector] public Image healthBarFill;

    
    [SerializeField] private float basePower = 0;
    public float powerModifier;
    public float Power => powerModifier * (this.basePower + this.equippedWeapon.PowerStat);

    [SerializeField] private float baseDefence = 0;

    /// <summary>
    /// Will be changed from one off items to buffing abilities
    /// </summary>
    public float defenceModifier = 0;
    public float Defence => defenceModifier * (this.equippedWeapon.DefenceStat + baseDefence);

    private Weapon GetDefaultHands()
    {
        var hands = Resources.Load<Weapon>("Items/Weapon/Hands");
        MyGuard.IsNotNull(hands);
        return hands;
    }

    public void SetupUnitForBattle()
    {
        Debug.Log("EVERYTHING BELOW GETFIRSTDIALOGUE SLIDE TO GO IN UNITFORBATTEL.CS an extension of this class for the turn based battle. NAH maybe not? thogh animate enemy health in here is conveintint because dont have to setup UI on the new object when battle starts");
        MyGuard.IsNotNull(this.healthBarFill);
        MyGuard.IsNotNull(this.HealthBarObject);
        this.healthBarFill.fillAmount = this.currentHealth / this.maxHealth;
    }

    /// <summary>
    /// Shake health bar and animate health bar fill.
    /// </summary>
    public void TakeDamage(float value)
    {
        Debug.Log("Check how the timing of StartShake and damage bar reducing works i dont remember but rewrite it so its clearly aligned.");
        var objectToShake = this.HealthBarObject.GetComponent<ShakeObject>();
        objectToShake.StartShake(1f, 5f);
        this.currentHealth -= value;
        // if current damage will kill Unit make it go down faster
        if (this.currentHealth <= 0)
        {
            StartCoroutine(AnimateHealthBarFill(0.1f, -value));
            this.Die();
        }
        else
        {
            StartCoroutine(AnimateHealthBarFill(0.5f, -value));
        }
    }

    /// <summary>
    /// Add health to this unit and animate health bar fill increasing.
    /// </summary>
    /// <param name="healAmount"></param>
    public void Heal(float healAmount)
    {
        Debug.Log("Heal play heal animation");
        this.currentHealth += healAmount;
        StartCoroutine(AnimateHealthBarFill(0.5f, healAmount));
    }

    protected abstract void Die();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animationLength">seconds time for health bar to go to where after damage puts it</param>
    /// <param name="healthChange">positive value for healing negative for damaging.</param>
    /// <returns></returns>
    private IEnumerator AnimateHealthBarFill(float animationLength, float healthChange)
    {
        var timeIncrement = 0.1f;
        var changePerTimeIncrement = healthChange / (animationLength / timeIncrement);
        while (this.healthBarFill.fillAmount > Mathf.Clamp((this.currentHealth / this.maxHealth), 0, 1))
        {
            this.healthBarFill.fillAmount += (changePerTimeIncrement / 100);
            yield return new WaitForSeconds(timeIncrement);
        }
    }
}
