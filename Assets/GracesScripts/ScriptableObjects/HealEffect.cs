using UnityEngine;

namespace Assets.GracesScripts.ScriptableObjects
{
    /// <summary>
    /// Represents an ability effect that heals the user.
    /// </summary>
    [CreateAssetMenu(menuName = "Ability Effects/Heal Effect")]
    public class HealEffect : AbilityEffect
    {
        protected override void ApplyEffect(DungeonUnit target, int value)
        {
            target.Heal(value);
        }
    }
}
