using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Store all global settings for the FastFeedback package. 
/// E.g., Prefabs and Animation Curves.
/// </summary>
public class FastFeedbackSettings : ScriptableObject
{
    private static FastFeedbackSettings _current;

    public static FastFeedbackSettings Current
    {
        get {
            if (_current != null) 
                return _current;
            else
            {
                _current = Resources.LoadAll<FastFeedbackSettings>("")[0];
                return _current;
            }
        }
    }
     
    public GameObject playSoundPrefab;
    public GameObject screenFlashPrefab;
    public GameObject PPFlashPrefab;
    public GameObject timeScalerPrefab;

    // Regular animation curves.
    public AnimationCurve easeInAndOut;
    public AnimationCurve smoothInAndOut;
    public AnimationCurve earlyEaseInAndOut;
    public AnimationCurve lateEaseInAndOut;
    public AnimationCurve easeOut;
    public AnimationCurve easeIn;
    public AnimationCurve linear;
    public AnimationCurve constant;
    public AnimationCurve easeInOutBounce;
    public AnimationCurve easeInBounce;
    public AnimationCurve easeOutBounce;

    // Reversed animation curves.
    public AnimationCurve easeInAndOutReversed;
    public AnimationCurve smoothInAndOutReversed;
    public AnimationCurve earlyEaseInAndOutReversed;
    public AnimationCurve lateEaseInAndOutReversed;
    public AnimationCurve easeOutReversed;
    public AnimationCurve easeInReversed;
    public AnimationCurve linearReversed;
    public AnimationCurve constantReversed;
    public AnimationCurve easeInOutBounceReversed;
    public AnimationCurve easeInBounceReversed;
    public AnimationCurve easeOutBounceReversed;
}
