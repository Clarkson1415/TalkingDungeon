using EasyTransition;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
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

        PlayerDataUtility.SaveGame(player);

        SceneManager.sceneLoaded += OnSceneLoaded;

        // using transition package
        TransitionManager.Instance().Transition(nextScene, transition, 0f);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // after scene has loaded overwrite player values except position
        var player = FindObjectOfType<PlayerDungeon>();
        if (player == null)
        {
            throw new ArgumentNullException("player not found cannot save");
        }

        // load all saved values of like items and abilities and health etc.
        PlayerDataUtility.LoadSaveDataOnPlayerNotPosition(player);

        // save ALL player data as now in new scene where position should be saved
        PlayerDataUtility.SaveGame(player);

        player.SetupPlayer();

        // then destroy this gameobject no longer needed in scene
        if (this == null)
        {
            return;
        }

        if (this.gameObject == null)
        {
            return;
        }

        Destroy(this.gameObject);
    }
}
