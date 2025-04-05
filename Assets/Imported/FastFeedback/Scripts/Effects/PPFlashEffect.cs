using FastFeedback;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PPFlashEffect : FastFeedbackEffect
{
    public PostProcessVolume volume;
    public PostProcessProfile profile;
    public bool inUse = false;

    private float strengthModifier;
    private AnimationCurve strengthCurve;
    private float startTime, finishTime, duration;

    private void Awake()
    {
        volume = GetComponent<PostProcessVolume>();    
    }

    public override void Apply(FeedbackItem item, GameObject target = null, GameObject origin = null)
    {
        base.Apply(item, target, origin);
        PPFlashProfile settings = (PPFlashProfile)item;

        profile = settings.postProcessProfile;
        strengthModifier = settings.flashStrength;
        strengthCurve = settings.GetAnimationCurve();

        inUse = true;
        duration = settings.timeToFlash;
        startTime = Time.time;
        finishTime = Time.time + duration;

        volume.enabled = true;
        volume.weight = strengthModifier * strengthCurve.Evaluate(0);
        volume.profile = profile;
    }

    private void Update()
    {
        if (!inUse) return;

        volume.weight = strengthModifier * strengthCurve.Evaluate((Time.time - startTime) / (finishTime - startTime));
        if (Time.time >= finishTime)
        {
            inUse = false;
            volume.enabled = false;
        }
    }
}
