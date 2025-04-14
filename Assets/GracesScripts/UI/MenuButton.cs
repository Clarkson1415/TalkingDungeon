using EasyTransition;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PlayerDataUtility;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuButton : DungeonButton
{
    public TransitionSettings transition;
    [HideInInspector] public string firstSceneToLoad;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nameOfTheFirstScene">the name of the first scene in the whole game.</param>
    public void StartNewGame()
    {
        TransitionManager.Instance().Transition(this.firstSceneToLoad, transition, 0f);
        SceneManager.sceneLoaded += OnFirstSceneLoadedForTheFirstTime;
    }

    private void OnFirstSceneLoadedForTheFirstTime(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnFirstSceneLoadedForTheFirstTime;

        var player = FindObjectOfType<PlayerDungeon>();
        if (player == null)
        {
            throw new ArgumentNullException("player not found cannot save");
        }

        player.SetupPlayer();

        PlayerDataUtility.SaveGame(player);
    }

    public void SaveGame()
    {
        var player = FindObjectOfType<PlayerDungeon>();
        if (player == null)
        {
            throw new ArgumentNullException("player not found cannot save");
        }

        PlayerDataUtility.SaveGame(player);
    }

    public void LoadGameFromSave()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        var scenePlayerSavedIn = PlayerPrefs.GetString(SaveKeys.Scene);
        TransitionManager.Instance().Transition(scenePlayerSavedIn, transition, 0f);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        var player = FindObjectOfType<PlayerDungeon>();
        if (player == null)
        {
            throw new ArgumentNullException("player not found cannot save");
        }

        PlayerDataUtility.LoadFromSave(player);
        player.SetupPlayer();
    }

    public void SaveAndQuitToTitle()
    {
        this.SaveGame();
        TransitionManager.Instance().Transition("TitleScreen", transition, 0f);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}