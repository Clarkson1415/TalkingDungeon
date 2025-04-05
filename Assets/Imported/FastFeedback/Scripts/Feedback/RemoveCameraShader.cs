using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FastFeedback
{
    /// <summary>
    /// Remove any custom shader effects matching the specified conditions on the specified camera.
    /// </summary>
    [System.Serializable]
    public class RemoveCameraShader : FeedbackItem
    {
        public DisableSettings disableSettings;
        public int disablePriority = 1;
        public CameraSettings cameraSettings = CameraSettings.MainCamera;
        public Camera customCamera;
        public string cameraTag = "";

        public enum CameraSettings
        {
            MainCamera,
            CustomCamera,
            CameraWithTag
        };

        public enum DisableSettings
        {
            DisableAllEffects,
            DisableEffectsWithTag,
            DisableEffectsWithPriority,
            DisableEffectsLessThanPriority,
            DisableEffectsGreaterThanPriority
        }

        /// <summary>
        /// Activate the feedback effect, setting up all appropriate actions.
        /// </summary>
        public override void Activate(GameObject target = null, GameObject origin = null, Vector3 targetPosition = new Vector3())
        {
            base.Activate(target, origin, targetPosition);

            // Guard clauses.
            Camera cam = customCamera;
            if (cameraSettings == CameraSettings.MainCamera)
                cam = Camera.main;
            else if (cameraSettings == CameraSettings.CameraWithTag)
            {
                GameObject obj = GameObject.FindGameObjectWithTag(cameraTag);
                if (obj == null) return;
                cam = obj.GetComponent<Camera>();
            }
            if (cam == null) return;


            /*
            GameObject obj = GetTargetGameObject(feedback);
            if (obj == null) return;
            EnableOutlineEffect outlineEffect = obj.GetComponent<EnableOutlineEffect>();
            if (outlineEffect == null) return;

            if (disableSettings == DisableSettings.DisableAllEffects)
                outlineEffect.RemoveAllOutlines();
            else if (disableSettings == DisableSettings.DisableEffectsWithTag)
                outlineEffect.RemoveOutlinesWithTag(effectTag);
            else if (disableSettings == DisableSettings.DisableEffectsGreaterThanPriority)
                outlineEffect.RemoveOutlinesGreaterThanPriority(disablePriority);
            else if (disableSettings == DisableSettings.DisableEffectsLessThanPriority)
                outlineEffect.RemoveOutlinesLessThanPriority(disablePriority);
            else if (disableSettings == DisableSettings.DisableEffectsWithPriority)
                outlineEffect.RemoveOutlinesWithPriority(disablePriority);
            */
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw the custom editor UI for the effect.
        /// </summary>
        public override void DrawUI(GameFeedback target)
        {
            base.DrawUI(target);

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

            // Choose which effects should be disabled on the camera.
            disableSettings = (DisableSettings)EditorGUILayout.EnumPopup("Disable Settings", disableSettings);
            if (disableSettings == DisableSettings.DisableEffectsWithTag)
            {
                effectTag = EditorGUILayout.TextField(" ", effectTag);
            }
            else if (disableSettings == DisableSettings.DisableEffectsWithPriority ||
                disableSettings == DisableSettings.DisableEffectsLessThanPriority ||
                disableSettings == DisableSettings.DisableEffectsGreaterThanPriority)
            {
                disablePriority = EditorGUILayout.IntSlider(" ", disablePriority, 1, 10);
            }
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Error checking.
            hasError = false;
            if (cameraSettings == CameraSettings.CustomCamera && customCamera == null)
            {
                EditorGUILayout.HelpBox("You must assign a valid target camera.", MessageType.Error);
                hasError = true;
            }
            if (disableSettings == DisableSettings.DisableEffectsWithTag && effectTag.Length == 0)
            {
                hasError = true;
                EditorGUILayout.HelpBox("You must assign a valid tag.", MessageType.Error);
            }
        }

        /// <summary>
        /// Return a short description of the effect.
        /// </summary>
        public override string GetDescription()
        {
            return "Camera - Remove Custom Effect";
        }

        /// <summary>
        /// Return the custom editor icon for the effect.
        /// </summary>
        public override Texture2D GetIcon()
        {
            return (Texture2D)EditorGUIUtility.Load(@"LightProbeProxyVolume Gizmo");
        }
#endif
    }
}