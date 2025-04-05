using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FastFeedback
{
    /// <summary>
    /// Play the specified video, using the specified settings.
    /// </summary>
    [System.Serializable]
    public class StopVideo : FeedbackItem
    {
        public StopMode stopMode;

        public enum StopMode
        {
            KeepPlace,
            ResetToStart,
            KeepPlaceAndHideVideo,
            ResetToStartAndHideVideo
        }

        /// <summary>
        /// Activate the feedback effect, setting up all appropriate actions using the given settings.
        /// </summary>
        public override void Activate(GameObject target = null, GameObject origin = null, Vector3 targetPosition = new Vector3())
        {
            base.Activate(target, origin, targetPosition);

            // Guard clauses.
            GameObject obj = GetTargetGameObject(target);
            if (obj == null) return;
            VideoPlayer video = obj.GetComponent<VideoPlayer>();
            if (video == null) return;

            // Feedback effects.
            VideoControlsEffect effect = obj.GetComponent<VideoControlsEffect>();
            if (effect == null) effect = obj.AddComponent<VideoControlsEffect>();
            effect.Apply(this, target, origin);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw the custom editor UI for the effect.
        /// </summary>
        public override void DrawUI(GameFeedback target)
        {
            base.DrawUI(target);

            // Option for choosing which object to affect.
            targetGameObjectSettings = (GameObjectSettings)EditorGUILayout.EnumPopup("Video On", targetGameObjectSettings);
            if (targetGameObjectSettings == GameObjectSettings.GameObjectWithTag ||
                targetGameObjectSettings == GameObjectSettings.ChildOfTargetWithTag ||
                targetGameObjectSettings == GameObjectSettings.ClosestGameObjectWithTag)
                targetGameObjectTag = EditorGUILayout.TextField(" ", targetGameObjectTag);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing which object to affect.
            stopMode = (StopMode)EditorGUILayout.EnumPopup("Stop Mode", stopMode);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Error checking.
            hasError = false;
            if (targetGameObjectTag.Length == 0 && (
                targetGameObjectSettings == GameObjectSettings.GameObjectWithTag &&
                targetGameObjectSettings == GameObjectSettings.ClosestGameObjectWithTag &&
                targetGameObjectSettings == GameObjectSettings.ChildOfTargetWithTag))
            {
                hasError = true;
                EditorGUILayout.HelpBox("You must assign a valid GameObject tag.", MessageType.Error);
            }
        }

        /// <summary>
        /// Return a short description of the effect.
        /// </summary>
        public override string GetDescription()
        {
            return "Video - Stop Video";
        }

        /// <summary>
        /// Return the custom editor icon for the effect.
        /// </summary>
        public override Texture2D GetIcon()
        {
            return (Texture2D)EditorGUIUtility.Load(@"d_Camera Icon");
        }
#endif
    }
}