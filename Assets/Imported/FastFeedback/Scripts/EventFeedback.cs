using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FastFeedback;

public class EventFeedback : MonoBehaviour 
{
    [SerializeReference]
    public List<EventFeedbackGroup> feedbackGroups = new List<EventFeedbackGroup>();

    [SerializeField]
    public bool isSetup;

    [System.Serializable]
    public class EventFeedbackGroup
    {
        [SerializeReference]
        public List<FeedbackItem> feedbackItems = new List<FeedbackItem>(); 
        public string feedbackGroupName = "New Feedback Group";
    }


    public void ActivateFeedback ()
    {
        foreach (FeedbackItem item in feedbackGroups[0].feedbackItems)
        {
            //if (item.isEnabled)
            //    item.Activate(this);
        }
    }

    public void ActivateFeedback (string keyword)
    {
        foreach (EventFeedbackGroup group in feedbackGroups)
        {
            if (group.feedbackGroupName == keyword)
            {
                foreach (FeedbackItem item in group.feedbackItems)
                {
                    //if (item.isEnabled)
                    //    item.Activate(this);
                }
            }
        }
    }



    public void PlaySound (AudioClip clip)
    {
        FeedbackManager.PlaySound(clip);
    }

    public void PlayAnimation (string animation)
    {
        FeedbackManager.PlayAnimation(gameObject, animation);
    }

    public void SetAnimationTrigger(string trigger)
    {
        FeedbackManager.SetAnimationTrigger(gameObject, trigger);
    }

    public void PulseColor (Color color)
    {
        
    }

    public void PulseSize (float increase)
    {
        FeedbackManager.PulseSize(gameObject, increase);
    }

    public void EmitParticles(ParticleSystem system)
    {

    }
}
