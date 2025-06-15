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
        TalkingDungeonScenes.ChangeScene(TalkingDungeonScenes.Intro, transition);
        Debug.Log($"loading scene: {TalkingDungeonScenes.Intro} from menuButton.cs");
    }

    public void SaveGame()
    {
        var player = FindObjectOfType<PlayerDungeon>();
        if (player == null)
        {
            throw new ArgumentNullException("player not found cannot save");
        }

        SaveGameUtility.SaveGame();
    }

    public void LoadGameFromSave()
    {
        var scenePlayerSavedIn = PlayerPrefs.GetString(SaveKeys.LastScene);
        TalkingDungeonScenes.ChangeScene(scenePlayerSavedIn, transition);
    }

    public void SaveAndQuitToTitle()
    {
        this.SaveGame();
        this.QuitToTitle();
    }

    public void QuitToTitle()
    {
        TalkingDungeonScenes.ChangeScene("TitleScreen", transition);
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