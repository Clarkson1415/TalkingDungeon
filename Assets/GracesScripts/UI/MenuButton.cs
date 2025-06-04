using EasyTransition;
using System;
using UnityEngine;
using static SaveGameUtility;
using Assets.GracesScripts;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuButton : DungeonButton
{
    public TransitionSettings transition;
    [SerializeField] private string FeedbackFormURL;

    /// <summary>
    /// Loads the 'IntroScene'
    /// </summary>
    public void StartNewGame()
    {
        PlayerPrefs.DeleteAll();
        TalkingDungeonScenes.LoadScene(TalkingDungeonScenes.Intro, transition, SaveGameState.NewGame);
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
        var scenePlayerSavedIn = PlayerPrefs.GetString(SaveKeys.LastScene);
        TalkingDungeonScenes.LoadScene(scenePlayerSavedIn, transition, SaveGameState.LoadingSave);
    }

    public void SaveAndQuitToTitle()
    {
        this.SaveGame();
        this.QuitToTitle();
    }

    public void QuitToTitle()
    {
        TalkingDungeonScenes.LoadScene("TitleScreen", transition, SaveGameState.QuittingToTitle);
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