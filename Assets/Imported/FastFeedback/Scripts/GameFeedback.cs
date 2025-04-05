using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FastFeedback;

[CreateAssetMenu(fileName = "FeedbackFX", menuName = "Feedback Effects", order = 1)]
public class GameFeedback : ScriptableObject 
{
    [SerializeReference]
    public List<FeedbackItem> feedbackItems = new List<FeedbackItem>();

    public void ActivateFeedback(GameObject target = null, GameObject origin = null, Vector3 targetPosition = new Vector3())
    {
        foreach (FeedbackItem item in feedbackItems)
        {
            if (item.isEnabled)
                item.Activate(target, origin, targetPosition);
        }
    }

    public void PlaySound(GameObject obj, AudioClip clip)
    {
        FeedbackManager.PlaySound(clip);
    }

    public void PlayAnimation(GameObject obj, string animation)
    {
        FeedbackManager.PlayAnimation(obj, animation);
    }

    public void SetAnimationTrigger(GameObject obj, string trigger)
    {
        FeedbackManager.SetAnimationTrigger(obj, trigger);
    }

    public void PulseColor(GameObject obj, Color color)
    {

    }

    public void PulseSize(GameObject obj, float increase)
    {
        FeedbackManager.PulseSize(obj, increase);
    }

    public void EmitParticles(GameObject obj, ParticleSystem system)
    {

    }
}
