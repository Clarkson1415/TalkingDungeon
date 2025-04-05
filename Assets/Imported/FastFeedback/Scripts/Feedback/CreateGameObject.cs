using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FastFeedback
{
    /// <summary>
    /// Instantiate the specified game object, at the specified position and rotation.
    /// </summary>
    [System.Serializable]
    public class CreateGameObject : FeedbackItem
    {
        public GameObject objectToSpawn;

        public PositionMode positionMode = PositionMode.AtTarget;
        public Vector3 targetPosition = Vector3.zero;
        public GameObject targetPositionObject = null;

        public RotationMode rotationMode = RotationMode.CopyTargetGameObject;
        public Vector3 targetEulerRotation = Vector3.zero;
        public GameObject targetRotationObject = null;

        public bool refreshExisting = false;
        public bool destroyAfterEnabled;
        public float destroyAfter;

        public Vector3 offset = Vector3.zero;
        public float scaleModifier = 1.0f;

        public enum PositionMode
        {
            AtTarget,
            AtTargetPosition,
            AtTargetObject,
            AtVector3
        }

        public enum RotationMode
        {
            CopyTargetGameObject,
            None,
            Random,
            EulerAngle
        }

        /// <summary>
        /// Activate the feedback effect, setting up all appropriate actions using the given settings.
        /// </summary>
        public override void Activate(GameObject target = null, GameObject origin = null, Vector3 targetPosition = new Vector3())
        {
            base.Activate(target, origin, targetPosition);

            if (objectToSpawn == null) return;

            Quaternion rotation = Quaternion.identity;
            if (rotationMode == RotationMode.CopyTargetGameObject && target == null)
                rotation = Quaternion.identity;
            if (rotationMode == RotationMode.CopyTargetGameObject)
                rotation = target.transform.rotation;
            else if (rotationMode == RotationMode.CopyTargetGameObject && targetRotationObject != null)
                rotation = targetRotationObject.transform.rotation;
            else if (rotationMode == RotationMode.EulerAngle)
                rotation = Quaternion.Euler(targetEulerRotation);
            else if (rotationMode == RotationMode.Random)
                rotation = Random.rotation;

            Vector3 position = targetPosition;
            if (positionMode == PositionMode.AtTarget && target != null)
                position = target.transform.position;
            else if (positionMode == PositionMode.AtTarget && target == null)
                position = targetPosition;
            else if (positionMode == PositionMode.AtTargetPosition)
                position = targetPosition;
            else if (positionMode == PositionMode.AtTargetObject)
                position = target.transform.position;
            else if (positionMode == PositionMode.AtVector3)
                position = targetPosition;


            // TO FUTURE JOEL: THIS SHOULD BE AN OPTION!
            GameObject obj = null;
            if (positionMode == PositionMode.AtTarget)
            {
                Transform existing = target.transform.Find(objectToSpawn.name);
                if (existing == null || !refreshExisting)
                {
                    obj = GameObject.Instantiate(objectToSpawn, position + offset, rotation);
                    obj.name = objectToSpawn.name;
                    //obj.transform.SetParent(target.transform);
                    obj.transform.localScale = objectToSpawn.transform.localScale * scaleModifier;
                }
                else
                {
                    obj = existing.gameObject;
                }
            }
            else
            {
                obj = GameObject.Instantiate(objectToSpawn, position, rotation);
                obj.name = objectToSpawn.name;
            }

            if (destroyAfter > 0)
                obj.GetOrAddComponent<DestroyAt>().Run(Time.time + destroyAfter);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw the custom editor UI for the effect.
        /// </summary>
        public override void DrawUI(GameFeedback target)
        {
            base.DrawUI(target);
            EditorGUILayout.BeginVertical();

            // Prefab reference for the object to spawn.
            objectToSpawn = (GameObject)EditorGUILayout.ObjectField("Object To Spawn", objectToSpawn, typeof(GameObject), true);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Options for how the game object should be positioned.
            string[] options = new string[] { "Position of Target", "Position of Target Point", "Position of Target Object", "At Vector3" };
            positionMode = (PositionMode)EditorGUILayout.Popup("Position", (int)positionMode, options);
            offset = EditorGUILayout.Vector3Field("Offset", offset);
            
            if (positionMode == PositionMode.AtVector3)
            {
                targetPosition = EditorGUILayout.Vector3Field(" ", targetPosition);
            }
            else if (positionMode == PositionMode.AtTarget)
            {
                string[] trueFalseOptions = new string[] { "True", "False" };
                refreshExisting = EditorGUILayout.Popup("Refresh Existing Effect", refreshExisting ? 0 : 1, trueFalseOptions) == 0;
            }
            //EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);


            // Options for how the game object should be rotated.
            options = new string[] { "Rotation of Target Object", "None", "Random", "Specific Angle" };
            rotationMode = (RotationMode)EditorGUILayout.Popup("Rotation", (int)rotationMode, options);
            if (rotationMode == RotationMode.CopyTargetGameObject)
            {
                targetRotationObject = (GameObject)EditorGUILayout.ObjectField(" ", targetRotationObject, typeof(GameObject), true);
            }
            else if (rotationMode == RotationMode.EulerAngle)
            {
                targetPosition = EditorGUILayout.Vector3Field(" ", targetEulerRotation);
            }
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            scaleModifier = EditorGUILayout.FloatField("Scale", scaleModifier);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Options for if/when the created game object should be destroyed.
            options = new string[] { "Never", "After X Seconds" };
            destroyAfterEnabled = EditorGUILayout.Popup("Destroy After", destroyAfterEnabled ? 1 : 0, options) == 1;
            if (destroyAfterEnabled)
            {
                destroyAfter = EditorGUILayout.FloatField(" ", destroyAfter);
            }
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Handle all error checking.
            hasError = false;
            if (objectToSpawn == null)
            {
                EditorGUILayout.HelpBox("You must assign the GameObject to spawn.", MessageType.Error);
                hasError = true;
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Return the custom editor icon for the effect.
        /// </summary>
        public override string GetDescription()
        {
            return "Game Object - Create";
        }

        /// <summary>
        /// Return a short description of the effect.
        /// </summary>
        public override Texture2D GetIcon()
        {
            return (Texture2D)EditorGUIUtility.Load(@"d_Prefab Icon");
        }
#endif
    }
}