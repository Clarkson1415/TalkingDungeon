using UnityEngine;

namespace Assets.GracesScripts.ScriptableObjects
{
    /// <summary>
    /// Represents an ability effect that damages the target.
    /// </summary>
    [CreateAssetMenu(menuName = "Ability Effects/Damage Effect")]
    public class DamageEffect : AbilityEffect
    {
        protected override void ApplyEffect(DungeonUnit target, int value)
        {
            Debug.Log("Damage effect applying animation and damage.");
            Debug.Log("TODO white flashing of the sprite when he takes damage.");
            target.TakeDamage(value);
        }
    }
}
