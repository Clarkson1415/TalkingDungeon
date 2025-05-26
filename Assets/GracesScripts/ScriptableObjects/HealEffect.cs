using UnityEngine;

namespace Assets.GracesScripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Ability Effects/HealEffect")]
    public class HealEffect : AbilityEffect
    {
        public int HealAmount;
        public override void Apply(Unit user, Unit target)
        {
            Debug.Log($"Healing {HealAmount} HP.");
            // target.GetComponent<Health>().Heal(HealAmount);
        }
    }
}
