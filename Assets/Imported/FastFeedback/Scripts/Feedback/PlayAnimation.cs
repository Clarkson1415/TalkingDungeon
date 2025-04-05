using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FastFeedback
{
    /// <summary>
    /// Play the specified animation on the specified animator, using the specified settings.
    /// </summary>
    [System.Serializable]
    public class PlayAnimation : FeedbackItem
    {
        public string animation = "";

        /// <summary>
        /// Activate the feedback effect, setting up all appropriate actions using the given settings.
        /// </summary>
        public override void Activate(GameObject target = null, GameObject origin = null, Vector3 targetPosition = new Vector3())
        {
            base.Activate(target, origin, targetPosition);

            // Guard clauses.
            if (animation.Length == 0) return;
            GameObject obj = GetTargetGameObject(target);
            if (obj == null) return;
            Animator animator = obj.GetComponentInChildren<Animator>();
            if (animator == null) return;

            // Feedback actions.
            animator.Play(animation);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw the custom editor UI for the effect.
        /// </summary>
        public override void DrawUI(GameFeedback target)
        {
            base.DrawUI(target);

            // Option for choosing the object to affect.
            targetGameObjectSettings = (GameObjectSettings)EditorGUILayout.EnumPopup("Animator On", targetGameObjectSettings);
            if (RequiresTag(targetGameObjectSettings))
                targetGameObjectTag = EditorGUILayout.TextField(" ", targetGameObjectTag);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing the name of the animation to play.
            animation = EditorGUILayout.TextField("Animation Name", animation);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Handle error logic.
            hasError = false;
            if (targetGameObjectSettings == GameObjectSettings.GameObjectWithTag && targetGameObjectTag.Length == 0)
            {
                EditorGUILayout.HelpBox("You must assign a valid game object tag.", MessageType.Error);
                hasError = true;
            }
            if (animation.Length == 0)
            {
                EditorGUILayout.HelpBox("You must specify the animation name.", MessageType.Error);
                hasError = true;
            }
        }

        /// <summary>
        /// Return the custom editor icon for the effect.
        /// </summary>
        public override Texture2D GetIcon()
        {
            return (Texture2D)EditorGUIUtility.Load(@"d_AnimationClip Icon");
        }

        /// <summary>
        /// Return a short description of the effect.
        /// </summary>
        public override string GetDescription()
        {
            return "Animation - Play";
        }
#endif
    }
}