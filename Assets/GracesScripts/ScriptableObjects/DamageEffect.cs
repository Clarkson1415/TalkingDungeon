using UnityEngine;

namespace Assets.GracesScripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Ability Effects/DamageEffect")]
    public class DamageEffect : AbilityEffect
    {
        public int Damage;
        public override void Apply(Unit user, Unit target)
        {
            Debug.Log($"{target.name} Dealing {Damage} damage. to {user.name}");
            // target.GetComponent<Health>().TakeDamage(Damage);
            target.TakeDamage(this.Damage);
        }
    }
}
