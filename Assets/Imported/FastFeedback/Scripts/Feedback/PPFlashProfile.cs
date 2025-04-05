#if UNITY_POST_PROCESSING_STACK_V2
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FastFeedback
{
    /// <summary>
    /// Flash the given post processing profile, using the specified settings.
    /// </summary>
    [System.Serializable]
    public class PPFlashProfile : FeedbackItem
    {
        // Default settings.
        public PostProcessProfile postProcessProfile;
        public float timeToFlash = 0.1f;
        public float flashStrength = 1.0f;

        /// <summary>
        /// Activate the feedback effect, setting up all appropriate actions using the given settings.
        /// </summary>
        public override void Activate(GameObject target = null, GameObject origin = null, Vector3 targetPosition = new Vector3())
        {
            base.Activate(target, origin, targetPosition);

            // Guard clauses.
            if (postProcessProfile == null) return;

            // Feedback effects.
            PPFlashEffect[] effects = GameObject.FindObjectsOfType<PPFlashEffect>();
            foreach (PPFlashEffect effect in effects)
            {
                if (effect.profile == postProcessProfile)
                {
                    if (!effect.inUse)
                    {
                        effect.Apply(this, target, origin);
                    }
                    return;
                }
            }
            GameObject obj = GameObject.Instantiate(Resources.LoadAll<FastFeedbackSettings>("")[0].PPFlashPrefab);
            obj.GetComponent<PPFlashEffect>().Apply(this, target, origin);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw the custom editor UI for the effect.
        /// </summary>
        public override void DrawUI(GameFeedback target)
        {
            base.DrawUI(target);

            
            postProcessProfile = (PostProcessProfile)EditorGUILayout.ObjectField("Profile to Flash", postProcessProfile, typeof(PostProcessProfile), true);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            timeToFlash = EditorGUILayout.FloatField("Flash Time", timeToFlash);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            flashStrength = EditorGUILayout.Slider("Flash Strength", flashStrength, 0.0f, 1.0f);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing the curve of the flash.
            animationCurveSettings = (AnimationCurveSettings)EditorGUILayout.EnumPopup("Flash Curve", animationCurveSettings);
            if (animationCurveSettings == AnimationCurveSettings.CustomCurve)
            {
                if (customCurve == null) customCurve = new AnimationCurve(FastFeedbackSettings.Current.easeInAndOut.keys);
                customCurve = EditorGUILayout.CurveField(" ", customCurve);
            }
            else
            {
                GUI.enabled = false;
                AnimationCurve temp = GetAnimationCurve();
                EditorGUILayout.CurveField(" ", temp);
                GUI.enabled = true;
            }
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);
        }

        /// <summary>
        /// Return the custom editor icon for the effect.
        /// </summary>
        public override Texture2D GetIcon()
        {
            return (Texture2D)EditorGUIUtility.Load(@"Packages/com.unity.postprocessing/PostProcessing/Gizmos/PostProcessLayer.png");
        }

        /// <summary>
        /// Return a short description of the effect.
        /// </summary>
        public override string GetDescription()
        {
            return "Post Processing - Flash Custom Profile";
        }
#endif
    }
}
#endif