using Assets.GracesScripts;
using EasyTransition;
using UnityEngine;
using static SaveGameUtility;

public class TalkingDungeonScenes
{
    public const string TitleScreen = "TitleScreen";
    public const string Intro = "Intro";
    public const string Dungeon1 = "Dungeon1";
    public const string Dungeon2 = "Dungeon2";
    public const string Battle = "TurnBased";

    public static void LoadScene(string scene, TransitionSettings transition, string gameState)
    {
        if (!GameState.GameStates.Contains(gameState))
        {
            Debug.LogError("Game state string must belong to valid game states.");
        }
        
        SaveNewGameState(gameState);

        // add to player prefs
        TransitionManager.Instance().Transition(scene, transition, 0f);
        // todo other stuff here like loading screen and sound fade out and in
    }

    public static void SaveNewGameState(string gameState)
    {
        if (!GameState.GameStates.Contains(gameState))
        {
            Debug.LogError("Game state string must belong to valid game states.");
        }

        PlayerPrefs.SetString(SaveKeys.GameState, gameState);
        PlayerPrefs.Save();
    }
}