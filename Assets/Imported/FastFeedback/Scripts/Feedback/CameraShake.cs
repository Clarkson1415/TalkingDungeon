using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FastFeedback
{
    /// <summary>
    /// Perform a screen shake on the specified camera, with the given duration and intensity.
    /// </summary>
    [System.Serializable]
    public class CameraShake : FeedbackItem
    {
        public Camera cameraToShake;
        public float shakeAmount = 0.1f;
        public float shakeDecay = 0.5f;
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

            Camera camToShake = null;
            if (cameraSettings == CameraSettings.MainCamera)
                camToShake = Camera.main;
            else if (cameraSettings == CameraSettings.CustomCamera)
                camToShake = cameraToShake;

            if (camToShake == null) return;

            if (camToShake.GetComponent<ScreenShakeEffect>() == null)
            {
                ScreenShakeEffect effect = camToShake.gameObject.AddComponent<ScreenShakeEffect>();
                effect.shakeStrength = shakeAmount;
            }
            else
            {
                camToShake.GetComponent<ScreenShakeEffect>().shakeStrength += shakeAmount;
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

            // Option for specifying which camera should be shaken.
            cameraSettings = (CameraSettings)EditorGUILayout.EnumPopup("Camera to Shake", cameraSettings);
            if (cameraSettings == CameraSettings.CustomCamera)
            {
                cameraToShake = (Camera)EditorGUILayout.ObjectField(" ", cameraToShake, typeof(Camera), true);
            }
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for specifying how much the camera should be shaken.
            shakeAmount = EditorGUILayout.Slider("Shake Amount", shakeAmount, 0.0f, 1.0f);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for specifying how fast the camera shake should decay.
            shakeDecay = EditorGUILayout.Slider("Shake Decay", shakeDecay, 0.0f, 1.0f);
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
            return "Camera - Shake";
        }
#endif
    }
}