using EasyTransition;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PlayerDataUtility;
using Unity.VisualScripting;
using System.Collections;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuButton : DungeonButton
{
    public TransitionSettings transition;
    [SerializeField] private string FeedbackFormURL;
    [SerializeField] public string sceneToLoad;
    private SavedAnimationText saveText;

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

        this.saveText = FindObjectOfType<SavedAnimationText>();
        if (saveText == null)
        {
            Debug.LogWarning("not sure why savetext is null idk if it should be or not.");
            return;
        }

        this.saveText.PlaySavedAnimation();
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

    public void OpenFeedbackForm()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSfg7iF1jkyPRbznepDfAlqV4rfdLAybiiSk9cyvw1LzsPJ35A/viewform?usp=dialog");
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