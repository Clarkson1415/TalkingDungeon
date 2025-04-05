using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FastFeedback;
using UnityEngine.UI;

/// <summary>
/// Controls the visual effect for the screen flash.
/// </summary>
public class ScreenFlashEffect : FastFeedbackEffect
{
    private ScreenFlash settings;
    private float startTime;
    private float finishTime;
    private CanvasGroup canvasGroup;
    private AnimationCurve curve;

    /// <summary>
    /// Perform all setup for the screen flash effect.
    /// </summary>
    public override void Apply(FeedbackItem item, GameObject target = null, GameObject origin = null)
    {
        base.Apply(item, target, origin);
        settings = (ScreenFlash)item;
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        GetComponentInChildren<RawImage>().texture = settings.spriteToFlash;
        GetComponentInChildren<RawImage>().color = settings.colorToFlash;
        startTime = Time.time;
        finishTime = startTime + settings.timeToFlash;

        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = settings.customCamera;
        canvas.planeDistance = settings.customCamera.nearClipPlane + 0.01f;
        curve = settings.GetAnimationCurve();
    }

    /// <summary>
    /// Destroy the screen flash effect when the time expires.
    /// </summary>
    private void Update()
    {
        canvasGroup.alpha = curve.Evaluate((Time.time - startTime) / (finishTime - startTime));
        if (Time.time > finishTime)
        {
            Destroy(gameObject);
        }
    }
}
