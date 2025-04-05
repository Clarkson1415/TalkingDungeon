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
    /// Destroy the specified game object, using the specified settings.
    /// </summary>
    [System.Serializable]
    public class DestroyGameObject : FeedbackItem
    {
        public DestroySettings destroySettings;
        public float destroyAfter;

        public enum DestroySettings
        {
            DestroyImmediately,
            DestroyAfterXSeconds
        };

        /// <summary>
        /// Activate the feedback effect, setting up all appropriate actions using the given settings.
        /// </summary>
        public override void Activate(GameObject target = null, GameObject origin = null, Vector3 targetPosition = new Vector3())
        {
            base.Activate(target, origin, targetPosition);

            // Guard clauses.
            GameObject obj = GetTargetGameObject(target);
            if (obj == null) return;

            // Feedback actions.
            if (destroySettings == DestroySettings.DestroyImmediately)
                GameObject.Destroy(obj);
            else
                GameObject.Destroy(obj, destroyAfter);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw the custom editor UI for the effect.
        /// </summary>
        public override void DrawUI(GameFeedback target)
        {
            base.DrawUI(target);

            EditorGUILayout.BeginVertical();

            // Option for choosing the object to affect.
            targetGameObjectSettings = (GameObjectSettings)EditorGUILayout.EnumPopup("Object to Destroy", targetGameObjectSettings);
            if (RequiresTag(targetGameObjectSettings))
                targetGameObjectTag = EditorGUILayout.TextField(" ", targetGameObjectTag);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Options for the destroy wait time (if applicable).
            destroySettings = (DestroySettings)EditorGUILayout.EnumPopup("Destroy Settings", destroySettings);
            if (destroySettings == DestroySettings.DestroyAfterXSeconds)
            {
                destroyAfter = EditorGUILayout.FloatField(" ", destroyAfter);
            }
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Error checking.
            hasError = false;
            if (RequiresTag(targetGameObjectSettings) && targetGameObjectTag.Length == 0)
            {
                EditorGUILayout.HelpBox("You must assign a valid tag for the GameObject to destroy.", MessageType.Error);
                hasError = true;
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Return the custom editor icon for the effect.
        /// </summary>
        public override string GetDescription()
        {
            return "Game Object - Destroy";
        }

        /// <summary>
        /// Return a short description of the effect.
        /// </summary>
        public override Texture2D GetIcon()
        {
            return (Texture2D)EditorGUIUtility.Load(@"d_Prefab Icon");
        }
#endif
    }
}
