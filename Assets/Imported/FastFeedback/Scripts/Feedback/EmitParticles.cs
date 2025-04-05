using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FastFeedback
{
    /// <summary>
    /// Emit particles from the specified particle system, using the specified settings.
    /// </summary>
    [System.Serializable]
    public class EmitParticles : FeedbackItem
    {
        public ParticleSystem particleSystem;
        public int particleCount;

        /// <summary>
        /// Activate the feedback effect, setting up all appropriate actions using the given settings.
        /// </summary>
        public override void Activate(GameObject target = null, GameObject origin = null, Vector3 targetPosition = new Vector3())
        {
            base.Activate(target, origin, targetPosition);

            // Guard clauses.
            GameObject obj = GetTargetGameObject(target);
            if (obj == null) return;
            ParticleSystem particleSystem = obj.GetComponentInChildren<ParticleSystem>();
            if (particleSystem == null) return;
            
            // Feedback actions.
            particleSystem.Emit(particleCount);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw the custom editor UI for the effect.
        /// </summary>
        public override void DrawUI(GameFeedback target)
        {
            base.DrawUI(target);

            // Option for choosing the object to affect.
            targetGameObjectSettings = (GameObjectSettings)EditorGUILayout.EnumPopup("Particle System On", targetGameObjectSettings);
            if (targetGameObjectSettings == GameObjectSettings.GameObjectWithTag)
                targetGameObjectTag = EditorGUILayout.TextField(" ", targetGameObjectTag);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Options for specifying how many particles to emit.
            particleCount = EditorGUILayout.IntField("Particle Count", particleCount);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Do all error checking here!
            hasError = false;
            if (RequiresTag(targetGameObjectSettings) && targetGameObjectTag.Length == 0)
            {
                EditorGUILayout.HelpBox("You must assign a valid game object tag.", MessageType.Error);
                hasError = true;
            }
        }

        /// <summary>
        /// Return the custom editor icon for the effect.
        /// </summary>
        public override Texture2D GetIcon()
        {
            return (Texture2D)EditorGUIUtility.Load(@"d_ParticleSystem Icon");
        }

        /// <summary>
        /// Return a short description of the effect.
        /// </summary>
        public override string GetDescription()
        {
            return "Particle System - Emit Particles";
        }
#endif
    }
}