using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FastFeedback
{
    /// <summary>
    /// Play the specified video, using the specified settings.
    /// </summary>
    [System.Serializable]
    public class PlayVideo : FeedbackItem
    {
        public float playbackSpeed = 1.0f;
        public bool loopVideo = false;

        public PlayConditions playConditions = PlayConditions.UntilFinished;
        public float playForDuration = 1;
        public int playXTimes = 2;

        public StopMode stopMode = StopMode.ResetToStartAndHideVideo;

        public enum StopMode
        {
            ResetToStartAndHideVideo,
            KeepPlaceAndHideVideo,
            KeepPlace,
            ResetToStart
        }

        public enum PlayConditions
        {
            UntilFinished,
            Forever,
            ForXSeconds,
            XTimes
        }

        /// <summary>
        /// Activate the feedback effect, setting up all appropriate actions using the given settings.
        /// </summary>
        public override void Activate(GameObject target = null, GameObject origin = null, Vector3 targetPosition = new Vector3())
        {
            base.Activate(target, origin, targetPosition);

            // Guard clauses.
            GameObject obj = GetTargetGameObject(target);
            if (obj == null) return;
            VideoPlayer video = obj.GetComponent<VideoPlayer>();
            if (video == null) return;

            // Feedback effects.
            VideoControlsEffect effect = obj.GetComponent<VideoControlsEffect>();
            if (effect == null) effect = obj.AddComponent<VideoControlsEffect>();
            effect.Apply(this, target, origin);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw the custom editor UI for the effect.
        /// </summary>
        public override void DrawUI(GameFeedback target)
        {
            base.DrawUI(target);

            // Option for choosing which object to affect.
            targetGameObjectSettings = (GameObjectSettings)EditorGUILayout.EnumPopup("Video On", targetGameObjectSettings);
            if (RequiresTag(targetGameObjectSettings))
                targetGameObjectTag = EditorGUILayout.TextField(" ", targetGameObjectTag);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing how long the video should play for.
            playConditions = (PlayConditions)EditorGUILayout.EnumPopup("Play", playConditions);
            if (playConditions == PlayConditions.ForXSeconds)
            {
                playForDuration = EditorGUILayout.FloatField(" ", playForDuration);
            }
            if (playConditions == PlayConditions.XTimes)
            {
                playXTimes = EditorGUILayout.IntField(" ", playXTimes);
            }
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing what should happen when the video stops playing.
            if (playConditions != PlayConditions.Forever)
            {
                stopMode = (StopMode)EditorGUILayout.EnumPopup("On Stop", stopMode);
                EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);
            }

            // Option for choosing the playback speed of the video.
            playbackSpeed = EditorGUILayout.FloatField("Playback Speed", playbackSpeed);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing whether the video should repeat.
            loopVideo = EditorGUILayout.Toggle("Loop Video", loopVideo);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Error checking.
            hasError = false;
            if (targetGameObjectSettings == GameObjectSettings.GameObjectWithTag && targetGameObjectTag.Length == 0)
            {
                hasError = true;
                EditorGUILayout.HelpBox("You must assign a valid GameObject tag.", MessageType.Error);
            }
        }

        /// <summary>
        /// Return a short description of the effect.
        /// </summary>
        public override string GetDescription()
        {
            return "Video - Play Video";
        }

        /// <summary>
        /// Return the custom editor icon for the effect.
        /// </summary>
        public override Texture2D GetIcon()
        {
            return (Texture2D)EditorGUIUtility.Load(@"d_Camera Icon");
        }
#endif
    }
}