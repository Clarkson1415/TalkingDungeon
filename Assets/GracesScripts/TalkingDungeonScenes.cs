﻿using EasyTransition;

public class TalkingDungeonScenes
{
    public const string TitleScreen = "TitleScreen";
    public const string Intro = "Intro";
    public const string Dungeon1 = "Dungeon1";
    public const string Dungeon2 = "Dungeon2";
    public const string Battle = "TurnBased";

    public static void ChangeScene(string scene, TransitionSettings transition)
    {
        TransitionManager.Instance().Transition(scene, transition, 0f);

        // todo other stuff here like loading screen and sound fade out and in
    }
}