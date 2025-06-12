using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Assets.GracesScripts.ScriptableObjects
{
    public abstract class AbilityEffect : ScriptableObject
    {
        /// <summary>
        /// Target User if true or targets target by default if false.
        /// </summary>
        public bool TargetUser;

        public GameObject effect;

        /// <summary>
        /// If effect is for example a flame needs to be flipped. Default direction position is it being instantiated on the right hand side enemy.
        /// </summary>
        public bool ShouldBeFlippedWhenPlayerIsTarget;

        public void ApplyToUnit(DungeonUnit user, DungeonUnit opponent, int value)
        {
            var target = TargetUser ? user : opponent;
            ApplyEffect(target, value);

            // if target is on the left the object needs to be flipped. multiple x scale by -1
            // by default the fire blast spawns to the left of the character so if right hand side enemy uses on player needs to be flipped.
            // if opponent x is less than users than opponent is on the left. and the enemy on the right is casting on the player.
            var TargetIsPlayer = opponent.gameObject.transform.position.x < user.gameObject.transform.position.x;
            
            // so if needs to be flipped 
            // new z rotation = current - 180 degrees
            // new x position = old x * -1

            if (effect != null)
            {
                var effectInScene = Instantiate(effect, target.transform);

                if (ShouldBeFlippedWhenPlayerIsTarget && TargetIsPlayer)
                {
                    // Rotate by -180 degrees around Z axis
                    effectInScene.transform.rotation = Quaternion.Euler(0, 0, effectInScene.transform.eulerAngles.z - 180f);

                    // Multiply x position by -1
                    Vector3 pos = effectInScene.transform.localPosition;
                    pos.x *= -1;
                    effectInScene.transform.localPosition = pos;
                }
            }
            else
            {
                Debug.Log($"Ability Effect has no effect prefab {this.name}");
            }
        }

        protected abstract void ApplyEffect(DungeonUnit target, int value);
    }
}
