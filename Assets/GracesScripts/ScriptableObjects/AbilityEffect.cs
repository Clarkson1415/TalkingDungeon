using UnityEngine;

namespace Assets.GracesScripts.ScriptableObjects
{
    public abstract class AbilityEffect : ScriptableObject
    {
        /// <summary>
        /// Target User if true or targets target by default if false.
        /// </summary>
        public bool TargetUser;

        public GameObject effect;

        public void ApplyToUnit(Unit user, Unit opponent, int value)
        {
            var target = TargetUser ? user : opponent;
            ApplyEffect(target, value);

            if (effect != null)
            {
                Instantiate(effect, target.transform);
            }
            else
            {
                Debug.Log($"Ability Effect has no effect prefab {this.name}");
            }
        }

        protected abstract void ApplyEffect(Unit target, int value);
    }
}
