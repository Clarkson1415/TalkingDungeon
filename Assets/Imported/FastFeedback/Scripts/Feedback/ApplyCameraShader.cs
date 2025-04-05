using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FastFeedback
{
    /// <summary>
    /// Applies a custom shader effect to the given camera, using the provided settings.
    /// </summary>
    [System.Serializable]
    public class ApplyCameraShader : FeedbackItem
    {
        public CameraShaderSettings cameraShaderSettings = CameraShaderSettings.Greyscale;

        public CameraSettings cameraSettings = CameraSettings.MainCamera;
        public Camera customCamera;
        public string cameraTag = "";

        public Shader customCameraShader;
        public Material customCameraMaterial;
        public float effectStrength = 1.0f;

        public DurationSettings durationSettings = DurationSettings.Forever;
        public float durationTime = 1.0f;

        public int priority = 5;

        public enum DurationSettings
        {
            Forever,
            ForXSeconds
        };

        public enum CameraShaderSettings
        {
            Blur,
            Greyscale,
            ColorHighlight,
            CustomShader
        }

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

            // Feedback effects.
            CameraShaderEffect effect = cam.gameObject.GetComponent<CameraShaderEffect>();
            if (effect == null) effect = cam.gameObject.AddComponent<CameraShaderEffect>();
            effect.Apply(this, target, origin);
        }

#if UNITY_EDITOR
        public override void DrawUI(GameFeedback target)
        {
            base.DrawUI(target);

            // Option for specifying which camera should receive the effect.
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

            // Option for specifying the effect to apply.
            cameraShaderSettings = (CameraShaderSettings)EditorGUILayout.EnumPopup("Effect", cameraShaderSettings);
            if (cameraShaderSettings == CameraShaderSettings.Blur)
            {
                customCameraShader = Shader.Find("Screen Effects/Blur");
            }
            else if (cameraShaderSettings == CameraShaderSettings.Greyscale)
            {
                customCameraShader = Shader.Find("Screen Effects/Grayscale");
            }
            else if (cameraShaderSettings == CameraShaderSettings.ColorHighlight)
            {
                customCameraShader = Shader.Find("Screen Effects/ColorExtraction");
            }
            else if (cameraShaderSettings == CameraShaderSettings.CustomShader)
            {
                customCameraMaterial = (Material)EditorGUILayout.ObjectField(" ", customCameraMaterial, typeof(Material), false);
            }
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for specifying the effect duration.
            durationSettings = (DurationSettings)EditorGUILayout.EnumPopup("Duration", durationSettings);
            if (durationSettings == DurationSettings.ForXSeconds)
            {
                durationTime = EditorGUILayout.FloatField(" ", durationTime);
                EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

                // Option for specifying the strength of the effect over time.
                animationCurveSettings = (AnimationCurveSettings)EditorGUILayout.EnumPopup("Strength Curve", animationCurveSettings);
                if (animationCurveSettings == AnimationCurveSettings.CustomCurve)
                {
                    if (customCurve == null) customCurve = new AnimationCurve(FastFeedbackSettings.Current.easeInAndOut.keys);
                    customCurve = EditorGUILayout.CurveField(" ", customCurve);
                }
                else
                {
                    GUI.enabled = false;
                    customCurve = GetAnimationCurve();
                    EditorGUILayout.CurveField(" ", customCurve);
                    GUI.enabled = true;
                }
            }
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for specifying the strength of the effect.
            effectStrength = EditorGUILayout.Slider("Effect Strength", effectStrength, 0.0f, 1.0f);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for specifying the priority of the effect.
            priority = EditorGUILayout.IntSlider("Effect Priority", priority, 1, 10);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for specifying a reference key for the effect. Can be used to stop the effect later.
            effectTag = EditorGUILayout.TextField("Reference Key", effectTag);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);
        }

        /// <summary>
        /// Return the custom editor icon for the effect.
        /// </summary>
        public override Texture2D GetIcon()
        {
            return (Texture2D)EditorGUIUtility.Load(@"LightProbeProxyVolume Gizmo");
        }

        /// <summary>
        /// Return a short description of the effect.
        /// </summary>
        public override string GetDescription()
        {
            return "Camera - Add Custom Effect";
        }
#endif
    }
}