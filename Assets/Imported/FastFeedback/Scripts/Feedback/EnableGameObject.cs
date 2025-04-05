using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FastFeedback
{
    /// <summary>
    /// Enable the specified game object, using the specified settings.
    /// </summary>
    [System.Serializable]
    public class EnableGameObject : FeedbackItem
    {
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
            obj.SetActive(true);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw the custom editor UI for the effect.
        /// </summary>
        public override void DrawUI(GameFeedback target)
        {
            base.DrawUI(target);

            // Option for choosing the object to affect.
            targetGameObjectSettings = (GameObjectSettings)EditorGUILayout.EnumPopup("Object to Destroy", targetGameObjectSettings);
            if (RequiresTag(targetGameObjectSettings))
                targetGameObjectTag = EditorGUILayout.TextField(" ", targetGameObjectTag);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Error checking.
            hasError = false;
            if (RequiresTag(targetGameObjectSettings) && targetGameObjectTag.Length == 0)
            {
                EditorGUILayout.HelpBox("You must assign a valid tag for the GameObject to enable.", MessageType.Error);
                hasError = true;
            }
        }

        /// <summary>
        /// Return the custom editor icon for the effect.
        /// </summary>
        public override Texture2D GetIcon()
        {
            return (Texture2D)EditorGUIUtility.Load(@"d_Prefab Icon");
        }

        /// <summary>
        /// Return a short description of the effect.
        /// </summary>
        public override string GetDescription()
        {
            return "Game Object - Enable";
        }
#endif
    }
}
