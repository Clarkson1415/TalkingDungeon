using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FastFeedback
{
    /// <summary>
    /// The default template for all feedback items. A feedback item triggers specific actions 
    /// when the FastFeedback component is activated.
    /// </summary>
    [System.Serializable]
    public class FeedbackItem
    {
        public bool isEnabled = true;
        public string effectTag = "";

        public enum GameObjectSettings
        {
            TargetGameObject,
            ChildOfTargetWithTag,
            GameObjectWithTag,
            ClosestGameObjectWithTag,
        }

        
        public bool hasError = false;

        public GameObjectSettings targetGameObjectSettings = GameObjectSettings.TargetGameObject;
        public GameObject targetGameObject = null;
        public string targetGameObjectTag = "";

        public AnimationCurveSettings animationCurveSettings = AnimationCurveSettings.EaseInAndOut;
        

        public bool RequiresTag (GameObjectSettings settings)
        {
            if (settings == GameObjectSettings.TargetGameObject) return false;
            else return true;
        }

        public AnimationCurve customCurve = null;

        public enum AnimationCurveSettings
        {
            EaseInAndOut,
            SmoothInAndOut,
            EaseOut,
            EaseIn,
            Linear,
            Constant,
            EarlyEaseInOut,
            LateEaseInOut,
            EaseInOutBounce,
            EaseInBounce,
            EaseOutBounce,

            EaseInAndOutReversed,
            SmoothInAndOutReversed,
            EaseOutReversed,
            EaseInReversed,
            LinearReversed,
            ConstantReversed,
            EarlyEaseInOutReversed,
            LateEaseInOutReversed,
            EaseInOutBounceReversed,
            EaseInBounceReversed,
            EaseOutBounceReversed,
            CustomCurve
        }

        public AnimationCurve GetAnimationCurve()
        {
            if (animationCurveSettings == AnimationCurveSettings.EaseInAndOut)
                return FastFeedbackSettings.Current.easeInAndOut;

            else if (animationCurveSettings == AnimationCurveSettings.SmoothInAndOut)
                return FastFeedbackSettings.Current.smoothInAndOut;

            else if (animationCurveSettings == AnimationCurveSettings.EaseOut)
                return FastFeedbackSettings.Current.easeOut;

            else if (animationCurveSettings == AnimationCurveSettings.EaseIn)
                return FastFeedbackSettings.Current.easeIn;

            else if (animationCurveSettings == AnimationCurveSettings.Linear)
                return FastFeedbackSettings.Current.linear;

            else if (animationCurveSettings == AnimationCurveSettings.Constant)
                return FastFeedbackSettings.Current.constant;

            else if (animationCurveSettings == AnimationCurveSettings.EarlyEaseInOut)
                return FastFeedbackSettings.Current.earlyEaseInAndOut;

            else if (animationCurveSettings == AnimationCurveSettings.LateEaseInOut)
                return FastFeedbackSettings.Current.lateEaseInAndOut;

            else if (animationCurveSettings == AnimationCurveSettings.EaseInOutBounce)
                return FastFeedbackSettings.Current.easeInOutBounce;

            else if (animationCurveSettings == AnimationCurveSettings.EaseInBounce)
                return FastFeedbackSettings.Current.easeInBounce;

            else if (animationCurveSettings == AnimationCurveSettings.EaseOutBounce)
                return FastFeedbackSettings.Current.easeOutBounce;

            else if (animationCurveSettings == AnimationCurveSettings.EaseInAndOutReversed)
                return FastFeedbackSettings.Current.easeInAndOutReversed;

            else if (animationCurveSettings == AnimationCurveSettings.SmoothInAndOutReversed)
                return FastFeedbackSettings.Current.smoothInAndOutReversed;

            else if (animationCurveSettings == AnimationCurveSettings.EaseOutReversed)
                return FastFeedbackSettings.Current.easeOutReversed;

            else if (animationCurveSettings == AnimationCurveSettings.EaseInReversed)
                return FastFeedbackSettings.Current.easeInReversed;

            else if (animationCurveSettings == AnimationCurveSettings.LinearReversed)
                return FastFeedbackSettings.Current.linearReversed;

            else if (animationCurveSettings == AnimationCurveSettings.ConstantReversed)
                return FastFeedbackSettings.Current.constantReversed;

            else if (animationCurveSettings == AnimationCurveSettings.EarlyEaseInOutReversed)
                return FastFeedbackSettings.Current.earlyEaseInAndOutReversed;

            else if (animationCurveSettings == AnimationCurveSettings.LateEaseInOutReversed)
                return FastFeedbackSettings.Current.lateEaseInAndOutReversed;

            else if (animationCurveSettings == AnimationCurveSettings.EaseInOutBounceReversed)
                return FastFeedbackSettings.Current.easeInOutBounceReversed;

            else if (animationCurveSettings == AnimationCurveSettings.EaseInBounceReversed)
                return FastFeedbackSettings.Current.easeInBounceReversed;

            else if (animationCurveSettings == AnimationCurveSettings.EaseOutBounceReversed)
                return FastFeedbackSettings.Current.easeOutBounceReversed;

            else if (animationCurveSettings == AnimationCurveSettings.CustomCurve)
                return customCurve;

            return null;
        }

        public GameObject GetChildWithTag (GameObject obj, string tag)
        {
            foreach (Transform child in obj.transform)
            {
                if (child.CompareTag(tag))
                    return child.gameObject;
            }
            return null;
        }

        public GameObject GetTargetGameObject(GameObject target)
        {
            if (targetGameObjectSettings == GameObjectSettings.TargetGameObject)
            {
                return target;
            }
            else if (targetGameObjectSettings == GameObjectSettings.GameObjectWithTag)
            {
                return GameObject.FindGameObjectWithTag(targetGameObjectTag);
            }
            else if (targetGameObjectSettings == GameObjectSettings.ClosestGameObjectWithTag)
            {
                return GameObject.FindGameObjectWithTag(targetGameObjectTag);
            }
            else if (targetGameObjectSettings == GameObjectSettings.ChildOfTargetWithTag)
            {
                return GetChildWithTag(target, targetGameObjectTag);
            }
            return null;
        }

        public virtual void Activate(GameObject target = null, GameObject origin = null, Vector3 targetPosition = new Vector3()) { }

#if UNITY_EDITOR
        protected static float SPACING_BETWEEN_ITEMS = 2;
        public bool isExpanded = true;

        /// <summary>
        /// Draw the custom editor UI for the effect.
        /// </summary>
        //public virtual void DrawUI(EventFeedback target) { 
        public virtual void DrawUI(GameFeedback target) { 
        
        }

        /// <summary>
        /// Return the custom editor icon for the effect.
        /// </summary>
        public virtual Texture2D GetIcon()
        {
            return EditorGUIUtility.FindTexture("animationvisibilitytoggleon@2x");
        }

        /// <summary>
        /// Return a short description of the effect.
        /// </summary>
        public virtual string GetDescription()
        {
            return "No description provided.";
        }
#endif
    }
}