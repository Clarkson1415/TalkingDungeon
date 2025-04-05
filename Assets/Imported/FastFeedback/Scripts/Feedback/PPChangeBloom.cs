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
    /// Probably going to be removed???
    /// </summary>
    [System.Serializable]
    public class PPChangeBloom : FeedbackItem
    {
        public Camera cameraToShake;
        public float shakeAmount = 0.1f;
        public float shakeDecay = 0.5f;

        public Color colorToFlash = Color.red;
        public Texture2D spriteToFlash;
        public float timeToFlash = 0.1f;
        public AnimationCurve flashAnimationCurve = null;

        public PostProcessVolume postProcessVolume;

        public CameraSettings cameraSettings = CameraSettings.MainCamera;
        public enum CameraSettings
        {
            MainCamera,
            CustomCamera
        };

        /// <summary>
        /// Activate the feedback effect, setting up all appropriate actions using the given settings.
        /// </summary>
        public override void Activate(GameObject target = null, GameObject origin = null, Vector3 targetPosition = new Vector3())
        {
            base.Activate(target, origin, targetPosition);
            GameObject obj = GameObject.Instantiate(Resources.LoadAll<FastFeedbackSettings>("")[0].screenFlashPrefab);
            obj.GetComponent<ScreenFlashEffect>().Apply(this, target, origin);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw the custom editor UI for the effect.
        /// </summary>
        public override void DrawUI(GameFeedback target)
        {
            base.DrawUI(target);

            EditorGUILayout.BeginVertical();

            /*
            colorToFlash = EditorGUILayout.ColorField("Flash Color", colorToFlash);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);
            postProcessVolume = (PostProcessVolume)EditorGUILayout.ObjectField("Flash Texture", postProcessVolume, typeof(PostProcessVolume), true);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);
            timeToFlash = EditorGUILayout.FloatField("Flash Time", timeToFlash);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);
            if (flashAnimationCurve == null) flashAnimationCurve = new AnimationCurve(FastFeedbackSettings.Current.easeInAndOut.keys);

            flashAnimationCurve = EditorGUILayout.CurveField("Alpha Curve", flashAnimationCurve);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);
            */

            EditorGUILayout.EndVertical();
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
            return "Post Processing - Change Bloom";
        }
#endif
    }
}
#endif