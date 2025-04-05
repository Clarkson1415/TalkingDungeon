using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FastFeedback;

/// <summary>
/// Controls the frame-by-frame pulsing of a specific GameObject. 
/// Created from the PulseSize FeedbackItem.
/// </summary>
public class PulseSizeEffect : FastFeedbackEffect
{
    private PulseSize settings;
    private float startTime;
    private float finishTime;
    private float destroyTime;
    private Vector3 startScale;
    private Vector3 endScale;
    private AnimationCurve curve;

    public override void Apply(FeedbackItem item, GameObject target = null, GameObject origin = null)
    {
        base.Apply(item, target, origin);

        // Cache the effect properties.
        settings = (PulseSize)item;
        startTime = Time.time;
        finishTime = Time.time + settings.pulseTime;
        startScale = transform.localScale;
        endScale = startScale * settings.pulseScaleModifier;

        // Calculate the time at which the effect should stop.
        if (settings.pulseDuration == PulseSize.PulseDuration.PulseOnce)
        {
            destroyTime = finishTime;
        }
        else if (settings.pulseDuration == PulseSize.PulseDuration.PulseXTimes)
        {
            destroyTime = Time.time + settings.pulseTime * settings.numberOfPulses;
        }
        else if (settings.pulseDuration == PulseSize.PulseDuration.PulseForXSeconds)
        {
            destroyTime = Time.time + settings.effectDuration;
        }

        // Assign the curve which controls how the object is scaled over time.
        curve = settings.GetAnimationCurve();
    }

    /// <summary>
    /// When this effect is removed, reset the object back to its default scale.
    /// </summary>
    private void OnDestroy()
    {
        transform.localScale = startScale;
    }

    private void Update()
    {
        float mod = curve.Evaluate(((Time.time - startTime) % settings.pulseTime) / settings.pulseTime);
        transform.localScale = Vector3.Lerp(startScale, endScale, mod);

        if (Time.time > destroyTime)
        {
            transform.localScale = startScale;
            Destroy(this);
        }
    }
}
