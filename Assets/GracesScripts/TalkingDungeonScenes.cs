using EasyTransition;
using UnityEngine.SceneManagement;

public class TalkingDungeonScenes
{
    public const string TitleScreen = "TitleScreen";
    public const string Intro = "Intro";
    public const string Dungeon1 = "Dungeon1";
    public const string Dungeon2 = "Dungeon2";
    public const string Battle = "TurnBased";

    public static void LoadScene(string scene, TransitionSettings transition)
    {
        TransitionManager.Instance().Transition(TalkingDungeonScenes.Intro, transition, 0f);
        // todo other stuff here like loading screen and sound fade out and in
    }
}