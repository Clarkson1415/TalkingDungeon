using Assets.GracesScripts;
using EasyTransition;
using System;
using UnityEngine;

/// <summary>
/// Load a scene when Triggered 2d collider.
/// </summary>
public class LeadNextScene : MonoBehaviour
{
    public TransitionSettings transition;
    [SerializeField] private string nextScene;

    private bool IsTriggerHit = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsTriggerHit)
        {
            return;
        }

        IsTriggerHit = true;

        // TODO save all player data before scene is loaded
        var player = FindObjectOfType<PlayerDungeon>();
        if (player == null)
        {
            throw new ArgumentNullException("player not found cannot save");
        }

        TalkingDungeonScenes.ChangeScene(nextScene, transition);
    }
}
