using Assets.GracesScripts.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability", order = 1)]
public class Ability : ScriptableObject
{
    public string Name;
    public string description;
    
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

    public void Apply(Unit user, Unit target)
    {
        foreach (var effect in this.Effects)
        {
            effect.ApplyToUnit(user, target, this.Value);
        }
    }
}