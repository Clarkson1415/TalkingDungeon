using UnityEngine;

namespace Assets.GracesScripts.ScriptableObjects
{
    public abstract class AbilityEffect : ScriptableObject
    {
        public string Description;
        public abstract void Apply(Unit user, Unit target);
    }
}
