using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FastFeedback
{
    /// <summary>
    /// Set the specific trigger on a given animator component.
    /// </summary>
    [System.Serializable]
    public class SetAnimatorVariable : FeedbackItem
    {
        public VariableType variableType = VariableType.Bool;
        public int intValue;
        public float floatValue;
        public bool boolValue;
        
        public enum VariableType
        {
            Bool,
            Float,
            Int
        }

        public string variableName = "";

        /// <summary>
        /// Activate the feedback effect, setting up all appropriate actions using the given settings.
        /// </summary>
        public override void Activate(GameObject target = null, GameObject origin = null, Vector3 targetPosition = new Vector3())
        {
            base.Activate(target, origin, targetPosition);

            // Guard clauses.
            if (variableName.Length == 0) return;
            GameObject obj = GetTargetGameObject(target);
            if (obj == null) return;
            Animator animator = obj.GetComponentInChildren<Animator>();
            if (animator == null) return;

            // Feedback actions.
            if (variableType == VariableType.Bool)
                animator.SetBool(variableName, boolValue);
            else if (variableType == VariableType.Float)
                animator.SetFloat(variableName, floatValue);
            else if (variableType == VariableType.Int)
                animator.SetFloat(variableName, intValue);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw the custom editor UI for the effect.
        /// </summary>
        public override void DrawUI(GameFeedback target)
        {
            base.DrawUI(target);

            // Option for choosing the object to affect.
            targetGameObjectSettings = (GameObjectSettings)EditorGUILayout.EnumPopup("Animator On", targetGameObjectSettings);
            if (targetGameObjectSettings == GameObjectSettings.GameObjectWithTag)
                targetGameObjectTag = EditorGUILayout.TextField(" ", targetGameObjectTag);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing the name of the trigger to set.
            variableName = EditorGUILayout.TextField("Variable Name", variableName);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for specifying the type of variable.
            variableType = (VariableType)EditorGUILayout.EnumPopup("Variable Type", variableType);
            if (variableType == VariableType.Bool)
            {
                boolValue = EditorGUILayout.Toggle("Value", boolValue);
            }
            else if (variableType == VariableType.Float)
            {
                floatValue = EditorGUILayout.FloatField("Value", floatValue);
            }
            else if (variableType == VariableType.Int)
            {
                intValue = EditorGUILayout.IntField("Value", intValue);
            }
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Handle error logic.
            hasError = false;
            if (RequiresTag(targetGameObjectSettings) && targetGameObjectTag.Length == 0)
            {
                EditorGUILayout.HelpBox("You must assign a valid game object tag.", MessageType.Error);
                hasError = true;
            }
            if (variableName.Length == 0)
            {
                EditorGUILayout.HelpBox("You must specify the variable name.", MessageType.Error);
                hasError = true;
            }
        }

        /// <summary>
        /// Return a short description of the effect.
        /// </summary>
        public override string GetDescription()
        {
            return "Animator - Set Variable Value";
        }

        /// <summary>
        /// Return the custom editor icon for the effect.
        /// </summary>
        public override Texture2D GetIcon()
        {
            return (Texture2D)EditorGUIUtility.Load(@"d_AnimationClip Icon");
        }
#endif
    }
}