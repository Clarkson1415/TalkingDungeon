using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FastFeedback
{
    /// <summary>
    /// Play the specified sound, using the specified settings. Settings include
    /// 2D/3D, play location, volume, volume curve, repeat settings, pitch randomisation etc.
    /// </summary>
    [System.Serializable]
    public class PlaySound : FeedbackItem
    {
        public AudioClip soundToPlay;
        public float volume = 1.0f;
        public PlayConditions playConditions;
        public int timesToPlay;
        public float durationToPlayFor;

        public SoundSettings soundSettings;
        public GameObject soundTarget;
        public string targetTag = "";

        public float minPitchChange = 1.0f, maxPitchChange = 1.0f;

        public GameObjectSettings soundTargetSettings = GameObjectSettings.TargetGameObject;

        public SoundChoiceSettings soundChoiceSettings = SoundChoiceSettings.SpecificSound;
        public AudioClip[] randomisedSounds = new AudioClip[50];
        public int randomisedSoundCount = 2;

        public enum SoundChoiceSettings
        {
            SpecificSound,
            RandomSound
        };

        public enum PlayConditions
        {
            PlayOnce,
            PlayXTimes,
            PlayForXSeconds
        };

        public enum SoundSettings
        {
            Sound2D,
            SoundOnThisObject,
            SoundOnOtherObject,
            SoundOnObjectWithTag
        };

        /// <summary>
        /// Activate the feedback effect, setting up all appropriate actions using the given settings.
        /// </summary>
        public override void Activate(GameObject target = null, GameObject origin = null, Vector3 targetPosition = new Vector3())
        {
            base.Activate(target, origin, targetPosition);
            if (soundToPlay != null)
            {
                GameObject obj = GameObject.Instantiate(Resources.LoadAll<FastFeedbackSettings>("")[0].playSoundPrefab);
                obj.GetComponent<PlaySoundEffect>().Apply(this, target, origin);

                /*
                if (playConditions == PlayConditions.PlayOnce)
                    GameManager.music.PlaySound(soundToPlay, volume);
                else
                {
                    GameObject obj = GameObject.Instantiate(Resources.LoadAll<FastFeedbackSettings>("")[0].playSoundPrefab);
                    obj.GetComponent<PlaySoundEffect>().Apply(this, target, origin);
                }
                */
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw the custom editor UI for the effect.
        /// </summary>
        public override void DrawUI(GameFeedback target)
        {
            base.DrawUI(target);

            // Option for choosing the sound to play.
            if (randomisedSounds.Length < 50) randomisedSounds = new AudioClip[50];
            soundChoiceSettings = (SoundChoiceSettings)EditorGUILayout.EnumPopup("Type of Sound", soundChoiceSettings);
            if (soundChoiceSettings == SoundChoiceSettings.SpecificSound)
            {
                soundToPlay = (AudioClip)EditorGUILayout.ObjectField("Sound to Play", soundToPlay, typeof(AudioClip), true);
            }
            else if (soundChoiceSettings == SoundChoiceSettings.RandomSound)
            {
                randomisedSoundCount = EditorGUILayout.IntField("Clips in Random Group", randomisedSoundCount);
                for (int i = 0; i < randomisedSoundCount; i++)
                {
                    randomisedSounds[i] = (AudioClip)EditorGUILayout.ObjectField($"  [{i}]", randomisedSounds[i], typeof(AudioClip), true);
                }
            }
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for choosing the sound volume.
            volume = EditorGUILayout.Slider("Volume", volume, 0.0f, 1.0f);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);
            
            // Option for choosing the sound repeat settings.
            string[] options = new string[] { "Play Once", "Play X Times", "Play for X Seconds" };
            playConditions = (PlayConditions)EditorGUILayout.Popup("Play Conditions", (int)playConditions, options);
            if (playConditions == PlayConditions.PlayXTimes)
            {
                timesToPlay = EditorGUILayout.IntField(" ", timesToPlay);
            }
            else if (playConditions == PlayConditions.PlayForXSeconds)
            {
                durationToPlayFor = EditorGUILayout.FloatField(" ", durationToPlayFor);
            }
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for specifying where the sound should be played.
            soundSettings = (SoundSettings)EditorGUILayout.EnumPopup("Sound Settings", soundSettings);
            if (soundSettings == SoundSettings.SoundOnOtherObject)
            {
                soundTarget = (GameObject)EditorGUILayout.ObjectField(" ", soundTarget, typeof(GameObject), true);
            }
            else if (soundSettings == SoundSettings.SoundOnObjectWithTag)
            {
                targetTag = EditorGUILayout.TextField(" ", targetTag);
            }
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Option for randomising the sound pitch.
            minPitchChange = EditorGUILayout.Slider("Minimum Random Pitch", minPitchChange, 0.5f, 3.0f);
            maxPitchChange = EditorGUILayout.Slider("Maximum Random Pitch", maxPitchChange, 0.5f, 3.0f);
            EditorGUILayout.Space(SPACING_BETWEEN_ITEMS);

            // Error checking.
            hasError = false;
            if (soundToPlay == null && soundChoiceSettings == SoundChoiceSettings.SpecificSound)
            {
                EditorGUILayout.HelpBox("You must assign an Audio Clip to play.", MessageType.Error);
                hasError = true;
            }
            if (soundSettings == SoundSettings.SoundOnOtherObject && soundTarget == null)
            {
                EditorGUILayout.HelpBox("You must assign the GameObject specifying where to play the sound.", MessageType.Error);
                hasError = true;
            }
            if (soundSettings == SoundSettings.SoundOnObjectWithTag && targetTag.Length == 0)
            {
                EditorGUILayout.HelpBox("You must assign the tag of the GameObject specifying where to play the sound.", MessageType.Error);
                hasError = true;
            }
        }

        /// <summary>
        /// Return the custom editor icon for the effect.
        /// </summary>
        public override Texture2D GetIcon()
        {
            return (Texture2D)EditorGUIUtility.Load(@"d_AudioClip Icon");
        }

        /// <summary>
        /// Return a short description of the effect.
        /// </summary>
        public override string GetDescription()
        {
            return "Audio - Play Sound";
        }
#endif
    }
}