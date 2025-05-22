using EasyTransition;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Load a scene when Triggered 2d collider.
/// </summary>
public class LoadIntoNewSceneThenSave : MonoBehaviour
{
    public TransitionSettings transition;
    [SerializeField] private string nextScene;

    private bool IsTriggerHit = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DontDestroyOnLoad(this.gameObject);

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

        SaveGameUtility.SaveGame(player);

        // using transition package
        TransitionManager.Instance().Transition(nextScene, transition, 0f);
    }
}
