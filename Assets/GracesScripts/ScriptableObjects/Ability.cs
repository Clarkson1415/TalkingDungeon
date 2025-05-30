using Assets.GracesScripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability", order = 1)]
public class Ability : ScriptableObject
{
    public string Name;
    [HideInInspector] public string Description => FormatDescription();
    [SerializeField] string _desc;

    private string FormatDescription()
    {
        var player = FindObjectOfType<PlayerDungeon>();
        MyGuard.IsNotNull("Player not found.");
        if (!this._desc.Contains("{0}"))
        {
            throw new ArgumentException($"Does not have {{0}} in descriptino string on Ability: {Name} in inspector please add so correct damage is displayed.");
        }

        if (string.IsNullOrEmpty(_desc))
        {
            throw new ArgumentException($"Ability Description string on {Name} is empty");
        }

        return string.Format(this._desc, this.Value * player.equippedWeapon.PowerStat);
    }
    
    /// <summary>
    /// Ability damage. multiplied by the weapons power.
    /// </summary>
    public int Value;

    /// <summary>
    /// Turns have to wait until can use again.
    /// </summary>
    public int cooldown = 0;

    /// <summary>
    /// todo: resize to fit the menu screen of objects and do this for profile pic too
    /// </summary>
    public Sprite image;

    public List<AbilityEffect> Effects;

    public void Apply(int weaponUsedPower, Unit user, Unit target)
    {
        foreach (var effect in this.Effects)
        {
            effect.ApplyToUnit(user, target, this.Value * weaponUsedPower);
        }
    }
}