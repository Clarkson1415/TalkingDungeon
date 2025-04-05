using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FastFeedback
{
    /// <summary>
    /// This effect allows the user to apply vibrations to their XR controllers,
    /// with a given strength and duration. Vibrations can be applied to both
    /// controllers seperately.
    /// </summary>
    [System.Serializable]
    public class VibrateController : FeedbackItem
    {
        public VibrationSettings vibrationSettings = VibrationSettings.BothControllers;
        public float vibrationStrength = 1;
        public float vibrationDuration = 1;

        public enum VibrationSettings
        {
            LeftController,
            RightController,
            BothControllers
        };

        /// <summary>
        /// Activate the feedback effect, setting up all appropriate actions using the given settings.
        /// </summary>
        public override void Activate(GameObject target = null, GameObject origin = null, Vector3 targetPosition = new Vector3())
        {
            base.Activate(target, origin, targetPosition);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw the custom editor UI for the effect.
        /// </summary>
        public override void DrawUI(GameFeedback target)
        {
            base.DrawUI(target);

            EditorGUILayout.BeginVertical();

            // Option for specifying the controller(s) to vibrate.
            vibrationSettings = (VibrationSettings)EditorGUILayout.EnumPopup("Controller to Vibrate", vibrationSettings);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for specifying the strength of the vibration.
            vibrationStrength = EditorGUILayout.Slider("Vibration Strength", vibrationStrength, 0.0f, 1.0f);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for specifying the duration of the vibration.
            vibrationDuration = EditorGUILayout.Slider("Vibration Duration", vibrationDuration, 0.0f, 2.0f);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Handle all error checking.
            EditorGUILayout.HelpBox("This effect will only work in XR projects.", MessageType.Error);

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Return a short description of the effect.
        /// </summary>
        public override string GetDescription()
        {
            return "Haptics - Vibrate Controller";
        }

        /// <summary>
        /// Return the custom editor icon for the effect.
        /// </summary>
        public override Texture2D GetIcon()
        {
            return (Texture2D)EditorGUIUtility.Load(@"d_RelativeJoint2D Icon");
        }
#endif
    }
}