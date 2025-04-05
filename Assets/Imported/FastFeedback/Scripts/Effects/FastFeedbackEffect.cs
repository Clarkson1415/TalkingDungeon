using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FastFeedback;

/// <summary>
/// Template for all fast feedback effects.
/// </summary>
public class FastFeedbackEffect : MonoBehaviour
{
    public virtual void Apply(FeedbackItem item, GameObject target = null, GameObject origin = null) { }
    public virtual void Reapply(FeedbackItem item, GameObject target = null, GameObject origin = null) { }
}
