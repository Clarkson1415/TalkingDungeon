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
    /// Flash the screen with the specified color/texture for the given amount of time.
    /// </summary>
    [System.Serializable]
    public class ScreenFlash : FeedbackItem
    {
        public Color colorToFlash = Color.red;
        public Texture2D spriteToFlash;
        public float timeToFlash = 0.1f;

        public CameraSettings cameraSettings = CameraSettings.MainCamera;
        public Camera customCamera;
        public string cameraTag = "";

        public enum CameraSettings
        {
            MainCamera,
            CustomCamera,
            CameraWithTag
        };

        /// <summary>
        /// Activate the feedback effect, setting up all appropriate actions using the given settings.
        /// </summary>
        public override void Activate(GameObject target = null, GameObject origin = null, Vector3 targetPosition = new Vector3())
        {
            base.Activate(target, origin, targetPosition);

            // Guard clauses.
            if (cameraSettings == CameraSettings.MainCamera)
            {
                customCamera = Camera.main;
            }
            else if (cameraSettings == CameraSettings.CameraWithTag)
            {
                GameObject cameraObj = GameObject.FindGameObjectWithTag(cameraTag);
                if (cameraObj == null) return;
                customCamera = cameraObj.GetComponentInChildren<Camera>();
            }
            if (customCamera == null) return;

            // Feedback effects.
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

            // Choose the camera to affect.
            cameraSettings = (CameraSettings)EditorGUILayout.EnumPopup("Effect Camera", cameraSettings);
            if (cameraSettings == CameraSettings.CustomCamera)
            {
                customCamera = (Camera)EditorGUILayout.ObjectField(" ", customCamera, typeof(Camera), true);
            }
            else if (cameraSettings == CameraSettings.CameraWithTag)
            {
                cameraTag = EditorGUILayout.TextField(" ", cameraTag);
            }
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing the color of the flash.
            colorToFlash = EditorGUILayout.ColorField("Flash Color", colorToFlash);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing the texture of the flash.
            spriteToFlash = (Texture2D)EditorGUILayout.ObjectField("Flash Texture", spriteToFlash, typeof(Texture2D), true, GUILayout.Height(16));
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing the duration of the flash.
            timeToFlash = EditorGUILayout.FloatField("Flash Time", timeToFlash);
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

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Return the custom editor icon for the effect.
        /// </summary>
        public override Texture2D GetIcon()
        {
            return (Texture2D)EditorGUIUtility.Load(@"d_Camera Icon");
        }

        /// <summary>
        /// Return a short description of the effect.
        /// </summary>
        public override string GetDescription()
        {
            return "Camera - Flash Color";
        }
#endif
    }
}