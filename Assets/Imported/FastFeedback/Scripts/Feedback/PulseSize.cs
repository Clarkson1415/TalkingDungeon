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
    /// Pulse the scale of the object, with the specified settings.
    /// </summary>
    [System.Serializable]
    public class PulseSize : FeedbackItem
    {
        // Default effect settings.
        public float pulseScaleModifier = 1.05f;
        public float pulseTime = 0.1f;
        public PulseDuration pulseDuration = PulseDuration.PulseOnce;
        public float effectDuration;
        public int numberOfPulses;

        public enum PulseDuration
        {
            PulseOnce,
            PulseXTimes,
            PulseForXSeconds
        };

        /// <summary>
        /// Activate the feedback effect, setting up all appropriate actions using the given settings.
        /// </summary>
        public override void Activate(GameObject target = null, GameObject origin = null, Vector3 targetPosition = new Vector3())
        {
            base.Activate(target, origin, targetPosition);
            GameObject obj = GetTargetGameObject(target);
            if (obj.GetComponent<PulseSizeEffect>() == null)
            {
                PulseSizeEffect effect = obj.AddComponent<PulseSizeEffect>();
                effect.Apply(this, target, origin);
            }
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
            targetGameObjectSettings = (GameObjectSettings)EditorGUILayout.EnumPopup("Object to Pulse", targetGameObjectSettings);
            if (RequiresTag(targetGameObjectSettings))
                targetGameObjectTag = EditorGUILayout.TextField(" ", targetGameObjectTag);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing the effect duration.
            pulseTime = EditorGUILayout.FloatField("Effect Duration", pulseTime);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing the scale modifier.
            pulseScaleModifier = EditorGUILayout.FloatField("Pulse Scale Modifier", pulseScaleModifier);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing the duration of a pulse.
            pulseDuration = (PulseDuration)EditorGUILayout.EnumPopup("Pulse Duration", pulseDuration);
            if (pulseDuration == PulseDuration.PulseXTimes)
            {
                numberOfPulses = EditorGUILayout.IntField(" ", numberOfPulses);
            }
            else if (pulseDuration == PulseDuration.PulseForXSeconds)
            {
                effectDuration = EditorGUILayout.FloatField(" ", effectDuration);
            }
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing a pulse curve.
            animationCurveSettings = (AnimationCurveSettings)EditorGUILayout.EnumPopup("Pulse Curve", animationCurveSettings);
            if (animationCurveSettings == AnimationCurveSettings.CustomCurve)
            {
                if (customCurve == null || customCurve.keys.Length == 0) customCurve = new AnimationCurve(FastFeedbackSettings.Current.easeInAndOut.keys);
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

            // Error checking.
            hasError = false;
            if (RequiresTag(targetGameObjectSettings) && targetGameObjectTag.Length == 0)
            {
                hasError = true;
                EditorGUILayout.HelpBox("You must assign a valid GameObject tag.", MessageType.Error);
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Return the custom editor icon for the effect.
        /// </summary>
        public override Texture2D GetIcon()
        {
            return (Texture2D)EditorGUIUtility.Load(@"d_CharacterJoint Icon");
        }

        /// <summary>
        /// Return a short description of the effect.
        /// </summary>
        public override string GetDescription()
        {

            return "Renderer - Pulse Size";
        }
#endif
    }
}