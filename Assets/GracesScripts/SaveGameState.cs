using System.Collections.Generic;

namespace Assets.GracesScripts
{
    public static class SaveGameState
    {
        public const string NewGame = "NewGame";
        public const string BattleLost = "BattleLost";
        public const string BattleWon = "BattleWon";
        public const string LoadingSave = "LoadingSave";
        public const string LoadIntoNewScene = "RegularSceneChange";
        public const string QuittingToTitle = "QuittingToTitle";
        public const string BattleRanAway = "BattleRunAwaySuccess";
        public const string StartedBattle = "StartedBattle";

        public static List<string> GameStates => new() { NewGame, BattleLost, BattleWon, LoadingSave, LoadIntoNewScene, QuittingToTitle, StartedBattle };
    }
}
