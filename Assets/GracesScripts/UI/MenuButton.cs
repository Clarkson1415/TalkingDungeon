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
    [SerializeField] public string sceneToLoad;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nameOfTheFirstScene">the name of the first scene in the whole game.</param>
    public void StartNewGame()
    {
        TransitionManager.Instance().Transition(TalkingDungeonScenes.Intro, transition, 0f);
        Debug.Log($"loading scene: {TalkingDungeonScenes.Intro} from menuButton.cs");
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
        var scenePlayerSavedIn = PlayerPrefs.GetString(SaveKeys.LastScene);
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

        PlayerDataUtility.LoadPositionFromSave(player);
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