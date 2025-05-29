using EasyTransition;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SaveGameUtility;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuButton : DungeonButton
{
    public TransitionSettings transition;
    [SerializeField] private string FeedbackFormURL;
    [SerializeField] public string sceneToLoad;

    /// <summary>
    /// Loads the 'IntroScene'
    /// </summary>
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

        SaveGameUtility.SaveGame(player);
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

        SaveGameUtility.LoadPositionFromSave(player);
    }

    public void SaveAndQuitToTitle()
    {
        this.SaveGame();
        this.QuitToTitle();
    }

    public void QuitToTitle()
    {
        TransitionManager.Instance().Transition("TitleScreen", transition, 0f);
    }

    public void OpenFeedbackForm()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSfg7iF1jkyPRbznepDfAlqV4rfdLAybiiSk9cyvw1LzsPJ35A/viewform?usp=dialog");
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}