using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FastFeedback
{
    /// <summary>
    /// Pulse the scale of the object, with the specified settings.
    /// </summary>
    [System.Serializable]
    public class ScaleTime : FeedbackItem
    {
        // Default effect settings.
        public float scaleTimeDuration = 0.5f;
        public float timeScaleModifier = 1.0f;

        /// <summary>
        /// Activate the feedback effect, setting up all appropriate actions using the given settings.
        /// </summary>
        public override void Activate(GameObject target = null, GameObject origin = null, Vector3 targetPosition = new Vector3())
        {
            base.Activate(target, origin, targetPosition);
            if (ScaleTimeEffect.Instance == null)
            {
                GameObject obj = GameObject.Instantiate(Resources.LoadAll<FastFeedbackSettings>("")[0].timeScalerPrefab);
                obj.GetComponent<ScaleTimeEffect>().Apply(this);
            }
            else
            {
                ScaleTimeEffect.Instance.Apply(this);
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

            // Option for choosing the effect duration.
            scaleTimeDuration = EditorGUILayout.FloatField("Effect Duration", scaleTimeDuration);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing the scale modifier.
            timeScaleModifier = EditorGUILayout.FloatField("Time Scale Modifier", timeScaleModifier);
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

            return "Time - Change Timescale";
        }
#endif
    }
}
