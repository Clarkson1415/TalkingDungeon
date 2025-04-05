using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FastFeedback;
using UnityEngine.UI;

/// <summary>
/// Controls the playback of a sound effect. 
/// Created from the PlaySound FeedbackItem.
/// </summary>
public class PlaySoundEffect : FastFeedbackEffect
{
    private PlaySound settings;
    private AudioSource source;
    private float stopPlayingAt;

    /// <summary>
    /// Apply all logic for playing the sound. 
    /// </summary>
    public override void Apply(FeedbackItem item, GameObject target = null, GameObject origin = null)
    {
        base.Apply(item, target, origin);
        settings = (PlaySound)item;
        source = GetComponentInChildren<AudioSource>();
        

        // Select the sound to play.
        if (settings.soundChoiceSettings == PlaySound.SoundChoiceSettings.SpecificSound)
            source.clip = settings.soundToPlay;
        else if (settings.soundChoiceSettings == PlaySound.SoundChoiceSettings.RandomSound)
            source.clip = settings.randomisedSounds[Random.Range(0, settings.randomisedSoundCount)];

        // Set the volume of the sound.
        source.volume = settings.volume;

        // Set the pitch of the sound.
        source.pitch = Random.Range(settings.minPitchChange, settings.maxPitchChange);

        // If the sound isn't 2D, move the sound object to the correct location.
        if (settings.soundSettings != PlaySound.SoundSettings.Sound2D)
        {
            source.spatialBlend = 1.0f;

            // Choose the target location for the sound.
            if (settings.soundSettings == PlaySound.SoundSettings.SoundOnOtherObject)
                target = settings.soundTarget;
            else if (settings.soundSettings == PlaySound.SoundSettings.SoundOnObjectWithTag)
                target = GameObject.FindGameObjectWithTag(settings.targetTag);

            // Move the sound game object to that location.
            if (target != null)
                gameObject.transform.position = target.transform.position;
        }

        // Set the sound loop conditions.
        source.loop = !(settings.playConditions == PlaySound.PlayConditions.PlayOnce);
        
        // Calculate when the sound should stop playing.
        if (settings.playConditions == PlaySound.PlayConditions.PlayOnce)
            stopPlayingAt = Time.time + settings.soundToPlay.length;
        else if (settings.playConditions == PlaySound.PlayConditions.PlayXTimes)
            stopPlayingAt = Time.time + settings.soundToPlay.length * settings.timesToPlay;
        else
            stopPlayingAt = Time.time + settings.durationToPlayFor;

        // Play the sound.
        source.Play();
    }
     
    /// <summary>
    /// Check to see if the sound has finished playing.
    /// </summary>
    private void Update()
    {
        if (Time.time > stopPlayingAt)
        {
            Destroy(gameObject);
        }
    }
}
