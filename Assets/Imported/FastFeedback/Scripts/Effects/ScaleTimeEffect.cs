using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FastFeedback;
using UnityEngine.UI;

/// <summary>
/// Controls the visual effect for the screen flash.
/// </summary>
public class ScaleTimeEffect : FastFeedbackEffect
{
    private ScaleTime settings;
    private float startTime;
    private float finishTime;
    public float effectStrength;
    private AnimationCurve curve;

    private float initialTimeScale = 1.0f;

    public static ScaleTimeEffect Instance;

    /// <summary>
    /// Perform all setup for the screen flash effect.
    /// </summary>
    public override void Apply(FeedbackItem item, GameObject target = null, GameObject origin = null)
    {
        base.Apply(item, target, origin);
        Instance = this;
        initialTimeScale = Time.timeScale;
        settings = (ScaleTime)item;
        startTime = Time.unscaledTime;
        finishTime = startTime + settings.scaleTimeDuration;
        effectStrength = settings.timeScaleModifier;
        curve = settings.GetAnimationCurve();
    }

    private void OnDestroy()
    {
        Time.timeScale = initialTimeScale;
    }

    /// <summary>
    /// Destroy the screen flash effect when the time expires.
    /// </summary>
    private void Update()
    {
        float value = effectStrength * curve.Evaluate((Time.unscaledTime - startTime) / (finishTime - startTime));
        Time.timeScale = Mathf.Max(0.0f, value);
        if (Time.unscaledTime > finishTime)
        {
            Destroy(gameObject);
        }
    }
}
