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
    /// Flash a given object with a color/texture, using the specified settings.
    /// </summary>
    [System.Serializable]
    public class FlashColor : FeedbackItem
    {
        public Color flashColor = Color.white;
        public float flashTime = 0.1f;
        public bool flashUsingEmission = true;

        public FlashDuration flashDuration = FlashDuration.FlashOnce;
        public float effectDuration = 0.1f;
        public int numberOfFlashes = 1;

        public enum FlashDuration
        {
            FlashOnce,
            FlashXTimes,
            FlashForXSeconds
        };

        /// <summary>
        /// Activate the feedback effect, setting up all appropriate actions using the given settings.
        /// </summary>
        public override void Activate(GameObject target = null, GameObject origin = null, Vector3 targetPosition = new Vector3())
        {
            base.Activate(target, origin, targetPosition);

            GameObject obj = GetTargetGameObject(target);

            if (obj != null && obj.GetComponent<FlashTextureEffect>() == null)
            {
                FlashTextureEffect effect = obj.AddComponent<FlashTextureEffect>();
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

            // Option for choosing which object to affect.
            targetGameObjectSettings = (GameObjectSettings)EditorGUILayout.EnumPopup("Object to Flash", targetGameObjectSettings);
            if (RequiresTag(targetGameObjectSettings))
                targetGameObjectTag = EditorGUILayout.TextField(" ", targetGameObjectTag);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing the color of the flash.
            flashColor = EditorGUILayout.ColorField("Flash Color", flashColor);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing the color of the flash.
            flashUsingEmission = EditorGUILayout.Toggle("Flash Using Emission", flashUsingEmission);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing the duration of the flash.
            flashTime = EditorGUILayout.FloatField("Time per Flash", flashTime);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing the duration of the flash.
            flashDuration = (FlashDuration)EditorGUILayout.EnumPopup("Flash Settings", flashDuration);
            if (flashDuration == FlashDuration.FlashXTimes)
            {
                numberOfFlashes = EditorGUILayout.IntField(" ", numberOfFlashes);
            }
            else if (flashDuration == FlashDuration.FlashForXSeconds)
            {
                flashTime = EditorGUILayout.FloatField(" ", flashTime);
            }
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing the curve of the flash.
            animationCurveSettings = (AnimationCurveSettings)EditorGUILayout.EnumPopup("Flash Curve", animationCurveSettings);
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
            if (targetGameObjectSettings == GameObjectSettings.GameObjectWithTag && targetGameObjectTag.Length == 0)
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
            return "Renderer - Flash Color";
        }
#endif
    }
}